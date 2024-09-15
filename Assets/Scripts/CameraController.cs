
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [Header("Components")]
    public GameObject player;

    [Header("Camera Movement Settings")]
    public float timeOffset = 0.2f;
    public Vector3 positionOffset = new Vector3(-0.5f, 3, -10);

    // Private variables.
    private Vector3 velocity;

    private void Awake()
    {
        // Singleton pattern - Ensure only one instance of CameraController exists.
        instance = this;
    }

    private void LateUpdate()
    {
        // Smoothly move the camera to the player's position with an offset.
        transform.position = Vector3.SmoothDamp(transform.position, player.transform.position + positionOffset, ref velocity, timeOffset);
    }
}
