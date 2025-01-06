using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Enemy[] Stage1Enemies = new Enemy[1];
    public Enemy[] TintedEnemies = new Enemy[1];
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
    public int rows = 5;
    public int columns = 11;

    public float HorizontalSpacing = 2.0f;
    public float VerticalSpacing = 2.0f;

    private bool bottomCheck = false;

    public GameManager gameManager;

    // Initial spawn of enemies
    private void Awake()
    {
        initialPos = transform.position;
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
                        enemy = Instantiate(TintedEnemies[Random.Range(0, TintedEnemies.Length)], transform);
                        Color enemyColor = new Color();
                        if (enemy.Stage1EnemyType[0] == 1)
                        {
                            enemyColor = new Color(255f / 255f, 0f / 255f, 0f / 255f, 1);
                        }
                        else if (enemy.Stage1EnemyType[0] == 2)
                        {
                            enemyColor = new Color(255f / 255f, 153f / 255f, 0f / 255f, 1);
                        }
                        else if (enemy.Stage1EnemyType[0] == 3)
                        {
                            enemyColor = new Color(255f / 255f, 255f / 255f, 0f / 255f, 1);
                        }
                        else if (enemy.Stage1EnemyType[0] == 4)
                        {
                            enemyColor = new Color(0f / 255f, 255f / 255f, 0f / 255f, 1);
                        }
                        else if (enemy.Stage1EnemyType[0] == 5)
                        {
                            enemyColor = new Color(74f / 255f, 134f / 255f, 232f / 255f, 1);
                        }
                        else if (enemy.Stage1EnemyType[0] == 6)
                        {
                            enemyColor = new Color(153f / 255f, 0f / 255f, 255f / 255f, 1);
                            enemy.GetComponent<Enemy>().Stage1EnemyType[0] = 8;
                        }
                        else if (enemy.Stage1EnemyType[0] == 7)
                        {
                            enemyColor = new Color(232f / 255f, 202f / 255f, 142f / 255f, 1);
                        }
                        else if (enemy.Stage1EnemyType[0] == 8)
                        {
                            enemyColor = new Color(1f, 1f, 1f, 1);
                        }

                        enemy.GetComponent<SpriteRenderer>().color = enemyColor;
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
            //for adding the function buttons I aded more padding to the left and right
            if (direction == Vector3.right && enemy.position.x >= (Camera.main.ViewportToWorldPoint(Vector3.right).x - 6f))
            {
                NextRow();
                break;
            }
            else if (direction == Vector3.left && enemy.position.x <= (Camera.main.ViewportToWorldPoint(Vector3.zero).x + 6f))
            {
                NextRow();
                break;
            }

            if (!bottomCheck && enemy.position.y <= player.transform.position.y)
            {
                bottomCheck = true;

                if (player.killed != null)
                    player.killed.Invoke();

                break;
            }
        }
    }

    // Move to the next row
    private void NextRow()
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
