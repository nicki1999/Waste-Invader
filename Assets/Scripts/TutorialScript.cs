using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
    public GameObject TutorialPlayer;
    public GameObject TutorialEnemy;
    public GameObject TutorialText;
    public GameObject DisposalEnemies;
    public GameObject TintedEnemies;
    public GameObject NormalEnemies;
    public GameObject InGameUI;
    public GameObject GOUI;
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
        DisposalEnemies.SetActive(false);
        TintedEnemies.SetActive(false);
        NormalEnemies.SetActive(false);
        StartCoroutine(PrintText());
        TutorialPrompt.SetActive(false);
        TutorialContinuePrompt.SetActive(false);
        GraceChecker = false;
        GracePeriod = false;
        TutorialStage = 0;
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
                    GraceChecker = false;
                    GracePeriod = false;
                    TutorialText.SetActive(false);
                    DisposalEnemies.SetActive(false);
                    TintedEnemies.SetActive(false);
                    TutorialPrompt.SetActive(false);
                    TutorialContinuePrompt.SetActive(false);
                    GM.NewGame();
                    GM.MenusToggleOff(GM.TutorialObjects);
                }
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    GraceChecker = false;
                    GracePeriod = false;
                    TutorialStage = 10;
                    TutorialPrompt.SetActive(false);
                    StartCoroutine(UpdateProceedTutorial());
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
                if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Alpha4))
                {
                    GraceChecker = false;
                    GracePeriod = false;
                    StartCoroutine(UpdateProceedTutorial());
                }
            }
        }
    }

    private IEnumerator PrintText()
    {
        while (TutorialText.activeSelf)
        {
            CurrentString = PrintString[TutorialStage];

            if (TutorialStage == 1)
            {
                InGameUI.SetActive(true);
                DisposalEnemies.SetActive(true);
                GOUI.SetActive(false);
            }

            if (TutorialStage == 8)
            {
                //DisposalEnemies.SetActive(false);
                TintedEnemies.SetActive(true);
            }

            //if (TutorialStage == 10)
            //{
            //    NormalEnemies.SetActive(true);
            //}

            if (TutorialStage == 10)
            {
                DisposalEnemies.SetActive(true);
                TintedEnemies.SetActive(true);
                NormalEnemies.SetActive(false);
            }

            if (TutorialStage == 11)
            {
                DisposalEnemies.SetActive(true);
                TintedEnemies.SetActive(true);
                NormalEnemies.SetActive(false);
            }

            //if (TutorialStage == 14)
            //{
            //    DisposalEnemies.SetActive(false);
            //    TintedEnemies.SetActive(false);
            //    NormalEnemies.SetActive(true);
            //}

            for (int i = 0; i < CurrentString.Length; i++)
            {
                TextObject.text = string.Concat(TextObject.text, CurrentString[i]);
                yield return new WaitForSeconds(CharWaitTime);
            }

            TutorialStage++;
            
            if (TutorialStage < 10)
            {
                //yield return new WaitForSeconds(DisableTimePerCharacter * (float)CurrentString.Length);
                //StartCoroutine(UpdateProceedTutorial());
                yield return new WaitForSeconds(1);
                TutorialContinuePrompt.SetActive(true);
            }

            if (TutorialStage == 10)
            {
                //yield return new WaitForSeconds(DisableTimePerCharacter * (float)CurrentString.Length);
                yield return new WaitForSeconds(1);
                TutorialPrompt.SetActive(true);
            }

            if (TutorialStage > 10 && TutorialStage < 13)
            {
                //yield return new WaitForSeconds(DisableTimePerCharacter * (float)CurrentString.Length);
                //StartCoroutine(UpdateProceedTutorial());
                yield return new WaitForSeconds(1);
                TutorialContinuePrompt.SetActive(true);
            }

            if (TutorialStage == 13)
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