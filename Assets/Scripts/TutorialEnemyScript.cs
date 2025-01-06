using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyScript : MonoBehaviour
{
    public int score = 10;
    public int Stage1EnemyType = 0;
    public int Stage2EnemyType = 0;

    public Sprite[] animationImages;
    public float animationTime = 1.0f;
    private SpriteRenderer spriteRenderer;
    private int animationFrame;

    public System.Action<Enemy> killed;
    public System.Action<Enemy> IncorrectHit;

    public string EnemyName;

    public TutorialScript tutorialScript;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tutorialScript = GetComponentInParent<TutorialScript>();
    }

    // When hit by a laser, send out death message
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            if (collision.gameObject.GetComponent<Projectile>().LaserType == Stage1EnemyType)
            {
                if (tutorialScript.TutorialStage == -1)
                {
                    spriteRenderer.sprite = animationImages[animationFrame];
                }
            }
        }
    }
}
