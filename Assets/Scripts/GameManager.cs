using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int world { get; private set; } = 1;
    public int stage { get; private set; } = 1;
    public int lives { get; private set; } = 3;
    public int coins { get; private set; }
    public int score { get; private set; }
    public float timeRemaining { get; private set; } = 400;
    public bool Playing => SceneManager.GetActiveScene().name.StartsWith("1-");
    public bool Paused { get; private set; }
    public bool Transitioning { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance == null) new GameObject("Game Session").AddComponent<GameManager>();
    }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.AddComponent<GameAudio>();
        gameObject.AddComponent<GameScreen>();
        SceneManager.sceneLoaded += SceneLoaded;
        Application.targetFrameRate = 60;
    }
    private void OnDestroy()
    {
        if (Instance != this) return;
        SceneManager.sceneLoaded -= SceneLoaded;
        Instance = null;
    }
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("1-")) stage = int.Parse(scene.name.Substring(2));
        Time.timeScale = 1;
        Paused = Transitioning = false;
        timeRemaining = stage == 3 ? 300 : 400;
        GameAudio.Instance.SetTheme(scene.name);
    }
    private void Update()
    {
        if (!Playing || Transitioning) return;
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
        var player = FindObjectOfType<Player>();
        if (Paused || player == null || player.dead || !player.GetComponent<PlayerMovement>().enabled) return;
        timeRemaining = Mathf.Max(0, timeRemaining - Time.deltaTime);
        if (timeRemaining <= 0) player.Death();
    }
    public void TogglePause() { Paused = !Paused; Time.timeScale = Paused ? 0 : 1; }
    public void NewGame() { StartAtLevel(1); }
    public void StartAtLevel(int selectedStage)
    {
        if (selectedStage < 1 || selectedStage > 4) return;
        CancelInvoke(); lives = 3; coins = score = 0;
        LoadLevel(1, selectedStage);
    }
    public void Menu() { CancelInvoke(); Time.timeScale = 1; SceneManager.LoadScene("Menu"); }
    public void GameOver() { CancelInvoke(); Time.timeScale = 1; SceneManager.LoadScene("GameOver"); }
    public void LoadLevel(int world, int stage)
    {
        CancelInvoke();
        this.world = world; this.stage = stage;
        SceneManager.LoadScene(stage > 4 ? "Victory" : $"{world}-{stage}");
    }
    public void NextLevel() { LoadLevel(world, stage + 1); }
    public void CompleteLevel()
    {
        if (Transitioning) return;
        Transitioning = true;
        AddScore(Mathf.CeilToInt(timeRemaining) * 10);
        GameAudio.Instance.Play("clear");
        Invoke(nameof(NextLevel), 2.5f);
    }
    public void ResetLevel(float delay)
    {
        if (Transitioning) return;
        Transitioning = true;
        CancelInvoke(nameof(ResetLevel)); Invoke(nameof(ResetLevel), delay);
    }
    public void ResetLevel() { lives--; if (lives > 0) LoadLevel(world, stage); else GameOver(); }
    public void AddScore(int points) { score += points; }
    public void AddCoin()
    {
        coins++; AddScore(200); GameAudio.Instance.Play("coin");
        if (coins >= 100) { coins -= 100; AddLife(); }
    }
    public void AddLife() { lives++; GameAudio.Instance.Play("powerup"); }
}
