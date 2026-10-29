using UnityEngine;

// Lightweight looping bubbles, generated once and animated without allocations.
public class Bubbles : MonoBehaviour
{
    private Transform[] bubbles;
    private void Start()
    {
        bubbles = new Transform[40];
        var texture = new Texture2D(8, 8);
        texture.filterMode = FilterMode.Point;
        for (int y = 0; y < 8; y++) for (int x = 0; x < 8; x++)
        {
            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(3.5f, 3.5f));
            texture.SetPixel(x, y, distance > 2 && distance < 3.5f ? new Color(.7f, .9f, 1, .5f) : Color.clear);
        }
        texture.Apply();
        var sprite = Sprite.Create(texture, new Rect(0, 0, 8, 8), new Vector2(.5f,.5f), 32);
        for (int i = 0; i < bubbles.Length; i++)
        {
            var obj = new GameObject("Bubble"); obj.transform.SetParent(transform);
            obj.AddComponent<SpriteRenderer>().sprite = sprite;
            obj.transform.position = new Vector3(4 + i * 4.5f, i % 10 + 1);
            bubbles[i] = obj.transform;
        }
    }
    private void Update()
    {
        foreach (var bubble in bubbles)
        {
            bubble.position += Vector3.up * Time.deltaTime * .8f;
            if (bubble.position.y > 12) bubble.position = new Vector3(bubble.position.x, 1);
        }
    }
}
