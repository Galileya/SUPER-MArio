using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Generates editable scenes with prefab instances. Not a runtime level generator.
public static class CampaignBuilder
{
    public static GameObject Spawn(string prefab, float x, float y)
    {
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/" + prefab + ".prefab");
        var obj = (GameObject)PrefabUtility.InstantiatePrefab(asset);
        obj.transform.position = new Vector3(x, y, 0);
        return obj;
    }
    public static GameObject Tile(string name, string sprite, Vector2 position, Vector2 size, Color tint, bool solid = true)
    {
        var obj = new GameObject(name);
        obj.transform.position = position;
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/" + sprite + ".png");
        renderer.drawMode = SpriteDrawMode.Tiled;
        renderer.size = size; renderer.color = tint;
        if (solid) obj.AddComponent<BoxCollider2D>().size = size;
        return obj;
    }
    public static void Begin(Color background, bool player = true, float maxX = 180)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        camera.tag = "MainCamera";
        camera.transform.position = new Vector3(12, 6.5f, -10);
        var cam = camera.GetComponent<Camera>();
        cam.orthographic = true; cam.orthographicSize = 7.5f;
        cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = background;
        if (!player) return;
        Spawn("Mario", 3, 2.1f);
        var scrolling = camera.AddComponent<SideScrolling>();
        scrolling.bounded = true; scrolling.minX = 12; scrolling.maxX = maxX;
        var barrier = new GameObject("Death Barrier", typeof(BoxCollider2D), typeof(DeathBarrier));
        barrier.transform.position = new Vector3(100, -4);
        var box = barrier.GetComponent<BoxCollider2D>(); box.size = new Vector2(260, 2); box.isTrigger = true;
    }
    public static void Save(string name)
    {
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Scenes/" + name + ".unity");
    }
    public static void Exit(float x, float y)
    {
        var exit = new GameObject("Exit to next world", typeof(BoxCollider2D), typeof(LevelExit));
        exit.transform.position = new Vector3(x, y);
        var box = exit.GetComponent<BoxCollider2D>(); box.isTrigger = true; box.size = new Vector2(1.2f, 4);
    }
    public static void Platform(float x, float y, Vector2 travel, float period = 5)
    {
        var obj = Tile("Moving lift", "HardBlock", new Vector2(x, y), new Vector2(3, .5f), new Color(1, .65f, .35f));
        obj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        var lift = obj.AddComponent<MovingPlatform>(); lift.travel = travel; lift.period = period;
    }
    [MenuItem("Mario/Build menu and underground")]
    public static void BuildUnderground()
    {
        foreach (string name in new[] { "Menu", "GameOver", "Victory" })
        {
            Begin(name == "Menu" ? new Color(.10f, .19f, .34f) : new Color(.055f, .06f, .11f), false);
            Tile("Floor", "GroundBlock", new Vector2(12, -.25f), new Vector2(40, 2), Color.white);
            Spawn("Castle", 22, 1.5f); Spawn("Bush2", 3, 1.5f); Spawn("Cloud1", 5, 11);
            Save(name);
        }
        Begin(Color.black, true, 180);
        string[] rows = File.ReadAllLines("Assets/Levels/1-2.txt");
        for (int row = 0; row < rows.Length; row++)
        for (int x = 0; x < rows[row].Length; x++)
        {
            float y = 12 - row;
            switch (rows[row][x])
            {
                case 'B': Spawn("UndergroundBrick", x, y); break;
                case 'G': Spawn("UndergroundBlock", x, y); break;
                case 'H': Spawn("HardBlock", x, y).GetComponent<SpriteRenderer>().color = Color.cyan; break;
                case '?': Spawn("MysteryBlock", x, y); break;
                case 'C': Spawn("Coin", x, y); break;
                case 'P': Tile("Pipe wall", "PipeBottom", new Vector2(x, y), Vector2.one, Color.white); break;
            }
        }
        // Original 1-2 enemy groups and two lifts across the late pits.
        foreach (int x in new[] { 16, 17, 29, 61, 62, 64, 99, 100, 102, 112, 136 }) Spawn("Goomba", x, 2);
        foreach (int x in new[] { 44, 46, 59, 145 }) Spawn("Koopa", x, 2);
        Platform(140, 5, new Vector2(0, 4)); Platform(155, 5, new Vector2(0, -4));
        Exit(166, 7);
        // Warp pipes remain a scenic nod; all three continue the required campaign.
        foreach (int x in new[] { 179, 183, 187 }) Exit(x, 3);
        Save("1-2");
        UpdateBuildSettings();
        AssetDatabase.SaveAssets();
    }
    public static void UpdateBuildSettings()
    {
        EditorBuildSettings.scenes = new[] { "Menu", "1-1", "1-2", "1-3", "1-4", "GameOver", "Victory" }
            .Where(n => File.Exists("Assets/Scenes/" + n + ".unity"))
            .Select(n => new EditorBuildSettingsScene("Assets/Scenes/" + n + ".unity", true)).ToArray();
    }
}
