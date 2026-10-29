using UnityEngine;

public class CastleHazard : MonoBehaviour
{
    public bool fireball;
    public float speed = -5;
    private Vector3 origin;
    private SpriteRenderer visual;
    private void Start() { origin = transform.position; visual = GetComponent<SpriteRenderer>(); }
    private void Update()
    {
        if (fireball)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
            if (Mathf.Abs(transform.position.x - origin.x) > 25) Destroy(gameObject);
        }
        visual.color = Color.Lerp(new Color(1,.2f,.02f), new Color(1,.8f,.05f), (Mathf.Sin(Time.time * 9) + 1) / 2);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (fireball) other.GetComponent<Player>().Hit();
        else other.GetComponent<Player>().Death();
    }
}
