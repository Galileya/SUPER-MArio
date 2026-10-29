using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform root;
    private Font font;
    private Text score, coins, world, timer, first, second;
    private GameObject pause;
    private int selection;
    private readonly Color ink = new Color(.045f,.065f,.12f,.96f);
    private void Awake()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var obj = new GameObject("Game UI",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
        obj.transform.SetParent(transform);
        canvas = obj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.planeDistance = 1; canvas.sortingOrder = 100;
        var scaler = obj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280,720); scaler.matchWidthOrHeight = .5f;
        root = obj.GetComponent<RectTransform>();
        var events = new GameObject("UI events", typeof(EventSystem),typeof(StandaloneInputModule));
        events.transform.SetParent(transform); events.GetComponent<EventSystem>().sendNavigationEvents = false;
        SceneManager.sceneLoaded += Build;
    }
    private void OnDestroy() { SceneManager.sceneLoaded -= Build; }
    private void Build(Scene scene, LoadSceneMode mode)
    {
        foreach (Transform child in root) Destroy(child.gameObject);
        canvas.worldCamera = Camera.main;
        selection = 0;
        var game = GameManager.Instance;
        if (game.Playing)
        {
            Panel(root,"HUD",new Rect(0,0,1280,80),ink);
            score = Label(root,new Rect(20,10,300,60),"",23);
            coins = Label(root,new Rect(390,10,220,60),"",23);
            world = Label(root,new Rect(670,10,220,60),"",23);
            timer = Label(root,new Rect(970,10,250,60),"",23);
            pause = Panel(root,"Pause",new Rect(390,190,500,320),ink).gameObject;
            var parent = pause.GetComponent<RectTransform>();
            Label(parent,new Rect(20,20,460,70),"PAUSA",48);
            Button(parent,new Rect(60,120,380,55),"CONTINUAR",game.TogglePause);
            Button(parent,new Rect(60,200,380,55),"VOLVER AL MENÚ",game.Menu);
            pause.SetActive(false);
        }
        else
        {
            bool menu = scene.name == "Menu";
            bool victory = scene.name == "Victory";
            Label(root,new Rect(120,80,1040,180),menu ? "SUPER\nMARIO BROS." : victory ? "¡REINO RESCATADO!" : "GAME OVER",64,new Color(1,.82f,.35f));
            Label(root,new Rect(190,280,900,50),menu ? "CUATRO MUNDOS · UNA AVENTURA" : $"PUNTAJE FINAL   {game.score:000000}",23);
            first = Button(root,new Rect(420,370,440,58),"",game.NewGame);
            second = Button(root,new Rect(420,450,440,58),"",() => { if(menu) game.NewGame(); else game.Menu(); });
            Label(root,new Rect(100,550,1080,40),"← → / A D: mover     ESPACIO: saltar / nadar     ESC: pausa",18);
            Label(root,new Rect(100,590,1080,40),"↑ ↓: elegir     ENTER: confirmar     ·     Proyecto académico",18);
            if(menu) Label(root,new Rect(100,635,1080,32),"Ambas opciones inician una partida individual, según la consigna.",16);
        }
    }
    private void Update()
    {
        var game = GameManager.Instance;
        if(game.Playing)
        {
            if(score == null) return;
            score.text = $"MARIO  {game.score:000000}\nVIDAS  {game.lives}";
            coins.text = $"MONEDAS\n× {game.coins:00}";
            world.text = $"MUNDO\n1-{game.stage}";
            timer.text = $"TIEMPO\n{Mathf.CeilToInt(game.timeRemaining):000}";
            pause.SetActive(game.Paused);
            return;
        }
        if(first == null) return;
        bool menu = SceneManager.GetActiveScene().name == "Menu";
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) selection = 1-selection;
        first.text = (selection == 0 ? "► " : "") + (menu ? "1 JUGADOR" : "VOLVER A JUGAR");
        second.text = (selection == 1 ? "► " : "") + (menu ? "2 JUGADORES · INICIAR" : "MENÚ PRINCIPAL");
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        { if(menu || selection == 0) game.NewGame(); else game.Menu(); }
    }
    private RectTransform Rect(Transform parent,string name,Rect area)
    {
        var obj = new GameObject(name,typeof(RectTransform)); obj.transform.SetParent(parent,false);
        var rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0,1);
        rect.anchoredPosition = new Vector2(area.x,-area.y); rect.sizeDelta = area.size;
        return rect;
    }
    private RectTransform Panel(Transform parent,string name,Rect area,Color color)
    {
        var rect = Rect(parent,name,area); rect.gameObject.AddComponent<Image>().color = color; return rect;
    }
    private Text Label(Transform parent,Rect area,string content,int size,Color? tint = null)
    {
        var rect = Rect(parent,"Label",area); var label = rect.gameObject.AddComponent<Text>();
        label.font = font; label.text = content; label.fontSize = size; label.alignment = TextAnchor.MiddleCenter;
        label.color = tint ?? Color.white; label.fontStyle = FontStyle.Bold;
        label.horizontalOverflow = HorizontalWrapMode.Overflow; label.raycastTarget = false;
        var shadow = rect.gameObject.AddComponent<Shadow>(); shadow.effectDistance = new Vector2(2,-2);
        return label;
    }
    private Text Button(Transform parent,Rect area,string content,UnityAction action)
    {
        var rect = Panel(parent,"Button",area,new Color(.14f,.25f,.38f,.98f));
        var button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = rect.GetComponent<Image>();
        button.onClick.AddListener(action);
        var colors = button.colors; colors.highlightedColor = new Color(1,.8f,.4f); button.colors = colors;
        return Label(rect,new Rect(0,0,area.width,area.height),content,24);
    }
}
