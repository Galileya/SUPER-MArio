using System.IO;
using UnityEditor;
using UnityEngine;

public static class WaterLevelBuilder
{
    public static void ImportSprite(string name)
    {
        string path = "Assets/Sprites/" + name + ".png";
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 16; importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true; importer.SaveAndReimport();
    }
    [MenuItem("Mario/Build aquatic level")]
    public static void Build()
    {
        foreach (string name in new[] { "WaterGround", "Coral", "Blooper", "CheepCheep" }) ImportSprite(name);
        CampaignBuilder.Begin(new Color(32/255f, 56/255f, 236/255f), true, 180);
        Object.FindObjectOfType<PlayerMovement>().swimming = true;
        Object.FindObjectOfType<PlayerMovement>().moveSpeed = 5;
        var rows = File.ReadAllLines("Assets/Levels/2-2.txt");
        for (int row = 0; row < rows.Length; row++)
        for (int x = 0; x < rows[row].Length; x++)
        {
            var pos = new Vector2(x, 12 - row);
            switch (rows[row][x])
            {
                case 'G': CampaignBuilder.Tile("Sea stone", "WaterGround", pos, Vector2.one, Color.white); break;
                case 'R': CampaignBuilder.Tile("Coral", "Coral", pos, Vector2.one, Color.white, false); break;
                case 'C': CampaignBuilder.Spawn("Coin", x, pos.y); break;
                case 'P': CampaignBuilder.Tile("Exit pipe", "PipeBottom", pos, Vector2.one, Color.white); break;
            }
        }
        CampaignBuilder.Tile("Water surface", "HardBlock", new Vector2(96,12.5f), new Vector2(194,.2f), new Color(.6f,.85f,1), false);
        // Transparent ceiling prevents swimming over the whole course.
        var ceiling = new GameObject("Water ceiling", typeof(BoxCollider2D));
        ceiling.transform.position = new Vector3(96, 13); ceiling.GetComponent<BoxCollider2D>().size = new Vector2(194,1);
        for (int i = 0; i < 7; i++) Creature(true, 22 + i * 27, 4 + i % 4, i);
        for (int i = 0; i < 12; i++) Creature(false, 75 + i * 9, 3 + i % 8, i);
        new GameObject("Bubbles", typeof(Bubbles));
        CampaignBuilder.Exit(187, 5);
        CampaignBuilder.Save("1-3"); CampaignBuilder.UpdateBuildSettings(); AssetDatabase.SaveAssets();
    }
    private static void Creature(bool blooper, float x, float y, int phase)
    {
        var obj = CampaignBuilder.Tile(blooper ? "Blooper" : "Cheep Cheep", blooper ? "Blooper" : "CheepCheep", new Vector2(x,y), new Vector2(1,blooper ? 1.5f : 1), Color.white, false);
        obj.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Simple;
        obj.layer = LayerMask.NameToLayer("Enemy");
        obj.AddComponent<BoxCollider2D>().isTrigger = true;
        obj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        var enemy = obj.AddComponent<WaterEnemy>(); enemy.blooper = blooper; enemy.phase = phase;
    }
}
