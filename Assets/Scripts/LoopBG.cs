using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopBG : MonoBehaviour
{
    public Vector3 startPos = new Vector3(0, 0, 0);
    public float loopX;
    public float loopY;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * 0.5f * Time.deltaTime;
        transform.position += Vector3.down * 0.5f * Time.deltaTime;

        if (gameObject.transform.position.x <= loopX)
        {
            Vector3 tempPos = startPos;
            tempPos.y = gameObject.transform.position.y;
            gameObject.transform.position = tempPos;
        }

        if (gameObject.transform.position.y <= loopY)
        {
            Vector3 tempPos = startPos;
            tempPos.x = gameObject.transform.position.x;
            gameObject.transform.position = tempPos;
        }
    }
}
