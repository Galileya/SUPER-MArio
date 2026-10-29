using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CampaignValidation
{
    public static void RebuildCastleAndWindows()
    {
        CastleLevelBuilder.Build(); BuildWindows();
    }
    public static void RepairAndBuildWindows()
    {
        CampaignBuilder.RepairUndergroundFloor(); BuildWindows();
    }
    [MenuItem("Mario/Rebuild complete campaign")]
    public static void BuildAll()
    {
        CampaignBuilder.BuildUnderground(); WaterLevelBuilder.Build(); CastleLevelBuilder.Build();
        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
        Debug.Log("CAMPAIGN_BUILD_OK: seven scenes generated");
    }
    public static void BuildWindows()
    {
        if (!File.Exists("Assets/Scenes/1-4.unity")) BuildAll();
        string[] scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray();
        Require(scenes.Length == 7, "Seven enabled campaign scenes");
        foreach (string path in scenes)
        {
            var scene = EditorSceneManager.OpenScene(path);
            Require(UnityEngine.Object.FindObjectOfType<Camera>() != null, path + " camera");
            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                Require(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject) == 0, path + " missing script on " + transform.name);
                if (path.EndsWith("1-4.unity"))
                    Require(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(transform.gameObject) != "Assets/Prefabs/Castle.prefab", "Original castle must not use the Castle prefab");
            }
            if (!Path.GetFileName(path).StartsWith("1-")) continue;
            Require(UnityEngine.Object.FindObjectsOfType<Player>().Length == 1, path + " one player");
            Require(UnityEngine.Object.FindObjectOfType<SideScrolling>() != null, path + " scrolling camera");
        }
        foreach (string audio in new[] { "overworld", "underground", "water", "castle", "jump", "stomp", "powerup", "damage", "death", "coin", "clear", "gameover" })
            Require(Resources.Load<AudioClip>("Audio/" + audio) != null, "Audio " + audio);
        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
        Directory.CreateDirectory("Builds/Windows");
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = scenes, locationPathName = "Builds/Windows/Mario.exe",
            target = BuildTarget.StandaloneWindows64, options = BuildOptions.Development
        });
        Require(report.summary.result == BuildResult.Succeeded, "Windows build: " + report.summary.result);
        Debug.Log("CAMPAIGN_VALIDATION_OK: scene references, audio and Windows build passed");
    }
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception("VALIDATION FAILED: " + message);
    }
}
