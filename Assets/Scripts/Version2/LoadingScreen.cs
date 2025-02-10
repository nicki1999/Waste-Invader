using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public Transform[] sprites;
    public float radius = 3f;
    public float speed = 2f;

    private float angleOffset;

    void Start()
    {
        angleOffset = 360f / sprites.Length;
    }

    void Update()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            float angle = (Time.time * speed * Mathf.Rad2Deg) + (i * angleOffset);
            float radians = angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(radians) * radius;
            float y = Mathf.Sin(radians) * radius;

            sprites[i].position = new Vector3(x, y, 0);
        }
    }

}

