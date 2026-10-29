using UnityEngine;

public class BowserBoss : MonoBehaviour
{
    public bool defeated;
    public Sprite projectile;
    private Vector3 origin;
    private float nextShot;
    private void Start() { origin = transform.position; nextShot = Time.time + 2; }
    private void Update()
    {
        if (defeated)
        {
            transform.position += Vector3.down * Time.deltaTime * 5;
            transform.Rotate(0, 0, -Time.deltaTime * 90);
            return;
        }
        float t = Time.time;
        transform.position = origin + new Vector3(Mathf.Sin(t) * 2, Mathf.Max(0, Mathf.Sin(t * 1.8f)) * 1.5f);
        if (t < nextShot || Camera.main == null || Mathf.Abs(Camera.main.transform.position.x - transform.position.x) > 18) return;
        nextShot = t + 2.3f;
        var ball = new GameObject("Bowser fire", typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D), typeof(CastleHazard));
        ball.transform.position = transform.position + Vector3.left * 1.5f;
        ball.transform.localScale = new Vector3(.75f,.35f,1);
        ball.GetComponent<SpriteRenderer>().sprite = projectile;
        ball.GetComponent<CircleCollider2D>().isTrigger = true;
        ball.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        ball.GetComponent<CastleHazard>().fireball = true;
        GameAudio.Instance.Play("fire");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!defeated && other.CompareTag("Player")) other.GetComponent<Player>().Hit();
    }
}
