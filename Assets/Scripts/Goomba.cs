using UnityEngine;

public class Goomba : MonoBehaviour
{
    public Sprite flatSprite;
    private bool defeated;
    private bool Reward()
    {
        if (defeated) return false;
        defeated = true;
        GameManager.Instance.AddScore(100); GameAudio.Instance.Play("stomp");
        return true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player.starpower) {
                Hit();
            } else if (collision.transform.DotTest(transform, Vector2.down)) {
                Flatten();
            } else {
                player.Hit();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shell")) {
            Hit();
        }
    }

    private void Flatten()
    {
        if (!Reward()) return;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EntityMovement>().enabled = false;
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = flatSprite;
        Destroy(gameObject, 0.5f);
    }

    private void Hit()
    {
        if (!Reward()) return;
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<DeathAnimation>().enabled = true;
        Destroy(gameObject, 3f);
    }

}
