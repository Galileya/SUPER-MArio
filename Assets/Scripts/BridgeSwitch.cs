using System.Collections;
using UnityEngine;

public class BridgeSwitch : MonoBehaviour
{
    public GameObject bridge;
    public BowserBoss boss;
    private bool activated;
    private Vector3 origin;
    private void Start() { origin = transform.position; }
    private void Update() { if (!activated) transform.position = origin + Vector3.up * Mathf.Sin(Time.time * 5) * .12f; }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated || !other.CompareTag("Player")) return;
        activated = true;
        GameAudio.Instance.Play("switch");
        GameManager.Instance.AddScore(5000);
        boss.defeated = true;
        boss.GetComponent<Collider2D>().enabled = false;
        other.GetComponent<PlayerMovement>().enabled = false;
        StartCoroutine(Collapse());
    }
    private IEnumerator Collapse()
    {
        foreach (Transform plank in bridge.transform)
        {
            plank.gameObject.SetActive(false);
            yield return new WaitForSeconds(.08f);
        }
        yield return new WaitForSeconds(.5f);
        GameManager.Instance.CompleteLevel();
    }
}
