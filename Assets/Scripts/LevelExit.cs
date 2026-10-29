using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private bool entered;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (entered || !other.CompareTag("Player")) return;
        entered = true;
        other.GetComponent<PlayerMovement>().enabled = false;
        GameManager.Instance.CompleteLevel();
    }
}
