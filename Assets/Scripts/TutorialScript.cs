using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject Type6plastic;
    public GameObject InGameUI;
    public GameObject GOUI;
    public GameObject TutorialPrompt;
    public GameObject TutorialContinuePrompt;
    public GameObject RecyclingTips;
    public GameManager GM;
    public GameObject ContinueButton;

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
        Type6plastic.SetActive(false);
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
                    ShowTutorial();
                }
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    SkipTutorial();
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
                    GraceChecker = false;
                    GracePeriod = false;
                    StartCoroutine(UpdateProceedTutorial());
                }
            }
        }
    }

    private IEnumerator PrintText()
    {
        GameObject HUD = InGameUI.transform.Find("HUDOverlay").gameObject;

        while (TutorialText.activeSelf)
        {
            CurrentString = PrintString[TutorialStage];

            // if (TutorialStage == 1)
            // {
            //     InGameUI.SetActive(true);
            //     DisposalEnemies.SetActive(true);
            //     GOUI.SetActive(false);
            // }
            if (TutorialStage == 2)
            {
                GOUI.SetActive(false);
                InGameUI.SetActive(true);
                string[] flashButtons = { "button_red", "button_green", "button_orange", "button_black" };
                string[] hideButtons = { "LeftArrow", "RightArrow" };
                StartCoroutine(FlashButtons(InGameUI, 3, 0.5f, flashButtons, hideButtons, false, HUD));
            }
            if (TutorialStage == 6)
            {
                StartCoroutine(FlashHUD(3, 0.5f, HUD));
                NormalEnemies.SetActive(true);
            }

            if (TutorialStage == 8)
            {
                NormalEnemies.SetActive(false);

                DisposalEnemies.SetActive(true);
            }
            if (TutorialStage == 12)
            {
                Type6plastic.SetActive(true);
            }

            //if (TutorialStage == 10)
            //{
            //    NormalEnemies.SetActive(true);
            //}

            // if (TutorialStage == 10)
            // {
            //     DisposalEnemies.SetActive(true);
            //     TintedEnemies.SetActive(true);
            //     NormalEnemies.SetActive(false);
            // }

            // if (TutorialStage == 11)
            // {
            //     DisposalEnemies.SetActive(true);
            //     TintedEnemies.SetActive(true);
            //     NormalEnemies.SetActive(false);
            // }

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

            if (TutorialStage < 14)
            {
                //yield return new WaitForSeconds(DisableTimePerCharacter * (float)CurrentString.Length);
                //StartCoroutine(UpdateProceedTutorial());
                yield return new WaitForSeconds(1);
                TutorialContinuePrompt.SetActive(true);
            }

            if (TutorialStage == 14)
            {
                //yield return new WaitForSeconds(DisableTimePerCharacter * (float)CurrentString.Length);
                yield return new WaitForSeconds(1);
                TutorialPrompt.SetActive(true);
            }

            // if (TutorialStage > 10 && TutorialStage < 13)
            // {
            //     //yield return new WaitForSeconds(DisableTimePerCharacter * (float)CurrentString.Length);
            //     //StartCoroutine(UpdateProceedTutorial());
            //     yield return new WaitForSeconds(1);
            //     TutorialContinuePrompt.SetActive(true);
            // }

            // if (TutorialStage == 13)
            // {
            //     //yield return new WaitForSeconds(DisableTimePerCharacter * (float)CurrentString.Length);
            //     yield return new WaitForSeconds(1);
            //     TutorialPrompt.SetActive(true);
            // }

            while (!ProceedTutorial)
            {
                yield return null;
            }

            TutorialContinuePrompt.SetActive(false);

            TextObject.text = "";
            CurrentString = "";
        }
    }
    private IEnumerator FlashHUD(int flashes, float interval, GameObject HUD)
    {
        HUD.SetActive(false);
        for (int i = 0; i < flashes; i++)
        {
            HUD.SetActive(false);
            yield return new WaitForSeconds(interval);
            HUD.SetActive(true);
            yield return new WaitForSeconds(interval);

        }

    }
    public IEnumerator FlashButtons(GameObject parent, int flashes, float interval, string[] flashButtons, string[] hideButtons, bool showHUD, GameObject HUD)
    {
        if (showHUD == false)
        {
            HUD.SetActive(false);
        }

        Button[] hideButtonsArray = parent.GetComponentsInChildren<Button>(true)
        .Where(hideButton => hideButtons.Contains(hideButton.gameObject.name))
        .ToArray();

        foreach (Button button in hideButtonsArray)
        {
            button.gameObject.SetActive(false);
        }
        Button[] buttons = parent.GetComponentsInChildren<Button>(true)
       .Where(button => flashButtons.Contains(button.gameObject.name))
       .ToArray();
        for (int i = 0; i < flashes; i++)
        {
            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(interval);
            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(interval);
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
    public void ShowTutorial()
    {
        GraceChecker = false;
        GracePeriod = false;
        TutorialText.SetActive(false);
        DisposalEnemies.SetActive(false);
        TintedEnemies.SetActive(false);
        TutorialPrompt.SetActive(false);
        TutorialContinuePrompt.SetActive(false);
        RecyclingTips.SetActive(false);
        ContinueButton.SetActive(false);
        GM.NewGame();

        GM.MenusToggleOff(GM.TutorialObjects);


    }
    public void SkipTutorial()
    {
        GraceChecker = false;
        GracePeriod = false;
        //TutorialStage = 10;  
        DisposalEnemies.SetActive(false);
        TintedEnemies.SetActive(false);
        NormalEnemies.SetActive(false);
        Type6plastic.SetActive(false);
        InGameUI.SetActive(false);
        TutorialContinuePrompt.SetActive(false);
        TutorialPrompt.SetActive(false);
        TutorialPlayer.SetActive(false);
        TutorialText.SetActive(false);
        RecyclingTips.SetActive(true);
        ContinueButton.SetActive(true);

    }
}