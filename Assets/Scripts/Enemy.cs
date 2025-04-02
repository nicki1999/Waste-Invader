using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int score = 10;
    public int[] Stage1EnemyType;
    public int[] Stage2EnemyType;

    public Sprite[] animationImages;
    public float animationTime = 1.0f;
    private SpriteRenderer spriteRenderer;
    private int animationFrame;

    public System.Action<Enemy> killed;
    public System.Action<Enemy> IncorrectHit;

    public string EnemyName;
    private bool hitCheck;

    public bool BonusHit = false;
    public int[] BonusHitTypes;

    public int PopUpMessageInt = 0;

    public int countWrongHit = 0;

    public int requiresCleaningObject = 0;
    private bool canTrigger = true;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitCheck = false;
    }
    private void Start()
    {
        //InvokeRepeating(nameof(AnimateEnemy), animationTime, animationTime);

    }

    private void AnimateEnemy()
    {
        animationFrame++;
        if (animationFrame >= animationImages.Length)
        {
            animationFrame = 0;
        }

        spriteRenderer.sprite = animationImages[animationFrame];
    }


    // Coroutine to handle the delay
    private IEnumerator TriggerCooldown()
    {
        // Set canTrigger to false so the trigger doesn't happen again immediately
        canTrigger = false;

        // Wait for the specified delay time
        yield return new WaitForSeconds(1.0f);

        // Re-enable triggering after the delay
        canTrigger = true;
    }
    // When hit by a laser, send out death message
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemyController = GetComponentInParent<EnemyController>();

        if (collision.gameObject.layer == LayerMask.NameToLayer("UIContainer") && canTrigger)
        {
            enemyController.NextRow();
            Debug.Log("UI Hit");
            StartCoroutine(TriggerCooldown());
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            Debug.Log("Laser Hit");
            GameManager gameManager = GetComponentInParent<EnemyController>().gameManager;

            if (gameManager.Stage == 1)
            {
                for (int i = 0; i < Stage1EnemyType.Length; i++)
                {
                    if (collision.gameObject.GetComponent<Projectile>().LaserType == Stage1EnemyType[i])
                    {
                        killed?.Invoke(this);
                        hitCheck = true;
                        countWrongHit = 0;
                    }
                }
                if (!hitCheck)
                {
                    IncorrectHit?.Invoke(this);
                    countWrongHit++;
                }
                // Debug.Log("WrongHit: " + countWrongHit);
            }
            else if (gameManager.Stage == 2)
            {
                int LaserType = collision.gameObject.GetComponent<Projectile>().LaserType;

                if (LaserType == 6)
                    LaserType = 8;
                else if (LaserType == 8)
                    LaserType = 6;

                for (int i = 0; i < Stage2EnemyType.Length; i++)
                {
                    if (LaserType == Stage2EnemyType[i])
                    {
                        if (BonusHit)
                        {
                            for (int j = 0; j < BonusHitTypes.Length; j++)
                            {
                                if (LaserType == BonusHitTypes[j])
                                {
                                    score *= 2;
                                }
                            }
                        }

                        killed?.Invoke(this);
                        hitCheck = true;
                        countWrongHit = 0;
                    }
                }
                if (!hitCheck)
                {
                    IncorrectHit?.Invoke(this);
                    countWrongHit++;
                }
            }
        }
    }

}
