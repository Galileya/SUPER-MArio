using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScreen : MonoBehaviour
{
    private int selection;
    private GUIStyle title, text, button, small;
    private void Update()
    {
        string scene = SceneManager.GetActiveScene().name;
        if (GameManager.Instance.Playing) return;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) selection = 1 - selection;
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (scene == "Menu" || selection == 0) GameManager.Instance.NewGame();
            else GameManager.Instance.Menu();
        }
    }
    private void OnGUI()
    {
        if (title == null)
        {
            title = new GUIStyle(GUI.skin.label) { fontSize = 68, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            title.normal.textColor = new Color(1, .82f, .35f);
            text = new GUIStyle(GUI.skin.label) { fontSize = 23, alignment = TextAnchor.MiddleCenter };
            small = new GUIStyle(text) { fontSize = 17 };
            button = new GUIStyle(GUI.skin.button) { fontSize = 24, fontStyle = FontStyle.Bold };
        }
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 1280f, Screen.height / 720f, 1));
        var game = GameManager.Instance;
        if (game.Playing)
        {
            GUI.Box(new Rect(0, 0, 1280, 80), GUIContent.none);
            GUI.Label(new Rect(20, 12, 300, 60), $"MARIO  {game.score:000000}\nVIDAS  {game.lives}", text);
            GUI.Label(new Rect(390, 12, 220, 60), $"MONEDAS\n× {game.coins:00}", text);
            GUI.Label(new Rect(670, 12, 220, 60), $"MUNDO\n1-{game.stage}", text);
            GUI.Label(new Rect(970, 12, 250, 60), $"TIEMPO\n{Mathf.CeilToInt(game.timeRemaining):000}", text);
            if (!game.Paused) return;
            GUI.Box(new Rect(390, 190, 500, 320), "");
            GUI.Label(new Rect(400, 210, 480, 60), "PAUSA", title);
            if (GUI.Button(new Rect(450, 310, 380, 55), "CONTINUAR", button)) game.TogglePause();
            if (GUI.Button(new Rect(450, 390, 380, 55), "VOLVER AL MENÚ", button)) game.Menu();
            return;
        }
        string scene = SceneManager.GetActiveScene().name;
        bool menu = scene == "Menu";
        bool victory = scene == "Victory";
        GUI.Label(new Rect(120, 100, 1040, 170), menu ? "SUPER\nMARIO BROS." : victory ? "¡REINO RESCATADO!" : "GAME OVER", title);
        GUI.Label(new Rect(190, 280, 900, 50), menu ? "CUATRO MUNDOS · UNA AVENTURA" : $"PUNTAJE FINAL   {game.score:000000}", text);
        if (GUI.Button(new Rect(420, 370, 440, 58), (selection == 0 ? "► " : "") + (menu ? "1 JUGADOR" : "VOLVER A JUGAR"), button)) game.NewGame();
        if (GUI.Button(new Rect(420, 450, 440, 58), (selection == 1 ? "► " : "") + (menu ? "2 JUGADORES · INICIAR" : "MENÚ PRINCIPAL"), button))
        { if (menu) game.NewGame(); else game.Menu(); }
        GUI.Label(new Rect(140, 550, 1000, 40), "← → / A D: mover     ESPACIO: saltar / nadar     ESC: pausa", small);
        GUI.Label(new Rect(140, 590, 1000, 40), "↑ ↓: elegir     ENTER: confirmar     ·     Proyecto académico", small);
        if (menu) GUI.Label(new Rect(200, 635, 880, 32), "Ambas opciones inician una partida individual, según la consigna.", small);
    }
}
