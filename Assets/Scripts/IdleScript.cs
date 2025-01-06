using UnityEngine;

public class IdleScript : MonoBehaviour
{
    private Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rigidbody.velocity.x == 0)
        {
            float rand = Random.Range(-20, 20);
            if (rand < 0 && rand > -5)
            {
                rand = -5;
            }
            else if (rand > 0 && rand < 5)
            {
                rand = 5;
            }
            rigidbody.velocity = new Vector2(rand, rigidbody.velocity.y);
        }
        else if (rigidbody.velocity.y == 0)
        {
            float rand = Random.Range(-20, 20);
            if (rand < 0 && rand > -5)
            {
                rand = -5;
            }
            else if (rand > 0 && rand < 5)
            {
                rand = 5;
            }
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rand);
        }
    }
}
