using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverAnimation : MonoBehaviour
{
    public Vector3 initialPos { get; private set; }

    public float animationTime;
    public float amountMoved;

    private void Awake()
    {
        initialPos = transform.position;
    }

    private void OnEnable()
    {
        transform.position = initialPos;
        InvokeRepeating(nameof(AnimateGO), animationTime, animationTime);
    }

    private void AnimateGO()
    {
        if (transform.localPosition.y > 200)
        {
            Vector3 tempPos = transform.position;
            tempPos.y -= amountMoved;
            transform.position = tempPos;
        }
        if (transform.localPosition.y < 200)
        {
            Vector3 tempPos = transform.position;
            tempPos.y += amountMoved;
            transform.position = tempPos;
        }
    }
}
