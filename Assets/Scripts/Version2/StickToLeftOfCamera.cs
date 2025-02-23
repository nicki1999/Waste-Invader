using UnityEngine;

public class StickToTheEdgeOfCamera : MonoBehaviour
{
    public Camera mainCamera; // Assign your main camera in the Inspector
    public float xOffset = 0f; // Adjust how far from the left it should be
    public string container; // "left" or "right" for positioning

    private float targetScaleX; // Target X scale based on aspect ratio
    private float biggerTolerance = 0.1f; // Tolerance for bigger aspect ratios
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main; // Get the main camera if not assigned

        AdjustScale();
    }

    void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main; // Get the main camera if not assigned

        AdjustPosition();
        AdjustScale();
    }

    void AdjustPosition()
    {
        if (container == "left")
        {
            Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
            transform.position = new Vector3(leftEdge.x + xOffset, transform.position.y, 0);
        }
        else if (container == "right")
        {
            Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));
            transform.position = new Vector3(rightEdge.x + xOffset, transform.position.y, 0);
        }
    }

    void AdjustScale()
    {
        if (mainCamera == null) return;

        // Get current aspect ratio
        float currentAspect = (float)Screen.width / Screen.height;
        if (Mathf.Abs(currentAspect - (18f / 9f)) < biggerTolerance || Mathf.Abs(currentAspect - (19.5f / 9f)) < biggerTolerance)
        {
            targetScaleX = 12f; // 16:10 Aspect Ratio
        }
        // Define known aspect ratios and corresponding scale values
        else if (Mathf.Approximately(currentAspect, 16f / 9f))
            targetScaleX = 9.5f; // 16:9 Aspect Ratio
        else if (Mathf.Approximately(currentAspect, 16f / 10f))
            targetScaleX = 8.5f; // 16:10 Aspect Ratio

        else if (Mathf.Approximately(currentAspect, 4f / 3f))
            targetScaleX = 7.5f; // 4:3 Aspect Ratio
        else
            targetScaleX = Mathf.Lerp(8f, 10f, (currentAspect - (4f / 3f)) / ((16f / 9f) - (4f / 3f))); // Interpolated for unknown ratios

        // Apply the target X scale
        transform.localScale = new Vector3(targetScaleX, transform.localScale.y, transform.localScale.z);
    }
}
