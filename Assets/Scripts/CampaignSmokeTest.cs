#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// Opt-in integration checks for the development player. Normal play is unaffected.
public class CampaignSmokeTest : MonoBehaviour
{
    private string output;
    private int assertions;
    private bool failed;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        if (Array.IndexOf(Environment.GetCommandLineArgs(), "-campaignSmokeTest") < 0) return;
        var obj = new GameObject("Campaign integration checks"); DontDestroyOnLoad(obj);
        obj.AddComponent<CampaignSmokeTest>();
    }
    private void Start()
    {
        output = Path.Combine(Application.dataPath, "../../..", "docs", "capturas");
        Directory.CreateDirectory(output);
        Screen.SetResolution(1280,720,false);
        Application.logMessageReceived += OnLog;
        StartCoroutine(Run());
    }
    private void OnLog(string message, string stack, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) failed = true;
    }
    private void Check(bool condition, string label)
    {
        assertions++;
        if (!condition) { failed = true; Debug.LogError("FAIL: " + label); }
        else Debug.Log("PASS: " + label);
    }
    private IEnumerator Capture(string name)
    {
        yield return new WaitForSeconds(.25f);
        yield return new WaitForEndOfFrame();
        // Render explicitly so hidden-window execution still produces real scene images.
        var camera = Camera.main;
        var target = RenderTexture.GetTemporary(1280,720,24);
        var previous = camera.targetTexture; var active = RenderTexture.active;
        camera.targetTexture = target; camera.Render(); RenderTexture.active = target;
        var capture = new Texture2D(1280,720,TextureFormat.RGB24,false);
        capture.ReadPixels(new Rect(0,0,1280,720),0,0); capture.Apply();
        File.WriteAllBytes(Path.Combine(output,name + ".png"),capture.EncodeToPNG());
        camera.targetTexture = previous; RenderTexture.active = active;
        RenderTexture.ReleaseTemporary(target); Destroy(capture);
        yield return new WaitForSeconds(.3f);
    }
    private IEnumerator Run()
    {
        yield return Capture("01-menu");
        var game = GameManager.Instance;
        game.NewGame(); yield return null; yield return new WaitForSeconds(.3f);
        Check(game.lives == 3 && game.score == 0 && game.stage == 1, "new game resets session");
        yield return Capture("02-superficie");
        float timer = game.timeRemaining;
        game.TogglePause(); yield return new WaitForSecondsRealtime(.2f);
        Check(Mathf.Approximately(timer,game.timeRemaining),"pause freezes timer"); game.TogglePause();
        for (int i=0;i<100;i++) game.AddCoin();
        Check(game.lives == 4 && game.coins == 0 && game.score == 20000,"100 coins grant exactly one life and 20000 points");
        game.AddCoin(); Check(game.coins == 1 && game.score == 20200,"coin score and counter");
        game.LoadLevel(1,2); yield return null;
        Check(game.lives == 4 && game.coins == 1 && game.score == 20200,"session persists between levels");
        var player = FindObjectOfType<Player>();
        player.transform.position = new Vector3(34,3); Camera.main.transform.position = new Vector3(34,6.5f,-10);
        yield return Capture("03-subterraneo");
        // Exercise the real exit collider rather than calling NextLevel directly.
        player.transform.position = new Vector3(166,6); Camera.main.transform.position = new Vector3(166,6.5f,-10);
        yield return new WaitForSeconds(3);
        Check(SceneManager.GetActiveScene().name == "1-3", "underground exit advances to aquatic world");
        if (SceneManager.GetActiveScene().name != "1-3") { game.LoadLevel(1,3); yield return null; }
        player = FindObjectOfType<Player>();
        Check(player.GetComponent<PlayerMovement>().swimming,"aquatic swimming enabled");
        player.transform.position = new Vector3(74,8); Camera.main.transform.position = new Vector3(74,6.5f,-10);
        yield return Capture("04-acuatico");
        player.transform.position = new Vector3(187,6); Camera.main.transform.position = new Vector3(180,6.5f,-10);
        yield return new WaitForSeconds(3);
        Check(SceneManager.GetActiveScene().name == "1-4", "aquatic exit advances to original castle");
        if (SceneManager.GetActiveScene().name != "1-4") { game.LoadLevel(1,4); yield return null; }
        player = FindObjectOfType<Player>();
        player.transform.position = new Vector3(94,3.2f); Camera.main.transform.position = new Vector3(101,6.5f,-10);
        yield return Capture("05-castillo");
        player.transform.position = new Vector3(110,3); Camera.main.transform.position = new Vector3(110,6.5f,-10);
        yield return new WaitForSeconds(5);
        Check(SceneManager.GetActiveScene().name == "Victory", "axe collapses bridge and ends campaign");
        yield return Capture("06-victoria");
        game.NewGame(); yield return null;
        player = FindObjectOfType<Player>();
        player.Grow(); yield return new WaitForSeconds(.6f);
        Check(player.big,"mushroom enables big renderer");
        player.Hit(); player.Hit(); yield return new WaitForSeconds(.6f);
        Check(!player.big && !player.dead,"damage shrinks and grants temporary immunity");
        player.Death(); player.Death(); yield return new WaitForSeconds(3.2f);
        Check(game.lives == 2,"repeated death callback only consumes one life");
        FindObjectOfType<Player>().Death(); yield return new WaitForSeconds(3.2f);
        FindObjectOfType<Player>().Death(); yield return new WaitForSeconds(3.2f);
        Check(SceneManager.GetActiveScene().name == "GameOver" && game.lives == 0,"last life opens Game Over");
        yield return Capture("07-game-over");
        game.NewGame(); yield return null;
        Check(game.lives==3 && game.score==0 && game.coins==0,"retry resets all counters");
        Directory.CreateDirectory("TestResults");
        File.WriteAllText("TestResults/campaign-smoke.txt", $"{(failed ? "FAIL" : "PASS")} - {assertions} assertions\n");
        Debug.Log($"CAMPAIGN_SMOKE_{(failed ? "FAIL" : "OK")}: {assertions} assertions");
        Application.Quit(failed ? 1 : 0);
    }
}
#endif
