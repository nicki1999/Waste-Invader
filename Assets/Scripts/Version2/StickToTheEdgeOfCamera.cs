using UnityEngine;

public class StickToLeftOfCamera : MonoBehaviour
{
    public Camera mainCamera; // Assign your main camera in the Inspector
    public float xOffset = -10f; // Adjust how far from the left it should be
    public float widthMultiplier = 100f;
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main; // Get the main camera if not assigned

        ResizeCollider();
    }
    void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main; // Get the main camera if not assigned

        // Get the left edge of the camera in world coordinates
        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));

        // Set position to be at the left edge with an offset
        transform.position = new Vector3(leftEdge.x + xOffset, transform.position.y, 0);
        ResizeCollider();
    }
    void ResizeCollider()
    {
        if (mainCamera == null) return;

        // Get the world space width of the camera
        float screenWidth = mainCamera.orthographicSize * mainCamera.aspect * 2;

        // Set the GameObject’s scale X based on screen width
        transform.localScale = new Vector3(screenWidth * widthMultiplier, transform.localScale.y, transform.localScale.z);
    }
}
