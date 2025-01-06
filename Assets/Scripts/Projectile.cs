using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 30f;
    public int LaserType = 0;
    public Vector3 direction = Vector3.up;
    public System.Action<Projectile> destroyed;

    // When destroyed, send out destroyed message
    private void OnDestroy()
    {
        if (destroyed != null)
            destroyed.Invoke(this);
    }

    // Move in direction at speed
    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    // When hit something, destroy self
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
