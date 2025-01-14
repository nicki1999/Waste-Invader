using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage2Tutorial : MonoBehaviour
{
    public GameObject TutorialText;
    public GameObject NormalEnemies;
    public GameObject TutorialPrompt;
    public GameObject TutorialContinuePrompt;
    public GameManager GM;

    public Text TextObject;
    public string[] PrintString;
    private string CurrentString;
    public float CharWaitTime;
    public float DisableTimePerCharacter;

    public bool ProceedTutorial = false;
    public int TutorialStage = 0;

    private bool GraceChecker;
    private bool GracePeriod;

    // Start is called before the first frame update
    void OnEnable()
    {
        TutorialText.SetActive(true);
        NormalEnemies.SetActive(true);
        StartCoroutine(PrintText());
        TutorialPrompt.SetActive(false);
        TutorialContinuePrompt.SetActive(false);
        GraceChecker = false;
        GracePeriod = false;
        TutorialStage = 0;
    }
    public void ShowTutorialPromptStage2(){
                            GraceChecker = false;
                    GracePeriod = false;
                    TutorialText.SetActive(false);
                    TutorialPrompt.SetActive(false);
                    TutorialContinuePrompt.SetActive(false);
                    GM.WatchedStage2Tutorial = true;
                    GM.TutorialsWatched++;
                    PlayerPrefs.SetInt("TutorialsWatched", GM.TutorialsWatched);
                    GM.StatTrackerStats[5].text = GM.TutorialsWatched.ToString();

                    GM.MenusToggleOn(GM.Stage2UI);
                    GM.MenusToggleOff(GM.Stage2TutorialObjects);
    }
    public void SkipTutorialPromptStage2(){
                            GraceChecker = false;
                    GracePeriod = false;
                    TutorialStage = 0;
                    TutorialPrompt.SetActive(false);
                    StartCoroutine(UpdateProceedTutorial());
    }
public void SkipTutorialStage2(){
                        GraceChecker = false;
                    GracePeriod = false;
                    StartCoroutine(UpdateProceedTutorial());
}
    void Update()
    {
        if (TutorialPrompt.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Alpha4))
                {
ShowTutorialPromptStage2();
                }
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    SkipTutorialStage2();
                }
            }
        }

        else if (TutorialContinuePrompt.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Alpha4) || Input.GetMouseButtonDown(0))
                {
                    SkipTutorialStage2();
                }
            }
        }
    }

    private IEnumerator PrintText()
    {
        while (TutorialText.activeSelf)
        {
            CurrentString = PrintString[TutorialStage];

            for (int i = 0; i < CurrentString.Length; i++)
            {
                TextObject.text = string.Concat(TextObject.text, CurrentString[i]);
                yield return new WaitForSeconds(CharWaitTime);
            }

            TutorialStage++;

            if (TutorialStage < 2)
            {
                //yield return new WaitForSeconds(DisableTimePerCharacter * (float)CurrentString.Length);
                //StartCoroutine(UpdateProceedTutorial());
                yield return new WaitForSeconds(1);
                TutorialContinuePrompt.SetActive(true);
            }

            if (TutorialStage == 2)
            {
                //yield return new WaitForSeconds(DisableTimePerCharacter * (float)CurrentString.Length);
                yield return new WaitForSeconds(1);
                TutorialPrompt.SetActive(true);
            }

            while (!ProceedTutorial)
            {
                yield return null;
            }

            TutorialContinuePrompt.SetActive(false);

            TextObject.text = "";
            CurrentString = "";
        }
    }

    private IEnumerator UpdateProceedTutorial()
    {
        ProceedTutorial = true;
        yield return new WaitForSeconds(1);
        ProceedTutorial = false;
    }

    private void OnDisable()
    {
        if (TextObject)
            TextObject.text = "";
        StopCoroutine(PrintText());
        TutorialStage = 0;
    }

    private IEnumerator WaitForGrace()
    {
        float duration = 0.25f;
        float totalTime = 0;
        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;
            yield return null;
        }

        GracePeriod = true;
    }
}
