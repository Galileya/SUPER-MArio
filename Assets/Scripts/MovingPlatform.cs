using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector2 travel = new Vector2(0, 4);
    public float period = 4;
    private Vector3 origin;
    private Vector3 previous;
    private Transform rider;
    private void Start() { origin = previous = transform.position; }
    private void FixedUpdate()
    {
        Vector3 next = origin + (Vector3)travel * Mathf.Sin(Time.time * 2 * Mathf.PI / period);
        Vector3 delta = next - previous;
        GetComponent<Rigidbody2D>().MovePosition(next);
        if (rider != null) rider.position += delta;
        previous = next;
    }
    private void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.collider.CompareTag("Player") && hit.transform.position.y > transform.position.y + .3f) rider = hit.transform;
    }
    private void OnCollisionExit2D(Collision2D hit) { if (hit.transform == rider) rider = null; }
}
