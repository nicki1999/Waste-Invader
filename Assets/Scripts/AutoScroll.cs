using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect;         // Reference to the ScrollRect component on "GameObject"
    public RectTransform content;         // Reference to "FactoidBG 1"
    
    private void Start()
    {
        if (!scrollRect)
        {
            Debug.LogError("ScrollRect is not assigned!");
        }

        if (!content)
        {
            Debug.LogError("Content RectTransform is not assigned!");
        }
    }

    void Update()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject != null && selectedObject.transform.IsChildOf(content))
        {
                        Debug.Log("Currently selected icon: " + selectedObject.name);
            RectTransform selectedRect = selectedObject.GetComponent<RectTransform>();
            RectTransform viewport = scrollRect.viewport;

            Vector3 viewportLocalPosition = viewport.InverseTransformPoint(content.position);
            Vector3 selectedLocalPosition = viewport.InverseTransformPoint(selectedRect.position);
            Debug.Log("Currently selected icon transform: " + viewportLocalPosition);

            float upperBound = viewportLocalPosition.y + viewport.rect.height / 2;
            float lowerBound = viewportLocalPosition.y - viewport.rect.height / 2;

            if (selectedLocalPosition.y > upperBound)
            {
                // Scroll down
                scrollRect.verticalNormalizedPosition -= 0.1f * Time.deltaTime;
            }
            else if (selectedLocalPosition.y < lowerBound)
            {
                // Scroll up
                scrollRect.verticalNormalizedPosition += 0.1f * Time.deltaTime;
            }
        }
    }
}
