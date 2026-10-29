using UnityEngine;

public class WaterEnemy : MonoBehaviour
{
    public bool blooper;
    public float range = 3;
    public float speed = 1.2f;
    public float phase;
    private Vector3 origin;
    private SpriteRenderer visual;
    private void Start() { origin = transform.position; visual = GetComponent<SpriteRenderer>(); }
    private void FixedUpdate()
    {
        float t = Time.time * speed + phase;
        Vector3 position = origin + new Vector3(Mathf.Sin(t) * range, Mathf.Sin(t * (blooper ? 2 : .7f)) * (blooper ? 2 : .6f));
        GetComponent<Rigidbody2D>().MovePosition(position);
        visual.flipX = Mathf.Cos(t) > 0;
        transform.localScale = new Vector3(1, 1 + .08f * Mathf.Sin(t * 8), 1);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var player = other.GetComponent<Player>();
        if (player.starpower)
        {
            GameManager.Instance.AddScore(200); GameAudio.Instance.Play("stomp"); Destroy(gameObject);
        }
        else player.Hit();
    }
}
