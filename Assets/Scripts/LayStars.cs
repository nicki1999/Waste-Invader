using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayStars : MonoBehaviour
{
    public SpriteRenderer star;
    public float starLaySpeed;
    public float starDespawnTime;
    public Player player;
    private bool moving;
    private bool movingLeft;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnStars), starLaySpeed, starLaySpeed);
    }

    private void Update()
    {
        moving = player.moving;
        movingLeft = player.movingLeft;
    }

    private void SpawnStars()
    {
        if (moving)
        {
            GameObject tempStar = Instantiate(star.gameObject, transform);

            Vector3 starSpawn;

            if (movingLeft)
            {
                starSpawn = new Vector3(player.transform.position.x + 0.5f, Random.Range(-10f, -12f), 0f);
            }
            else
            {
                starSpawn = new Vector3(player.transform.position.x - 0.5f, Random.Range(-10f, -12f), 0f);
            }

            tempStar.gameObject.transform.position = starSpawn;
            Destroy(tempStar, starDespawnTime);
        }
    }
}
