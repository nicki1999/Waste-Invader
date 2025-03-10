using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    private Player player;
    private EnemyController enemyController;

    public GameObject gameOverUI;
    public GameObject ratingUI;
    public Text scoreText;
    public Text LivesText;
    public Text HighScoreText;
    public GameObject InGameUI;
    public GameObject MenuUI;
    public GameObject CreditsUI;
    public GameObject HighScoresUI;
    public GameObject LeaderboardUI;
    public GameObject TutorialUI;
    public GameObject SettingsUI;
    public GameObject CampusUI;
    public GameObject RecyclingUI;
    public GameObject AboutUI;
    public GameObject StatTrackerUI;
    public GameObject LoadingUI;
    public Text[] StatTrackerStats;
    public GameObject IdleMenuUI;
    public GameObject KeyboardUI;
    public Button KeyboardButton;
    public GameObject PopUpUI;
    public GameObject Stage2UI;
    public GameObject Stage2TutorialUI;
    public GameObject Stage2TutorialObjects;
    public GameObject Stage2Tips;
    public GameObject ContinueButtonStage2;
    public GameObject ContinueButtonStage1;
    public Text PopUpText;
    public GameObject MenuBG;
    public EventSystem eventSystem;
    public Transform[] toggleOnGO;
    public GameObject TutorialObjects;
    public GameObject TutorialText;
    public GameObject TutorialTextPrompt;
    public GameObject IdleImage;
    private Vector3 InitialIdlePosition;
    public ScrollViewSample wiki;
    public Material DaySkybox;
    public Material NightSkybox;
    public Material TutorialSkybox;

    public GameObject[] buttons;
    public GameObject LeftButtonContainer;
    public GameObject RightButtonContainer;
    public Canvas canvas;
    private bool AFKCheck;
    private bool IdleCheck;
    private bool GracePeriod;
    private bool GraceChecker;
    private int totalNumberOfRatings;
    private int totalRating;
    private int GamesPlayed;
    public int TutorialsWatched;
    private bool TimerRunning;
    private float TimePlayed;
    private bool MusicMuted;
    private int totalStage1Score;
    private int totalStage2Score;
    private int totalScore;
    private int totalWaves;
    private bool PauseTimers;
    private float WaveScore;
    public bool WatchedStage2Tutorial;

    public int score { get; private set; }
    private int HighScore;
    public int lives { get; private set; }
    public bool TutorialActive { get; private set; }
    public int Stage;
    private int Wave;
    public int TintedEnemyChance;

    public AudioManager audioManager;
    public KeyboardScript Keyboard;
    public string CurrentPlayer;

    public Text[] ScoreList;
    public Text[] WaveList;
    public Text[] NameList;
    public Text[] LeaderboardScoreList;
    public Text[] LeaderboardWaveList;
    public Text[] LeaderboardNameList;
    public GameObject leaderboardObject;

    private float biggerTolerance = 0.1f;

    private KeyCode[] sequence = new KeyCode[]
    {
        KeyCode.Joystick1Button1,
        KeyCode.Joystick1Button2,
        KeyCode.Joystick1Button3,
        KeyCode.Joystick1Button4,
        KeyCode.Joystick1Button5,
        KeyCode.Joystick1Button6,
        KeyCode.Joystick1Button7,
        KeyCode.Joystick1Button1
    };
    private int sequenceIndex;

    // Find references
    private void Awake()
    {
        player = FindObjectOfType<Player>();
        enemyController = FindObjectOfType<EnemyController>();
    }


    // Reference death actions, and start new game
    private void Start()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();


        player.killed += OnPlayerKilled;
        enemyController.killed += OnEnemyKilled;
        enemyController.IncorrectHit += OnEnemyIncorrectlyHit;

        audioManager.Play("MenuMusic");
        audioManager.Stop("Level1Music");
        audioManager.Stop("Level2Music");

        AFKCheck = false;
        IdleCheck = false;
        GracePeriod = false;
        GraceChecker = false;
        TutorialActive = false;
        TimerRunning = false;
        MusicMuted = false;
        PauseTimers = false;
        WatchedStage2Tutorial = false;
        InitialIdlePosition = IdleImage.transform.position;

        if (PlayerPrefs.HasKey("ScoreList1"))
        {
            for (int i = 0; i < ScoreList.Length; i++)
            {
                ScoreList[i].text = PlayerPrefs.GetString("ScoreList" + i);
            }
            for (int i = 0; i < WaveList.Length; i++)
            {
                WaveList[i].text = PlayerPrefs.GetString("WaveList" + i);
            }
            for (int i = 0; i < NameList.Length; i++)
            {
                NameList[i].text = PlayerPrefs.GetString("NameList" + i);
            }
        }

        if (PlayerPrefs.HasKey("totalRating"))
        {
            totalNumberOfRatings = PlayerPrefs.GetInt("totalNumberOfRatings");
            totalRating = PlayerPrefs.GetInt("totalRating");

            float average = (float)totalRating / (float)totalNumberOfRatings;
            average = (Mathf.Round(average * 100) / 100.0f);

            StatTrackerStats[3].text = average.ToString();
        }

        if (PlayerPrefs.HasKey("GamesPlayed"))
        {
            GamesPlayed = PlayerPrefs.GetInt("GamesPlayed");
            StatTrackerStats[0].text = GamesPlayed.ToString();
        }

        if (PlayerPrefs.HasKey("totalSessionDuration"))
        {
            TimePlayed = PlayerPrefs.GetFloat("totalSessionDuration", TimePlayed);
            float average = (float)TimePlayed / (float)GamesPlayed;
            average = (Mathf.Round(average * 100) / 100.0f);

            StatTrackerStats[4].text = average.ToString();
        }

        if (PlayerPrefs.HasKey("TutorialsWatched"))
        {
            TutorialsWatched = PlayerPrefs.GetInt("TutorialsWatched");
            StatTrackerStats[5].text = TutorialsWatched.ToString();
        }

        if (PlayerPrefs.HasKey("TotalStage1Score"))
        {
            totalStage1Score = PlayerPrefs.GetInt("TotalStage1Score");
        }

        if (PlayerPrefs.HasKey("TotalStage2Score"))
        {
            totalStage2Score = PlayerPrefs.GetInt("TotalStage2Score");
        }

        if (PlayerPrefs.HasKey("TotalScore"))
        {
            totalScore = PlayerPrefs.GetInt("TotalScore");
        }

        if (PlayerPrefs.HasKey("TotalWaves"))
        {
            totalWaves = PlayerPrefs.GetInt("TotalWaves");
        }

        if (totalStage2Score > 0)
        {
            float averageStage1Score = (float)totalStage1Score / (float)GamesPlayed;
            averageStage1Score = (Mathf.Round(averageStage1Score * 100) / 100.0f);

            float averageStage2Score = (float)totalStage2Score / (float)GamesPlayed;
            averageStage2Score = (Mathf.Round(averageStage2Score * 100) / 100.0f);

            float averageTotalScore = (float)totalScore / (float)GamesPlayed;
            averageTotalScore = (Mathf.Round(averageTotalScore * 100) / 100.0f);

            float averageWaves = (float)totalWaves / (float)GamesPlayed;
            averageWaves = (Mathf.Round(averageWaves * 100) / 100.0f);

            float average = (((((averageTotalScore / averageWaves) / ((averageStage1Score + averageStage2Score) / 2)) - 1) * 100));
            average = (Mathf.Round(average * 100) / 100.0f);

            StatTrackerStats[2].text = average.ToString() + "%";
        }
        else if (totalStage1Score > 0)
        {
            float averageStage1Score = (float)totalStage1Score / (float)GamesPlayed;
            averageStage1Score = (Mathf.Round(averageStage1Score * 100) / 100.0f);

            float averageTotalScore = (float)totalScore / (float)GamesPlayed;
            averageTotalScore = (Mathf.Round(averageTotalScore * 100) / 100.0f);

            float averageWaves = (float)totalWaves / (float)GamesPlayed;
            averageWaves = (Mathf.Round(averageWaves * 100) / 100.0f);

            float average = ((((((averageTotalScore / averageWaves) - averageStage1Score) / averageStage1Score) - 1) * 100));
            average = (Mathf.Round(average * 100) / 100.0f);

            StatTrackerStats[2].text = average.ToString() + "%";
        }

        if (totalScore > 0)
        {
            float average = (float)totalScore / (float)GamesPlayed;
            average = (Mathf.Round(average * 100) / 100.0f);

            StatTrackerStats[1].text = average.ToString();
        }

        MainMenu();

        GetLeaderboard(MenuUI);

    }

    // If no lives and player hits enter, restart
    private void Update()
    {
        if ((RecyclingUI.activeSelf && ContinueButtonStage1.activeSelf) || RecyclingUI.activeSelf && ContinueButtonStage2.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitGame();
                Debug.Log("pressed escape");
            }
        }
        float currentAspect = (float)Screen.width / Screen.height;

        if (Mathf.Abs(currentAspect - (18f / 9f)) < biggerTolerance || Mathf.Abs(currentAspect - (19.5f / 9f)) < biggerTolerance)
        {
            //Debug.Log("18:9 & 19.5:9 Aspect Ratio");
            if (InGameUI.activeSelf)
            {
                canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0f;
            }
            else
            {
                canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
            }

        }

        if (MenuUI.activeSelf)
        {
            LeftButtonContainer.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/bg_menu_initiatives");
            RightButtonContainer.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/bg_menu_initiatives");

            if (Input.GetKeyDown(KeyCode.Escape))
                ExitGame();
        }

        if (ratingUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                totalNumberOfRatings++;
                totalRating += 1;
                float average = (float)totalRating / (float)totalNumberOfRatings;
                average = (Mathf.Round(average * 100) / 100.0f);
                StatTrackerStats[3].text = average.ToString();
                PlayerPrefs.SetInt("totalNumberOfRatings", totalNumberOfRatings);
                PlayerPrefs.SetInt("totalRating", totalRating);

                audioManager.Stop("Level1Music");
                audioManager.Stop("Level2Music");
                audioManager.Stop("GameOver");
                audioManager.Play("MenuMusic");

                MainMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                totalNumberOfRatings++;
                totalRating += 2;
                float average = (float)totalRating / (float)totalNumberOfRatings;
                average = (Mathf.Round(average * 100) / 100.0f);
                StatTrackerStats[3].text = average.ToString();
                PlayerPrefs.SetInt("totalNumberOfRatings", totalNumberOfRatings);
                PlayerPrefs.SetInt("totalRating", totalRating);

                audioManager.Stop("Level1Music");
                audioManager.Stop("Level2Music");
                audioManager.Stop("GameOver");
                audioManager.Play("MenuMusic");

                MainMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.Alpha3))
            {
                totalNumberOfRatings++;
                totalRating += 3;
                float average = (float)totalRating / (float)totalNumberOfRatings;
                average = (Mathf.Round(average * 100) / 100.0f);
                StatTrackerStats[3].text = average.ToString();
                PlayerPrefs.SetInt("totalNumberOfRatings", totalNumberOfRatings);
                PlayerPrefs.SetInt("totalRating", totalRating);

                audioManager.Stop("Level1Music");
                audioManager.Stop("Level2Music");
                audioManager.Stop("GameOver");
                audioManager.Play("MenuMusic");

                MainMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Alpha4))
            {
                totalNumberOfRatings++;
                totalRating += 4;
                float average = (float)totalRating / (float)totalNumberOfRatings;
                average = (Mathf.Round(average * 100) / 100.0f);
                StatTrackerStats[3].text = average.ToString();
                PlayerPrefs.SetInt("totalNumberOfRatings", totalNumberOfRatings);
                PlayerPrefs.SetInt("totalRating", totalRating);

                audioManager.Stop("Level1Music");
                audioManager.Stop("Level2Music");
                audioManager.Stop("GameOver");
                audioManager.Play("MenuMusic");

                MainMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Alpha5))
            {
                totalNumberOfRatings++;
                totalRating += 5;
                float average = (float)totalRating / (float)totalNumberOfRatings;
                average = (Mathf.Round(average * 100) / 100.0f);
                StatTrackerStats[3].text = average.ToString();
                PlayerPrefs.SetInt("totalNumberOfRatings", totalNumberOfRatings);
                PlayerPrefs.SetInt("totalRating", totalRating);

                audioManager.Stop("Level1Music");
                audioManager.Stop("Level2Music");
                audioManager.Stop("GameOver");
                audioManager.Play("MenuMusic");

                MainMenu();
            }
        }

        if (TutorialUI.activeSelf)
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

        if (AboutUI.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ReturnButton(AboutUI);
                }

            }
        }

        if (HighScoresUI.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    MenusToggleOn(MenuUI);
                    MenusToggleOff(HighScoresUI);
                    audioManager.Play("Back");
                    GraceChecker = false;
                    GracePeriod = false;
                }

            }
        }
        if (LeaderboardUI.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    MenusToggleOn(MenuUI);
                    MenusToggleOff(LeaderboardUI);
                    audioManager.Play("Back");
                    GraceChecker = false;
                    GracePeriod = false;
                }

            }
        }
        if (LoadingUI.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {

                }

            }
        }

        if (CreditsUI.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ReturnButton(CreditsUI);
                }

            }
        }

        if (SettingsUI.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    MenusToggleOn(MenuUI);
                    MenusToggleOff(SettingsUI);
                    audioManager.Play("Back");
                    GraceChecker = false;
                    GracePeriod = false;
                }

            }
        }

        if (CampusUI.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    MenusToggleOn(MenuUI);
                    MenusToggleOff(CampusUI);
                    audioManager.Play("Back");
                    GraceChecker = false;
                    GracePeriod = false;
                }

            }
        }

        if (RecyclingUI.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    MenusToggleOn(MenuUI);
                    MenusToggleOff(RecyclingUI);
                    audioManager.Play("Back");
                    GraceChecker = false;
                    GracePeriod = false;
                }

            }
        }

        if (StatTrackerUI.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    MenusToggleOn(MenuUI);
                    MenusToggleOff(StatTrackerUI);
                    audioManager.Play("Back");
                    GraceChecker = false;
                    GracePeriod = false;
                }

            }
        }



        if (CreditsUI.activeSelf)
        {
            if (Input.GetKeyDown(sequence[sequenceIndex]))
            {
                if (++sequenceIndex == sequence.Length)
                {
                    sequenceIndex = 0;
                    StatTrackerUI.SetActive(true);
                    CreditsUI.SetActive(false);
                }
            }
            else if (Input.anyKeyDown)
                sequenceIndex = 0;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (InGameUI.activeSelf)
            {
                audioManager.Play("MenuMusic");
                audioManager.Stop("Level1Music");
                audioManager.Stop("Level2Music");
            }
            IdleCheck = false;
            MainMenu();
        }

        if (MenuUI.activeSelf && !IdleCheck)
        {
            IdleCheck = true;
            StartCoroutine(IdleScreen());
        }

        if (MenuUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.Alpha6))
            {
                ToggleMusicButton();
            }

            if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.B))
            {
                PlayerPrefs.DeleteAll();
            }
        }

        if (Stage2TutorialUI.activeSelf)
        {
            Debug.Log("Stage2TutorialUI is active");
            InGameUI.SetActive(false);
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    SkipTutorialStage2();

                }
                if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Alpha4))
                {
                    ShowTutorialStage2();
                }
            }
        }

        if (Stage2UI.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    SkipTipsPromptStage2();
                }
                if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Alpha4))
                {
                    ShowTipsPromptStage2();
                }
            }
        }

        if (Stage2Tips.activeSelf)
        {
            if (!GraceChecker)
            {
                GraceChecker = true;
                StartCoroutine(WaitForGrace());
            }
            if (GracePeriod)
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ShowTipsStage2();
                }
            }
        }

        if (IdleMenuUI.activeSelf && Input.anyKeyDown)
        {
            MainMenu();
            audioManager.Play("MenuMusic");
        }

        if (InGameUI.activeSelf)
        {
            Button[] conditionalButtons = InGameUI.GetComponentsInChildren<Button>(true);
            // buttons that need to be disabled for stage 1
            String[] disableButtons = new string[] { "button_yellow", "button_blue", "button_brown", "button_purple" };
            String[] disableButtonsStage2 = new string[] { "Button_Idle" };
            if (Stage == 1 || TutorialObjects.activeSelf)
            {
                foreach (Button button in conditionalButtons)
                {
                    foreach (string disableButton in disableButtons)
                    {
                        if (button.name.Contains(disableButton))
                        {
                            button.interactable = false;
                            button.gameObject.SetActive(false);
                        }
                    }
                }

            }
            else if (Stage == 2 && !Stage2TutorialObjects.activeSelf)
            {
                foreach (Button button in conditionalButtons)
                {
                    foreach (string disableButton in disableButtons)
                    {
                        if (button.name.Contains(disableButton))
                        {
                            button.interactable = true;
                            button.gameObject.SetActive(true);
                        }
                    }
                    foreach (string disableButton in disableButtonsStage2)
                    {
                        if (button.name.Contains(disableButton))
                        {
                            button.interactable = false;
                            button.gameObject.SetActive(false);
                        }
                    }
                }
                LeftButtonContainer.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/bg_menu_scores");
                RightButtonContainer.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/bg_menu_scores");

            }

            if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Alpha1))
            {
                TriggerButtonAnimation("button_red");
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                TriggerButtonAnimation("button_orange");
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.Alpha3))
            {
                TriggerButtonAnimation("button_yellow");
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Alpha4))
            {
                TriggerButtonAnimation("button_green");
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Alpha5))
            {
                TriggerButtonAnimation("button_blue");
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.Alpha6))
            {
                TriggerButtonAnimation("button_purple_exception");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Joystick1Button6))
            {
                TriggerButtonAnimation("button_brown");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                TriggerButtonAnimation("button_black");
            }
        }

    }
    public void TriggerButtonAnimation(string buttonBaseName)
    {
        int selectedWeapon = 0;
        string audioType = "";
        if (buttonBaseName == "button_red")
        {
            selectedWeapon = 0;
            audioType = "Shoot1";
        }
        else if (buttonBaseName == "button_orange")
        {
            selectedWeapon = 1;
            audioType = "Shoot2";
        }
        else if (buttonBaseName == "button_yellow")
        {
            selectedWeapon = 2;
            audioType = "Shoot2";
        }
        else if (buttonBaseName == "button_green")
        {
            selectedWeapon = 3;
            audioType = "Shoot4";
        }
        else if (buttonBaseName == "button_blue")
        {
            selectedWeapon = 4;
            audioType = "Shoot5";
        }
        else if (buttonBaseName == "button_purple_exception")
        {
            selectedWeapon = 5;
            audioType = "Shoot6";
            buttonBaseName = "button_purple_exception";

        }

        else if (buttonBaseName == "button_purple")
        {
            selectedWeapon = 7;
            audioType = "Shoot6";
        }
        else if (buttonBaseName == "button_brown")
        {
            selectedWeapon = 6;
            audioType = "Shoot7";
        }
        else if (buttonBaseName == "button_black")
        {
            selectedWeapon = 7;
            audioType = "Shoot8";
        }

        TriggerButton(buttonBaseName, selectedWeapon, audioType);
    }
    void TriggerButton(string buttonBaseName, int selectedWeapon, string audioType)
    {
        //Debug.Log($"Triggering button {selectedWeapon}");
        this.player.FireWeaponButton(selectedWeapon, audioType);
        GameObject button = FindButtonByName(buttonBaseName);
        if (button != null)
        {
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                Sprite idleSprite = LoadSprite($"{buttonBaseName}_1");
                Sprite halfPressedSprite = LoadSprite($"{buttonBaseName}_2");
                Sprite fullyPressedSprite = LoadSprite($"{buttonBaseName}_3");

                StartCoroutine(AnimateButton(buttonImage, idleSprite, halfPressedSprite, fullyPressedSprite));
            }
        }
    }
    GameObject FindButtonByName(string buttonBaseName)
    {
        foreach (GameObject button in buttons)
        {
            if (button.name.Contains(buttonBaseName))
            {
                return button;
            }
        }
        return null;
    }
    Sprite LoadSprite(string spriteName)
    {
        return Resources.Load<Sprite>($"Sprites/temp/ui/{spriteName}");
    }
    System.Collections.IEnumerator AnimateButton(Image buttonImage, Sprite idleSprite, Sprite halfPressedSprite, Sprite fullyPressedSprite)
    {
        // Half-pressed
        buttonImage.sprite = halfPressedSprite;
        yield return new WaitForSeconds(0.1f);

        // Fully pressed
        buttonImage.sprite = fullyPressedSprite;
        yield return new WaitForSeconds(0.1f);

        // Back to idle
        buttonImage.sprite = idleSprite;
    }
    private void MainMenu()
    {
        foreach (Transform item in toggleOnGO)
        {
            item.gameObject.SetActive(false);
        }

        InGameUI.SetActive(false);
        MenuUI.SetActive(true);
        CreditsUI.SetActive(false);
        HighScoresUI.SetActive(false);
        LeaderboardUI.SetActive(false);
        TutorialUI.SetActive(false);
        SettingsUI.SetActive(false);
        CampusUI.SetActive(false);
        RecyclingUI.SetActive(false);
        AboutUI.SetActive(false);
        StatTrackerUI.SetActive(false);
        IdleMenuUI.SetActive(false);
        TutorialObjects.SetActive(false);
        wiki.CreateItemsNoTutorialStage1();

        TutorialText.SetActive(false);
        TutorialTextPrompt.SetActive(false);
        KeyboardUI.SetActive(false);
        gameOverUI.SetActive(false);
        ratingUI.SetActive(false);
        PopUpUI.SetActive(false);
        Stage2UI.SetActive(false);
        Stage2Tips.SetActive(false);
        LoadingUI.SetActive(false);
        TutorialObjects.GetComponentInChildren<Player>().Tutorial = false;

        eventSystem.SetSelectedGameObject(MenuUI.GetComponentInChildren<Button>().gameObject, new BaseEventData(eventSystem));

        TimerRunning = false;
        PauseTimers = false;
        IdleCheck = false;
        Stage = 0;

        KeyboardButton.onClick.RemoveListener(NewGame);
        KeyboardButton.onClick.RemoveListener(StartTutorial);

        if (!MenuBG.activeSelf)
            MenuBG.SetActive(true);
    }

    // Reset score and lives, disable game over ui, and restart game
    public void NewGame()
    {
        GamesPlayed++;
        StatTrackerStats[0].text = GamesPlayed.ToString();
        PlayerPrefs.SetInt("GamesPlayed", GamesPlayed);

        if (MenuBG.activeSelf)
            MenuBG.SetActive(false);

        RenderSettings.skybox = DaySkybox;

        gameOverUI.SetActive(false);
        ratingUI.SetActive(false);
        TutorialActive = false;

        foreach (Transform item in toggleOnGO)
        {
            item.gameObject.SetActive(true);
        }

        InGameUI.SetActive(true);
        MenuUI.SetActive(false);
        TutorialUI.SetActive(false);
        if (TutorialObjects != null)
        {
            Player player = TutorialObjects.GetComponentInChildren<Player>();
            if (player != null)
            {
                player.Tutorial = false;
            }
            else
            {
                Debug.LogWarning("Player component not found in TutorialObjects.");
            }
        }
        else
        {
            Debug.LogError("TutorialObjects is null!");
        }
        Stage = 1;
        TintedEnemyChance = 0;

        Wave = 0;
        SetScore(0);
        SetLives(3);
        NewRound();

        audioManager.Stop("MenuMusic");
        audioManager.Play("Level1Music");
        audioManager.Stop("Level2Music");

        TimerRunning = true;
        PauseTimers = false;
        WatchedStage2Tutorial = false;

        StartCoroutine(Countdown());
        StartCoroutine(SessionDuration());
    }

    public void StartTutorial()
    {
        RenderSettings.skybox = TutorialSkybox;
        TutorialActive = true;
        TutorialObjects.SetActive(true);
        wiki.CreateItemsTutorialStage1();


        TutorialText.SetActive(true);
        TutorialObjects.GetComponentInChildren<Player>().Tutorial = true;

        if (MenuBG.activeSelf)
            MenuBG.SetActive(false);
    }

    public void MenusToggleOff(GameObject Toggle)
    {
        Toggle.SetActive(false);
        if (!MenuUI.gameObject.activeSelf)
        {
            if (HighScoresUI.activeSelf)
                eventSystem.SetSelectedGameObject(HighScoresUI.GetComponentInChildren<Scrollbar>().gameObject, new BaseEventData(eventSystem));
            else if (LoadingUI.activeSelf)
                eventSystem.SetSelectedGameObject(LoadingUI.GetComponentInChildren<Scrollbar>().gameObject, new BaseEventData(eventSystem));
            else if (SettingsUI.activeSelf)
                eventSystem.SetSelectedGameObject(SettingsUI.GetComponentInChildren<Slider>().gameObject, new BaseEventData(eventSystem));
            else if (CampusUI.activeSelf)
                eventSystem.SetSelectedGameObject(CampusUI.GetComponentInChildren<Scrollbar>().gameObject, new BaseEventData(eventSystem));
            else if (RecyclingUI.activeSelf)
                eventSystem.SetSelectedGameObject(RecyclingUI.GetComponentInChildren<Scrollbar>().gameObject, new BaseEventData(eventSystem));
            else if (KeyboardUI.activeSelf)
                eventSystem.SetSelectedGameObject(KeyboardUI.GetComponentInChildren<Button>().gameObject, new BaseEventData(eventSystem));
            else if (CreditsUI.activeSelf)
                eventSystem.SetSelectedGameObject(CreditsUI.GetComponentInChildren<Scrollbar>().gameObject, new BaseEventData(eventSystem));
            else if (LeaderboardUI.activeSelf)
                eventSystem.SetSelectedGameObject(LeaderboardUI.GetComponentInChildren<Scrollbar>().gameObject, new BaseEventData(eventSystem));

            else if (Stage2Tips.activeSelf)
                eventSystem.SetSelectedGameObject(Stage2Tips.GetComponentInChildren<Scrollbar>().gameObject, new BaseEventData(eventSystem));

        }
        else
        {
            eventSystem.SetSelectedGameObject(MenuUI.GetComponentInChildren<Button>().gameObject, new BaseEventData(eventSystem));
        }
    }

    public void MenusToggleOn(GameObject Toggle)
    {
        Toggle.SetActive(true);
    }
    public void ShowTutorial()
    {
        MenusToggleOn(KeyboardUI);
        MenusToggleOff(TutorialUI);
        //KeyboardButton.onClick.AddListener(() => StartCoroutine(Main.Instance.web.FilterPlayerName(Keyboard.storedText)));
        KeyboardButton.onClick.AddListener(() =>
        {
            StartCoroutine(WaitForErrorTextAndShowTutorial());
        });
        audioManager.Play("Confirm");
        GraceChecker = false;
        GracePeriod = false;
    }
    public void SkipTutorial()
    {
        MenusToggleOn(KeyboardUI);
        MenusToggleOff(TutorialUI);
        KeyboardButton.onClick.AddListener(() =>
        {
            StartCoroutine(WaitForErrorTextAndStartGame());
        });
        // KeyboardButton.onClick.AddListener(NewGame);
        audioManager.Play("Confirm");
        GraceChecker = false;
        GracePeriod = false;
    }
    public void ShowTutorialStage2()
    {
        Stage2TutorialObjects.SetActive(true);
        LeftButtonContainer.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/bg_menu_scores");
        RightButtonContainer.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/bg_menu_scores");
        foreach (Transform item in toggleOnGO)
        {
            item.gameObject.SetActive(false);
        }

        audioManager.Play("Confirm");
        MenusToggleOff(Stage2TutorialUI);
        GraceChecker = false;
        GracePeriod = false;
    }

    public void SkipTipsPromptStage2()
    {
        RecyclingUI.SetActive(false);
        ContinueButtonStage2.SetActive(false);
        MenusToggleOff(Stage2UI);

        foreach (Transform item in toggleOnGO)
        {
            item.gameObject.SetActive(true);
        }

        PauseTimers = false;
        audioManager.Play("Confirm");
        NewRound();
        GraceChecker = false;
        GracePeriod = false;
    }
    public void ShowTipsPromptStage2()
    {
        RecyclingUI.SetActive(true);
        ContinueButtonStage2.SetActive(true);
        // Stage2Tips.SetActive(true);
        foreach (Transform item in toggleOnGO)
        {
            item.gameObject.SetActive(false);
        }
        // audioManager.Play("Confirm");
        MenusToggleOff(Stage2UI);
        GraceChecker = false;
        GracePeriod = false;
    }
    public void ShowTipsStage2()
    {
        Stage2Tips.SetActive(false);
        foreach (Transform item in toggleOnGO)
        {
            item.gameObject.SetActive(true);
        }
        PauseTimers = false;
        NewRound();
        audioManager.Play("Back");
        GraceChecker = false;
        GracePeriod = false;
    }

    private IEnumerator WaitForErrorTextAndShowTutorial()
    {
        CurrentPlayer = Keyboard.storedText;

        // Wait for the coroutine to finish before continuing
        yield return StartCoroutine(Main.Instance.web.FilterPlayerName(CurrentPlayer));

        // Now execute the rest of the code
        Keyboard.storedText = "";
        Keyboard.displayText.text = "";

        if (string.IsNullOrEmpty(Main.Instance.web.errorText.text))
        {
            Debug.Log("Error text is null or empty");
            MenusToggleOff(KeyboardUI);
            StartTutorial();
        }
        else
        {
            Debug.Log($"Error text is not empty: {Main.Instance.web.errorText.text}");
        }
    }

    private IEnumerator WaitForErrorTextAndStartGame()
    {
        CurrentPlayer = Keyboard.storedText;

        // Wait for the coroutine to finish before continuing
        yield return StartCoroutine(Main.Instance.web.FilterPlayerName(CurrentPlayer));

        // Now execute the rest of the code
        Keyboard.storedText = "";
        Keyboard.displayText.text = "";

        if (string.IsNullOrEmpty(Main.Instance.web.errorText.text))
        {
            Debug.Log("Error text is null or empty");
            MenusToggleOff(KeyboardUI);
            NewGame();
        }
        else
        {
            Debug.Log($"Error text is not empty: {Main.Instance.web.errorText.text}");
        }
    }


    public void SkipTutorialStage2()
    {
        MenusToggleOff(Stage2TutorialUI);
        MenusToggleOn(Stage2UI);
        audioManager.Play("Confirm");
        GraceChecker = false;
        GracePeriod = false;
    }
    public void ToggleMusicButton()
    {
        if (MusicMuted)
        {
            MusicMuted = false;
            audioManager.ToggleMusic(MusicMuted);
        }
        else
        {
            MusicMuted = true;
            audioManager.ToggleMusic(MusicMuted);
        }
    }

    public void ReturnButton(GameObject toggle)
    {
        MenusToggleOn(MenuUI);
        MenusToggleOff(toggle);
        audioManager.Play("Back");
        GraceChecker = false;
        GracePeriod = false;
    }
    public void ExitGame()
    {
        Application.Quit(0);
    }

    // Respawn enemies to base areas
    private void NewRound()
    {
        enemyController.ResetEnemies();
        enemyController.gameObject.SetActive(true);

        Respawn();
    }

    // Respawn player
    private void Respawn()
    {
        Vector3 position = player.transform.position;
        position.x = 0f;
        player.transform.position = position;
        player.Health = player.maxHealth;
        player.gameObject.SetActive(true);
        player.healthSlider.value = player.Health;
    }

    // activate game over gui and disable game
    private IEnumerator GameOver()
    {
        float duration = 3f;
        float totalTime = 0;

        audioManager.Stop("Level1Music");
        audioManager.Stop("Level2Music");
        audioManager.Play("GameOver");

        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;
            yield return null;
        }

        foreach (Transform item in toggleOnGO)
        {
            item.gameObject.SetActive(false);
        }
        gameOverUI.SetActive(true);
        TimerRunning = false;

        duration = 3f;
        totalTime = 0;

        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;
            yield return null;
        }

        if (WatchedStage2Tutorial == true)
            ratingUI.SetActive(true);
        else
        {
            audioManager.Stop("Level1Music");
            audioManager.Stop("Level2Music");
            audioManager.Stop("GameOver");
            audioManager.Play("MenuMusic");

            MainMenu();
        }

        int[] scores = new int[25];

        for (int i = 0; i < ScoreList.Length; i++)
        {
            scores[i] = int.Parse(ScoreList[i].text.ToString());
        }

        for (int i = 0; i < scores.Length; i++)
        {
            if (score > scores[i])
            {
                int[] tempScore = new int[25];
                string[] tempWave = new string[25];
                string[] tempName = new string[25];

                if (scores[i] > 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        tempScore[j] = scores[j];
                        tempWave[j] = WaveList[j].text;
                        tempName[j] = NameList[j].text;
                    }

                    for (int j = i + 1; j < scores.Length; j++)
                    {
                        tempScore[j] = scores[j - 1];
                        tempWave[j] = WaveList[j - 1].text;
                        tempName[j] = NameList[j - 1].text;
                    }

                    tempScore[i] = score;
                    tempWave[i] = Wave.ToString();
                    tempName[i] = CurrentPlayer;

                    for (int j = 0; j < scores.Length; j++)
                    {
                        ScoreList[j].text = (tempScore[j].ToString().PadLeft(5, '0'));
                        WaveList[j].text = tempWave[j].ToString();
                        NameList[j].text = tempName[j];
                    }

                }
                else
                {

                    ScoreList[i].text = (score.ToString().PadLeft(5, '0'));
                    WaveList[i].text = Wave.ToString();
                    NameList[i].text = CurrentPlayer;
                }

                for (int j = 0; j < ScoreList.Length; j++)
                {
                    PlayerPrefs.SetString("ScoreList" + j, ScoreList[j].text);
                    Debug.Log("ScoreList" + j + " = " + ScoreList[j].text);
                }
                for (int j = 0; j < WaveList.Length; j++)
                {
                    PlayerPrefs.SetString("WaveList" + j, WaveList[j].text);
                    Debug.Log("WaveList" + j + " = " + WaveList[j].text);
                }
                for (int j = 0; j < NameList.Length; j++)
                {
                    PlayerPrefs.SetString("NameList" + j, NameList[j].text);
                    Debug.Log("NameList" + j + " = " + NameList[j].text);
                }
                Debug.Log("Checking Main.Instance...");
                if (Main.Instance == null)
                {
                    Debug.LogError("Main.Instance is NULL!");
                }
                else if (Main.Instance.web == null)
                {
                    Debug.LogError("Main.Instance.web is NULL!");
                }
                else
                {
                    Debug.Log("Main.Instance.web is available.");
                }

                Debug.Log("Checking UI elements...");
                if (NameList == null || NameList.Length == 0 || NameList[0] == null)
                {
                    Debug.LogError("NameList[0] is NULL!");
                }
                else
                {
                    Debug.Log("NameList[0] is: " + NameList[0].text);
                }

                if (WaveList == null || WaveList.Length == 0 || WaveList[0] == null)
                {
                    Debug.LogError("WaveList[0] is NULL!");
                }
                else
                {
                    Debug.Log("WaveList[0] is: " + WaveList[0].text);
                }

                if (ScoreList == null || ScoreList.Length == 0 || ScoreList[0] == null)
                {
                    Debug.LogError("ScoreList[0] is NULL!");
                }
                else
                {
                    Debug.Log("ScoreList[0] is: " + ScoreList[0].text);
                }

                // Only start coroutine if everything is valid
                if (Main.Instance != null && Main.Instance.web != null &&
                    NameList != null && NameList.Length > 0 && NameList[0] != null &&
                    WaveList != null && WaveList.Length > 0 && WaveList[0] != null &&
                    ScoreList != null && ScoreList.Length > 0 && ScoreList[0] != null)
                {
                    yield return StartCoroutine(Main.Instance.web.AddToLeaderboard(
                        NameList[0].text, int.Parse(WaveList[0].text), int.Parse(ScoreList[0].text)));
                    GetLeaderboard(MenuUI);

                }
                else
                {
                    Debug.LogError("Skipping AddToLeaderboard() due to null reference.");
                }



                i += scores.Length;
            }
        }

        lives = 0;
        yield return null;
    }

    // Sets the score text
    private void SetScore(int score)
    {
        this.score = score;
        if (score >= 0)
        {
            scoreText.text = score.ToString().PadLeft(4, '0');
        }
        else
        {
            score *= -1;
            scoreText.text = "-" + score.ToString().PadLeft(4, '0');
            score *= -1;
        }

        SetHighScore(score);
    }

    private void SetHighScore(int score)
    {
        if (HighScore < score)
        {
            HighScore = score;
            HighScoreText.text = HighScore.ToString().PadLeft(4, '0');
        }
    }

    // Sets lives left text
    private void SetLives(int lives)
    {
        this.lives = Mathf.Max(lives, 0);
        LivesText.text = lives.ToString();
    }

    // Remove life, if some left, respawn and restart round, else, send to game over
    private void OnPlayerKilled()
    {
        SetLives(lives - 1);

        player.gameObject.SetActive(false);

        if (lives > 0)
            Invoke(nameof(NewRound), 1f);
        else
            StartCoroutine(GameOver());
    }
    private void GetLeaderboard(GameObject currentActiveScene)
    {
        //1. assign the text to leaderboard
        int childCount = leaderboardObject.transform.childCount;

        LeaderboardScoreList = new Text[childCount];
        LeaderboardWaveList = new Text[childCount];
        LeaderboardNameList = new Text[childCount];
        for (int i = 0; i < childCount; i++)
        {
            Transform entry = leaderboardObject.transform.GetChild(i);
            Text scoreText = entry.Find("Score").GetComponent<Text>();
            Text waveText = entry.Find("Wave").GetComponent<Text>();
            Text nameText = entry.Find("Name").GetComponent<Text>();
            // Ensure components exist before assigning
            if (scoreText != null && waveText != null && nameText != null)
            {
                LeaderboardScoreList[i] = scoreText;
                LeaderboardWaveList[i] = waveText;
                LeaderboardNameList[i] = nameText;
            }
        }
        if (Main.Instance == null)
        {
            Debug.LogError("Main.Instance is null!");
            return;
        }

        if (Main.Instance.web == null)
        {
            Debug.LogError("Main.Instance.web is null!");
            return;
        }

        if (LeaderboardScoreList == null || LeaderboardWaveList == null || LeaderboardNameList == null)
        {
            Debug.LogError("One of the leaderboard lists is null!");
            return;
        }
        //3. populate the arrays with the database values
        Debug.Log("Attempting to get leaderboard...");
        if (LoadingUI != null)
        {
            MenusToggleOff(currentActiveScene);
            MenusToggleOn(LoadingUI);

        }

        StartCoroutine(WaitAndFetchLeaderboard(currentActiveScene));


    }
    private IEnumerator WaitAndFetchLeaderboard(GameObject currentActiveScene)
    {
        yield return StartCoroutine(Main.Instance.web.GetLeaderboard(LeaderboardScoreList, LeaderboardWaveList, LeaderboardNameList));
        MenusToggleOff(LoadingUI);
        MenusToggleOn(MenuUI);

    }
    // When an enemy dies, add score, if all dead, start next round
    private void OnEnemyKilled(Enemy enemy)
    {
        SetScore(score + enemy.score);
        WaveScore += enemy.score;
        audioManager.Play("Destroy");

        if (enemyController.NumberKilled == enemyController.TotalEnemies)
        {
            float WallsHit = enemyController.transform.position.y;
            WallsHit -= enemyController.initialPos.y;

            SetScore(score - (int)WaveScore);

            if (WallsHit == 0)
            {
                WaveScore *= 2;
            }
            else if (WallsHit == -1)
            {
                WaveScore *= 1.9f;
            }
            else if (WallsHit == -2)
            {
                WaveScore *= 1.8f;
            }
            else if (WallsHit == -3)
            {
                WaveScore *= 1.7f;
            }
            else if (WallsHit == -4)
            {
                WaveScore *= 1.6f;
            }
            else if (WallsHit == -5)
            {
                WaveScore *= 1.5f;
            }
            else if (WallsHit == -6)
            {
                WaveScore *= 1.4f;
            }
            else if (WallsHit == -7)
            {
                WaveScore *= 1.3f;
            }
            else if (WallsHit == -8)
            {
                WaveScore *= 1.2f;
            }
            else if (WallsHit == -9)
            {
                WaveScore *= 1.1f;
            }
            else if (WallsHit <= -10)
            {
                WaveScore *= 1;
            }

            SetScore(score + (int)WaveScore);

            if (TintedEnemyChance < 100)
            {
                TintedEnemyChance += 50;
            }
            else
            {
                if (!audioManager.IsPlaying("Level2Music"))
                {
                    audioManager.Stop("Level1Music");
                    audioManager.Play("Level2Music");
                }

                Stage = 2;
                RenderSettings.skybox = NightSkybox;
            }
            Wave++;

            if (Wave == 1)
            {
                totalStage1Score += score;
            }
            else if (Wave == 4)
            {
                totalStage2Score += score;
            }

            totalScore += score;
            totalWaves += 1;

            PlayerPrefs.SetInt("TotalStage1Score", totalStage1Score);
            PlayerPrefs.SetInt("TotalStage2Score", totalStage2Score);
            PlayerPrefs.SetInt("TotalScore", totalScore);
            PlayerPrefs.SetInt("TotalWaves", totalWaves);

            if (totalStage2Score > 0)
            {
                float averageStage1Score = (float)totalStage1Score / (float)GamesPlayed;
                averageStage1Score = (Mathf.Round(averageStage1Score * 100) / 100.0f);

                float averageStage2Score = (float)totalStage2Score / (float)GamesPlayed;
                averageStage2Score = (Mathf.Round(averageStage2Score * 100) / 100.0f);

                float averageTotalScore = (float)totalScore / (float)GamesPlayed;
                averageTotalScore = (Mathf.Round(averageTotalScore * 100) / 100.0f);

                float averageWaves = (float)totalWaves / (float)GamesPlayed;
                averageWaves = (Mathf.Round(averageWaves * 100) / 100.0f);

                float average = (((((averageTotalScore / averageWaves) / ((averageStage1Score + averageStage2Score) / 2)) - 1) * 100));
                average = (Mathf.Round(average * 100) / 100.0f);

                StatTrackerStats[2].text = average.ToString() + "%";
            }
            else if (totalStage1Score > 0)
            {
                float averageStage1Score = (float)totalStage1Score / (float)GamesPlayed;
                averageStage1Score = (Mathf.Round(averageStage1Score * 100) / 100.0f);

                float averageTotalScore = (float)totalScore / (float)GamesPlayed;
                averageTotalScore = (Mathf.Round(averageTotalScore * 100) / 100.0f);

                float averageWaves = (float)totalWaves / (float)GamesPlayed;
                averageWaves = (Mathf.Round(averageWaves * 100) / 100.0f);

                float average = (((((averageTotalScore / averageWaves) / averageStage1Score) - 1) * 100));
                average = (Mathf.Round(average * 100) / 100.0f);

                StatTrackerStats[2].text = average.ToString() + "%";
            }

            if (totalScore > 0)
            {
                float average = (float)totalScore / (float)GamesPlayed;
                average = (Mathf.Round(average * 100) / 100.0f);

                StatTrackerStats[1].text = average.ToString();
            }

            if (Wave != 3)
            {
                NewRound();
            }
            else
            {
                Stage2TutorialUI.SetActive(true);
                PauseTimers = true;
            }

            WaveScore = 0;
        }
    }

    private void OnEnemyIncorrectlyHit(Enemy enemy)
    {
        SetScore(score - (enemy.score / 2));
        WaveScore -= (enemy.score / 2);
        audioManager.Play("Shoot3");
    }


    private IEnumerator Countdown()
    {
        float duration = 780f;
        float totalTime = 0;
        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;
            if (lives <= 0)
            {
                yield break;
            }
            if (MenuUI.activeSelf)
            {
                yield break;
            }

            while (PauseTimers)
            {
                yield return null;
            }

            yield return null;
        }
        StartCoroutine(GameOver());
        yield return null;
    }

    private IEnumerator ReturnToMainScreen()
    {
        float duration = 30f;
        float totalTime = 0;
        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;
            if (Input.anyKeyDown)
            {
                AFKCheck = false;
                yield break;
            }
            yield return null;
        }
        if (InGameUI.activeSelf)
        {
            audioManager.Play("MenuMusic");
            audioManager.Stop("Level1Music");
            audioManager.Stop("Level2Music");
        }
        MainMenu();
        AFKCheck = false;
        yield return null;
    }


    private IEnumerator IdleScreen()
    {
        float duration = 30f;
        float totalTime = 0;
        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;
            if (Input.anyKeyDown)
            {
                IdleCheck = false;
                yield break;
            }
            yield return null;
        }

        audioManager.Stop("MenuMusic");
        MenuUI.SetActive(false);
        MenuBG.SetActive(false);
        IdleMenuUI.SetActive(true);
        IdleCheck = false;
        IdleImage.transform.position = InitialIdlePosition;
        yield return null;
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

    public void IncorrectPopUpMessage(Enemy enemy)
    {
        string message = "";

        if (Stage == 1)
        {
            if (enemy.Stage1EnemyType[0] == 1)
            {
                message = "Dispose this item with the \"Compost\" button";
            }
            else if (enemy.Stage1EnemyType[0] == 2)
            {
                message = "Dispose this item with the \"Mixed Recycling\" button";
            }
            else if (enemy.Stage1EnemyType[0] == 3)
            {
                message = "Dispose this item with the \"Electronic Recyling\" button";
            }
            else if (enemy.Stage1EnemyType[0] == 4)
            {
                message = "Dispose this item with the \"Paper recyling\" button";
            }
            else if (enemy.Stage1EnemyType[0] == 5)
            {
                message = "Dispose this item with the \"Campus Initiatives\" button";
            }
            else if (enemy.Stage1EnemyType[0] == 6)
            {
                message = "Dispose this item with the \"Landfill/Trash\" button";
            }
            else if (enemy.Stage1EnemyType[0] == 7)
            {
                message = "Dispose this item with the \"Donation\" button";
            }
            else if (enemy.Stage1EnemyType[0] == 8)
            {
                message = "Dispose this item with the \"Landfill/Trash\" button";
            }

            StartCoroutine(ActivatePopUp(enemy, message));
        }
        else if (Stage == 2)
        {
            if (enemy.Stage2EnemyType[0] == 1)
            {
                message = "Dispose this item with the \"Compost\" button";
            }
            else if (enemy.Stage2EnemyType[0] == 2)
            {
                message = "Dispose this item with the \"Mixed Recycling\" button";
            }
            else if (enemy.Stage2EnemyType[0] == 3)
            {
                message = "Dispose this item with the \"Electronic Recyling\" button";
            }
            else if (enemy.Stage2EnemyType[0] == 4)
            {
                message = "Dispose this item with the \"Paper recyling\" button";
            }
            else if (enemy.Stage2EnemyType[0] == 5)
            {
                message = "Dispose this item with the \"Campus Initiatives\" button";
            }
            else if (enemy.Stage2EnemyType[0] == 6)
            {
                message = "Dispose this item with the \"Landfill/Trash\" button";
            }
            else if (enemy.Stage2EnemyType[0] == 7)
            {
                message = "Dispose this item with the \"Donation\" button";
            }
            else if (enemy.Stage2EnemyType[0] == 8)
            {
                message = "Dispose this item with the \"Landfill/Trash\" button";
            }

            StartCoroutine(ActivatePopUp(enemy, message));
        }
    }

    public void IndividualPopUpMessage(Enemy enemy)
    {
        string message = "";

        if (enemy.PopUpMessageInt == 1)
        {
            message = "You should always clean something that might have any residue with at least a rinse of water before disposal.";
        }
        else if (enemy.PopUpMessageInt == 2)
        {
            message = "Don't forget to put them in the trash, you cannot flush these!";
        }
        else if (enemy.PopUpMessageInt == 3)
        {
            message = "Never put these in your trash, it can start a fire. Put them in electronic recycling points!";
        }
        else if (enemy.PopUpMessageInt == 4)
        {
            message = "These can be recycled as long as you rinse them out first!";
        }
        else if (enemy.PopUpMessageInt == 5)
        {
            message = "Squeeze out moisture and take out any staples or alike before you compost it. And make sure the bag is a natural fiber too!";
        }
        else if (enemy.PopUpMessageInt == 6)
        {
            message = "MOST ramen cups cannot be recycled because they contain Polystyrene type 6 plastic, so double check before you dispose of it!";
        }
        else if (enemy.PopUpMessageInt == 7)
        {
            message = "These bags are often lined with wax so they can't be recycled.";
        }
        else if (enemy.PopUpMessageInt == 8)
        {
            message = "Thin and stretchy plastic has chemicals that can't be separated from the plastic, and therefore cannot be replaced.";
        }

        StartCoroutine(ActivatePopUp(enemy, message));
    }

    public IEnumerator ActivatePopUp(Enemy enemy, string message)
    {
        if (PopUpUI.activeSelf)
        {
            yield break;
        }

        float duration = 5f;
        float shakeDuration = 0.5f;
        float totalTime = 0;
        float magnitude = 10f;
        RectTransform rectTransform = PopUpUI.GetComponent<RectTransform>();
        Vector3 originalPos = rectTransform.anchoredPosition;
        Image popupImage = PopUpUI.GetComponent<Image>();
        Color color = popupImage.color;
        Color flashRed = new Color(199f / 255f, 70f / 255f, 70f / 255f); // #C74646 in Unity RGB

        PopUpUI.SetActive(true);
        PopUpText.text = message;

        while (totalTime <= duration)
        {
            if (totalTime <= shakeDuration)
            {
                float offsetX = Mathf.Sin(Time.time * 50f) * magnitude; // Smooth left-right shake
                                                                        // float offsetY = UnityEngine.Random.Range(-magnitude, magnitude);
                rectTransform.anchoredPosition = originalPos + new Vector3(offsetX, 0, 0);


                float t = Mathf.PingPong(Time.time * magnitude, 1f);
                popupImage.color = Color.Lerp(color, flashRed, t);
            }
            else
            {
                rectTransform.anchoredPosition = originalPos; // Reset position after shake
                popupImage.color = color;
            }



            if (gameOverUI.activeSelf)
            {
                PopUpUI.SetActive(false);
                yield break;
            }
            totalTime += Time.deltaTime;

            yield return null;
        }

        PopUpUI.SetActive(false);
    }

    public IEnumerator SessionDuration()
    {
        float totalTime = 0;

        while (TimerRunning)
        {
            totalTime += Time.deltaTime;
            yield return null;

            while (PauseTimers)
            {
                yield return null;
            }
        }

        TimePlayed += totalTime;

        PlayerPrefs.SetFloat("totalSessionDuration", TimePlayed);

        float average = (float)TimePlayed / (float)GamesPlayed;
        average = (Mathf.Round(average * 100) / 100.0f);

        StatTrackerStats[4].text = average.ToString();
    }
}