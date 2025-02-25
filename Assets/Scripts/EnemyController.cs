using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public Enemy[] Stage1Enemies = new Enemy[1];
    public Enemy[] TintedEnemies = new Enemy[1];
    public Enemy[] TintedEnemiesStage1 = new Enemy[1];
    public Enemy[] Stage2Enemies = new Enemy[1];

    public AnimationCurve Speed = new AnimationCurve();
    public Player player;
    public Vector3 direction { get; private set; } = Vector3.right;
    public Vector3 initialPos { get; private set; }
    public System.Action<Enemy> killed;
    public System.Action<Enemy> IncorrectHit;

    public int NumberKilled { get; private set; }
    public int NumberAlive => TotalEnemies - NumberKilled;
    public int TotalEnemies => rows * columns;
    public float PercentKilled => (float)NumberKilled / (float)TotalEnemies;
    private float slowdownFactor = 0.3f;
    private int rows = 0;
    private int columns = 0;

    public float HorizontalSpacing = 2.0f;
    public float VerticalSpacing = 2.0f;

    private bool bottomCheck = false;

    public GameManager gameManager;
    public string targetColorHex = "#BC2727"; // Hex color string to set the skybox color.
    private Color originalColor;    // To store the original color of the skybox.
    private Color targetColor;

    private float worldOffset = 0.0f;
    public GameObject leftButtonContainer;
    public GameObject rightButtonContainer;
    public Canvas canvas;
    // Initial spawn of enemies
    private float currentAspect;
    private float tolerance = 0.05f;
    private float biggerTolerance = 0.1f;

    private void Awake()
    {

        Transform functionButtonContainerLeft = leftButtonContainer.transform.Find("FunctionButtonContainerLeft");
        Transform functionButtonContainerRight = rightButtonContainer.transform.Find("FunctionButtonContainerRight");

        VerticalLayoutGroup leftVerticalLayoutGroup = functionButtonContainerLeft.GetComponent<VerticalLayoutGroup>();
        VerticalLayoutGroup rightVerticalLayoutGroup = functionButtonContainerRight.GetComponent<VerticalLayoutGroup>();

        initialPos = transform.position;
        currentAspect = (float)Screen.width / Screen.height;
        Debug.Log($"Current Aspect Ratio: {currentAspect}");



        if (Mathf.Abs(currentAspect - (16f / 9f)) < tolerance)
        {
            Debug.Log("16:9 Aspect Ratio");
            rows = 3;
            columns = 10;
            leftVerticalLayoutGroup.padding.top = 661;
            rightVerticalLayoutGroup.padding.top = 661;
            leftVerticalLayoutGroup.spacing = 94;
            rightVerticalLayoutGroup.spacing = 94;
        }
        else if (Mathf.Abs(currentAspect - (16f / 10f)) < tolerance)
        {
            Debug.Log("16:10 Aspect Ratio");
            rows = 3;
            columns = 8;
            leftVerticalLayoutGroup.padding.top = 776;
            rightVerticalLayoutGroup.padding.top = 776;
            leftVerticalLayoutGroup.spacing = 94;
            rightVerticalLayoutGroup.spacing = 94;
        }
        else if (Mathf.Abs(currentAspect - (18f / 9f)) < biggerTolerance)
        {
            leftVerticalLayoutGroup.padding.top = 534;
            rightVerticalLayoutGroup.padding.top = 534;
            leftVerticalLayoutGroup.spacing = 71;
            rightVerticalLayoutGroup.spacing = 71;
            rows = 3;
            columns = 10;
        }

        else if (Mathf.Abs(currentAspect - (4f / 3f)) < tolerance)
        {
            Debug.Log("4:3 Aspect Ratio");
            leftVerticalLayoutGroup.padding.top = 968;
            rightVerticalLayoutGroup.padding.top = 968;
            leftVerticalLayoutGroup.spacing = 138;
            rightVerticalLayoutGroup.spacing = 138;
            rows = 3;
            columns = 6;
        }
        else if (Mathf.Abs(currentAspect - (3f / 2f)) < tolerance)
        {
            Debug.Log("3:2 Aspect Ratio");
            leftVerticalLayoutGroup.padding.top = 776;
            rightVerticalLayoutGroup.padding.top = 776;
            leftVerticalLayoutGroup.spacing = 94;
            rightVerticalLayoutGroup.spacing = 94;
            rows = 3;
            columns = 6;
        }
        else if (Mathf.Abs(currentAspect - (19f / 9f)) < tolerance)
        {
            Debug.Log("19:9 Aspect Ratio");
            rows = 3;
            columns = 10;
            leftVerticalLayoutGroup.padding.top = 484;
            rightVerticalLayoutGroup.padding.top = 484;
            leftVerticalLayoutGroup.spacing = 64;
            rightVerticalLayoutGroup.spacing = 64;
        }
        else if (Mathf.Abs(currentAspect - (20f / 9f)) < biggerTolerance)
        {
            Debug.Log("20:9 Aspect Ratio");
            rows = 3;
            columns = 10;
            leftVerticalLayoutGroup.padding.top = 438;
            rightVerticalLayoutGroup.padding.top = 438;
            leftVerticalLayoutGroup.spacing = 55;
            rightVerticalLayoutGroup.spacing = 55;
        }
        else if (Mathf.Abs(currentAspect - (19.5f / 9f)) < biggerTolerance)
        {
            Debug.Log("19.5:9 Aspect Ratio");
            rows = 3;
            columns = 10;
            leftVerticalLayoutGroup.padding.top = 456;
            rightVerticalLayoutGroup.padding.top = 456;
            leftVerticalLayoutGroup.spacing = 58;
            rightVerticalLayoutGroup.spacing = 58;
        }


        else
        {
            Debug.Log("Interpolated for unknown ratios");
            rows = 3;
            columns = 7;
        }

    }


    private void RandomizeEnemies()
    {


        for (int i = 0; i < rows; i++)
        {
            Vector2 centerOffset = new Vector2((-HorizontalSpacing * (columns - 1)) * 0.5f, (-VerticalSpacing * (rows - 1)) * 0.5f);
            Vector3 rowPosition = new Vector3(centerOffset.x, (VerticalSpacing * i) + centerOffset.y, 0f);

            for (int j = 0; j < columns; j++)
            {
                Enemy enemy = new Enemy();
                if (gameManager.Stage == 1)
                {
                    if (Random.Range(1, 101) <= gameManager.TintedEnemyChance)
                    {
                        enemy = Instantiate(TintedEnemiesStage1[Random.Range(0, TintedEnemiesStage1.Length)], transform);
                        // while (enemy.Stage1EnemyType[0] == 3 || enemy.Stage1EnemyType[0] == 5 || enemy.Stage1EnemyType[0] == 7 || enemy.Stage1EnemyType[0] == 8)
                        // {
                        //     enemy = Instantiate(TintedEnemies[Random.Range(0, TintedEnemies.Length)], transform);
                        // }
                        // Color enemyColor = new Color();
                        // if (enemy.Stage1EnemyType[0] == 1)
                        // {
                        //     enemyColor = new Color(255f / 255f, 0f / 255f, 0f / 255f, 1);
                        // }
                        // else if (enemy.Stage1EnemyType[0] == 2)
                        // {
                        //     enemyColor = new Color(255f / 255f, 153f / 255f, 0f / 255f, 1);
                        // }
                        // else if (enemy.Stage1EnemyType[0] == 3)
                        // {
                        //     enemyColor = new Color(255f / 255f, 255f / 255f, 0f / 255f, 1);
                        // }
                        // else if (enemy.Stage1EnemyType[0] == 4)
                        // {
                        //     enemyColor = new Color(0f / 255f, 255f / 255f, 0f / 255f, 1);
                        // }
                        // else if (enemy.Stage1EnemyType[0] == 5)
                        // {
                        //     enemyColor = new Color(74f / 255f, 134f / 255f, 232f / 255f, 1);
                        // }
                        // else if (enemy.Stage1EnemyType[0] == 6)
                        // {
                        //     enemyColor = new Color(153f / 255f, 0f / 255f, 255f / 255f, 1);
                        //     enemy.GetComponent<Enemy>().Stage1EnemyType[0] = 8;
                        // }
                        // else if (enemy.Stage1EnemyType[0] == 7)
                        // {
                        //     enemyColor = new Color(232f / 255f, 202f / 255f, 142f / 255f, 1);
                        // }
                        // else if (enemy.Stage1EnemyType[0] == 8)
                        // {
                        //     enemyColor = new Color(1f, 1f, 1f, 1);
                        // }

                        // enemy.GetComponent<SpriteRenderer>().color = enemyColor;
                    }
                    else
                    {
                        enemy = Instantiate(Stage1Enemies[Random.Range(0, Stage1Enemies.Length)], transform);
                    }
                }
                else if (gameManager.Stage == 2)
                {
                    enemy = Instantiate(Stage2Enemies[Random.Range(0, Stage2Enemies.Length)], transform);
                }
                enemy.killed += EnemyDeath;
                enemy.IncorrectHit += IncorrectlyHit;

                Vector3 pos = rowPosition;
                pos.x += HorizontalSpacing * j;
                enemy.transform.localPosition = pos;
            }
        }
    }

    // Update speed based on # of enemies left, and move enemies, if any hitting the wall, move down, if any below map, restart
    private void Update()
    {

        float speed = Speed.Evaluate(PercentKilled) * slowdownFactor;
        transform.position += direction * speed * Time.deltaTime;

        foreach (Transform enemy in transform)
        {
            if (!enemy.gameObject.activeInHierarchy)
                continue;

            Enemy enemyTransform = enemy.GetComponent<Enemy>();

            if (enemyTransform == null)
            {
                continue;
            }
            if (enemyTransform.countWrongHit >= 2)
            {
                if (gameManager.DaySkybox != null && gameManager.Stage == 1)
                {
                    originalColor = gameManager.DaySkybox.GetColor("_Tint");
                    if (ColorUtility.TryParseHtmlString(targetColorHex, out targetColor))
                    {
                        // Start the coroutine to change the color to the target color for 2 seconds.
                        StartCoroutine(ChangeSkyboxColorTemporarily(targetColor, 0.5f, gameManager.DaySkybox));
                    }
                }
                else if (gameManager.NightSkybox != null && gameManager.Stage == 2)
                {
                    originalColor = gameManager.NightSkybox.GetColor("_Tint");
                    if (ColorUtility.TryParseHtmlString(targetColorHex, out targetColor))
                    {
                        // Start the coroutine to change the color to the target color for 2 seconds.
                        StartCoroutine(ChangeSkyboxColorTemporarily(targetColor, 0.5f, gameManager.NightSkybox));
                    }
                }
                enemyTransform.countWrongHit = 0;
                // Call any Game Over method here, e.g., gameManager.GameOver();
                break;
            }

            //for adding the function buttons I aded more padding to the left and right
            // if (direction == Vector3.right && enemy.position.x >= (Camera.main.ViewportToWorldPoint(Vector3.right).x))
            // {
            //     NextRow();
            //     break;
            // }
            // else if (direction == Vector3.left && enemy.position.x <= (Camera.main.ViewportToWorldPoint(Vector3.zero).x))
            // {
            //     NextRow();
            //     break;
            // }

            if (!bottomCheck && enemy.position.y <= player.transform.position.y)
            {
                bottomCheck = true;

                if (player.killed != null)
                    player.killed.Invoke();

                break;
            }
        }
    }
    private IEnumerator ChangeSkyboxColorTemporarily(Color newColor, float duration, Material skybox)
    {
        skybox.SetColor("_Tint", newColor);
        yield return new WaitForSeconds(duration);
        skybox.SetColor("_Tint", originalColor);

    }
    // Move to the next row
    public void NextRow()
    {
        direction = new Vector3(-direction.x, 0f, 0f);

        Vector3 pos = transform.position;
        pos.y -= 1f;
        transform.position = pos;
    }

    // When enemy dies, deactivate it and move up the counter
    private void EnemyDeath(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        NumberKilled++;
        killed(enemy);

        if (enemy.PopUpMessageInt > 0)
        {
            gameManager.IndividualPopUpMessage(enemy);
        }
    }

    private void IncorrectlyHit(Enemy enemy)
    {
        player.IncorrectHit();
        gameManager.IncorrectPopUpMessage(enemy);
        IncorrectHit(enemy);
    }

    // Reset enemies
    public void ResetEnemies()
    {
        NumberKilled = 0;
        direction = Vector3.right;
        transform.position = initialPos;

        foreach (Transform invader in transform)
        {
            Destroy(invader.gameObject);
        }

        RandomizeEnemies();

        bottomCheck = false;
    }

}
