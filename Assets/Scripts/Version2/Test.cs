

// // sprites that don't exist: meat, grains, coffee grinds = CoffeeGrounds, clamshell fruit containers, Soap bottles, plastic Pipes, package protection such as on paper towels(doesn't exist), medicine bottles, iced coffee cups, cream cheese container
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour
{
    public Text buttonTextTitle; // Title 
    public GameObject buttonTextDescription;
    public Button firstSelectedButton;


    void Start()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();

        if (eventSystem != null)
        {
            eventSystem.firstSelectedGameObject = firstSelectedButton.gameObject;
            eventSystem.SetSelectedGameObject(firstSelectedButton.gameObject);
        }
        else
        {
            Debug.LogError("EventSystem not found in the scene.");
        }
    }
    
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            GameObject currentSelectedButton = EventSystem.current.currentSelectedGameObject;
            string buttonName = currentSelectedButton.name;
            Transform buttonParent = currentSelectedButton.transform.parent;
            showDesctiption(buttonParent.name);
            buttonTextTitle.text =  buttonName;
        }
        else
        {
            buttonTextTitle.text = "";
        }
    }

    private void showDesctiption(string selectedButtonType)
    {
        Transform textDescription = buttonTextDescription.transform;

        // Iterate through each child of the parent GameObject
        for (int i = 0; i < textDescription.childCount; i++)
        {
            // Get the child GameObject at index i
            GameObject childObject = textDescription.GetChild(i).gameObject;
            
            if(childObject.name == selectedButtonType){
                childObject.SetActive(true);
            }
            else{
            // Deactivate the child GameObject
            childObject.SetActive(false);
            }

        }
    }
    }

