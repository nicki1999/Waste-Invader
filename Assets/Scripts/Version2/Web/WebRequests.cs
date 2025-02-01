using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Unity.VisualScripting;
using System;

public class WebRequests : MonoBehaviour
{
    void Start()
    {
        // StartCoroutine(GetRequest("http://172.30.81.176/SpaceInvadersBackend/index.php"));
        // StartCoroutine(GetLeaderboard("http://172.30.81.176/SpaceInvadersBackend/GetLeaderboard.php"));
        //StartCoroutine(AddToLeaderboard("http://172.30.81.176/SpaceInvadersBackend/AddToLeaderboard.php", "test1", 1, 1000, 40));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    IEnumerator GetLeaderboard(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public IEnumerator AddToLeaderboard(string playerName, int playerWave, int playerScore)
    {
        Debug.Log($"AddToLeaderboard started: {playerName}, {playerWave}, {playerScore}");

        string uri = "http://172.30.8.134//SpaceInvadersBackend/AddToLeaderboard.php";
        WWWForm form = new WWWForm();
        form.AddField("playerName", playerName);
        form.AddField("playerWave", playerWave);
        form.AddField("playerScore", playerScore);
        Debug.Log($"Sending Data -> Name: {playerName}, Wave: {playerWave}, Score: {playerScore}");


        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
                Debug.LogError($"Response Code: {webRequest.responseCode}");
                Debug.LogError($"Response Text: {webRequest.downloadHandler.text}");
            }
            else
            {
                Debug.Log("Server Response: " + webRequest.downloadHandler.text);
            }
        }
    }

}