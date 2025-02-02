using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardScript : MonoBehaviour
{

    public Text displayText;
    public string storedText = "";
    public static event Action OnEnterPressed;


    public void KeyInput(string Key)
    {
        if (storedText.Length < 3)
            storedText += Key;

        displayText.text = storedText;
    }

    public void DeleteKey()
    {
        if (storedText.Length > 0)
            storedText = storedText.Substring(0, storedText.Length - 1);

        displayText.text = storedText;
    }
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Return)) // Detect Enter key
    //     {
    //         OnEnterPressed?.Invoke(); // Fire the event
    //     }
    // }
}
