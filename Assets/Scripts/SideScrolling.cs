using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrolling : MonoBehaviour
{
    private new Camera camera;
    private Transform player;

    public float height = 6.5f;
    public float undergroundHeight = -9.5f;
    public float undergroundThreshold = 0f;
    public bool bounded;
    public float minX = 12, maxX = 180;
    public float smoothTime = .12f;
    private float scrollVelocity;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        player = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        // track the player moving to the right
        Vector3 cameraPosition = transform.position;
        float target = Mathf.Max(cameraPosition.x, player.position.x + (bounded ? 2f : 0f));
        if (bounded) target = Mathf.Clamp(target, minX, maxX);
        cameraPosition.x = Mathf.SmoothDamp(cameraPosition.x, target, ref scrollVelocity, smoothTime);
        transform.position = cameraPosition;
    }

    public void SetUnderground(bool underground)
    {
        // set underground height offset
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
    }

}
