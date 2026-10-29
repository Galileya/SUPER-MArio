using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CastleLevelBuilder
{
    [MenuItem("Mario/Build original Bowser castle")]
    public static void Build()
    {
        CreateBossSprite();
        CampaignBuilder.Begin(new Color(.055f,.035f,.075f), true, 110);
        Ground(0,18); Ground(28,44); Ground(60,73); Ground(88,96); Ground(109,123);
        var lava = CampaignBuilder.Tile("Lava", "HardBlock", new Vector2(60,0), new Vector2(130,1.5f), Color.red, false);
        lava.AddComponent<BoxCollider2D>().size = new Vector2(130,1.5f);
        lava.GetComponent<BoxCollider2D>().isTrigger = true; lava.AddComponent<CastleHazard>();
        for (int x = 0; x < 124; x += 8)
        {
            CampaignBuilder.Tile("Background pillar", "UndergroundBrick", new Vector2(x,7), new Vector2(2,10), new Color(.25f,.15f,.22f), false).GetComponent<SpriteRenderer>().sortingOrder = -5;
            CampaignBuilder.Tile("Vault", "UndergroundBrick", new Vector2(x+4,12), new Vector2(6,1), new Color(.4f,.25f,.3f));
        }
        CampaignBuilder.Platform(23,3.5f,new Vector2(0,1.8f),5);
        CampaignBuilder.Platform(48,4,new Vector2(0,2),5);
        CampaignBuilder.Platform(55,4,new Vector2(0,-2),5);
        CampaignBuilder.Platform(78,3.5f,new Vector2(1,1.5f),5);
        CampaignBuilder.Platform(84,4,new Vector2(0,2),5);
        for (int x = 10; x <= 90; x += 10)
        {
            for (int i=0;i<3;i++) CampaignBuilder.Spawn("Coin", x+i, 5);
        }
        CampaignBuilder.Spawn("MysteryBlock", 12, 5);
        CampaignBuilder.Spawn("MagicMushroom", 33, 3);
        CampaignBuilder.Spawn("Koopa", 37, 2);
        CampaignBuilder.Spawn("Goomba", 66, 2);
        CampaignBuilder.Tile("Bridge step", "HardBlock", new Vector2(94,2), new Vector2(3,1), new Color(.6f,.5f,.5f));
        var bridge = new GameObject("Collapsible bridge");
        for (int x = 96; x < 109; x++)
        {
            var plank = CampaignBuilder.Tile("Bridge plank", "Brick", new Vector2(x,2), new Vector2(1,.5f), new Color(.8f,.45f,.3f));
            plank.transform.SetParent(bridge.transform);
        }
        var bossObject = CampaignBuilder.Tile("Bowser", "Bowser", new Vector2(103,3.5f), new Vector2(2,2), Color.white, false);
        bossObject.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Simple;
        bossObject.transform.localScale = new Vector3(1.6f,1.6f,1);
        bossObject.AddComponent<BoxCollider2D>().isTrigger = true;
        bossObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        var boss = bossObject.AddComponent<BowserBoss>();
        boss.projectile = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/HardBlock.png");
        var axe = CampaignBuilder.Tile("Bridge switch - axe", "BowserAxe", new Vector2(110,3), Vector2.one, Color.white, false);
        axe.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Simple;
        axe.AddComponent<BoxCollider2D>().isTrigger = true;
        var trigger = axe.AddComponent<BridgeSwitch>(); trigger.bridge = bridge; trigger.boss = boss;
        BuildOriginalFortress();
        CampaignBuilder.Save("1-4"); CampaignBuilder.UpdateBuildSettings(); AssetDatabase.SaveAssets();
    }
    private static void BuildOriginalFortress()
    {
        var fortress = new GameObject("Original fortress - twin forge towers");
        var stone = new Color(.45f,.35f,.55f);
        var accent = new Color(1,.55f,.16f);
        foreach (float x in new[] { 115f,122f })
        {
            var tower = CampaignBuilder.Tile("Custom stone tower", "HardBlock", new Vector2(x,4.5f),new Vector2(2,6),stone);
            tower.transform.SetParent(fortress.transform);
            foreach (float offset in new[] { -.65f,.65f })
                CampaignBuilder.Tile("Battlement", "Brick", new Vector2(x+offset,8),new Vector2(.7f,1.5f),stone).transform.SetParent(fortress.transform);
            CampaignBuilder.Tile("Tower window", "HardBlock", new Vector2(x,5.8f),new Vector2(.55f,1.1f),accent,false).transform.SetParent(fortress.transform);
        }
        var wall = CampaignBuilder.Tile("Fortress rear wall", "UndergroundBrick",new Vector2(118.5f,4),new Vector2(5,5),new Color(.2f,.17f,.3f),false);
        wall.transform.SetParent(fortress.transform); wall.GetComponent<SpriteRenderer>().sortingOrder=-3;
        foreach(float x in new[] { 117f,120f })
            CampaignBuilder.Tile("Gate jamb", "HardBlock",new Vector2(x,3.5f),new Vector2(.5f,4),accent).transform.SetParent(fortress.transform);
        CampaignBuilder.Tile("Gate lintel", "HardBlock",new Vector2(118.5f,5.7f),new Vector2(3.5f,.6f),accent).transform.SetParent(fortress.transform);
        var door=CampaignBuilder.Tile("Recessed gate", "HardBlock",new Vector2(118.5f,3.3f),new Vector2(2.4f,3.6f),new Color(.055f,.04f,.1f),false);
        door.transform.SetParent(fortress.transform); door.GetComponent<SpriteRenderer>().sortingOrder=-2;
    }
    private static void Ground(int first,int last)
    {
        CampaignBuilder.Tile("Stone foundation", "HardBlock", new Vector2((first+last)/2f,1), new Vector2(last-first+1,1), new Color(.6f,.55f,.65f));
    }
    private static void CreateBossSprite()
    {
        // Editable pixel patterns keep new artwork reproducible inside the project.
        string[] rows = {
            "........................", "......W.................", ".....WGG...W............", "....RGGGG.WGW...........",
            "...RRRGGGGGGGG..........", "..RRRGGGGGGWWGG.........", "...RRGGGGGGBWGGGG.......", "....GGGGGGGGGGGGGG......",
            "...GGGGGGGYYYYYYY.......", "..WGGGGGGYYWYWWW........", ".WGGGGGGGYYYYYYY........", "..GGGGGGGYYYYY..........",
            ".WGGGGGGGGYYYYGG........", "..GGGGGGGGYYYYYGG.......", ".WGGGGGGGGYYYYYGGW......", "..GGGGGGGGYYYYYWWW......",
            "...GGGGGGGYYYYY.........", "..GGGGGGGGYYYYYY........", "..GGGGGGGYYYYYYY........", "...GGGGGYYYYYYGG........",
            "....GGGGGYYYGGGG........", "...GGGGG....GGGGG.......", "..WWWWW....WWWWWWW......", "........................"
        };
        var palette = new Dictionary<char,Color> { ['.']=Color.clear, ['G']=new Color(.1f,.6f,.13f), ['Y']=new Color(1,.7f,.26f), ['R']=new Color(.9f,.16f,.02f), ['W']=new Color(1,.94f,.8f), ['B']=Color.black };
        Pattern("Bowser",rows,palette);
        Pattern("BowserAxe", new[] { "......WWWW......", ".....WWWWW......", "....WWWWWW......", ".....WWWWW......", "......WWWW......", "......YY........", "......YY........", "......YY........", "......YY........", "......YY........", "......YY........", "......YY........", "................", "................", "................", "................" },palette);
    }
    private static void Pattern(string name,string[] rows,Dictionary<char,Color> palette)
    {
        var texture=new Texture2D(rows[0].Length,rows.Length,TextureFormat.RGBA32,false);
        for(int y=0;y<rows.Length;y++) for(int x=0;x<rows[y].Length;x++) texture.SetPixel(x,rows.Length-y-1,palette[rows[y][x]]);
        texture.Apply(); File.WriteAllBytes("Assets/Sprites/"+name+".png",texture.EncodeToPNG());
        Object.DestroyImmediate(texture); WaterLevelBuilder.ImportSprite(name);
    }
}
