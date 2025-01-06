using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollScript : MonoBehaviour
{

    public float Speed;
    public int StopPoint;
    public bool Scrolling = false;
    public Vector3 StartPos;
    public Scrollbar scrollbar;
    public float scrollbarLast = 0.0f;

    private void OnEnable()
    {
        gameObject.transform.localPosition = StartPos;
        Scrolling = true;
        scrollbar.value = 0.0f;
        scrollbarLast = 0.0f;
    }

    private void Update()
    {
        if (!Scrolling)
        {
            return;
        }

        transform.Translate(Vector3.up * Time.deltaTime * Speed);

        if (gameObject.transform.localPosition.y > StopPoint)
        {
            Scrolling = false;
        }
    }

    private void OnDisable()
    {
        Scrolling = false;
    }

    public void ScrollBarChange(int scrollInt)
    {

        Vector3 temp = new Vector3(0, scrollInt, 0);
        if (scrollbarLast < scrollbar.value)
            transform.position += temp;
        else
            transform.position -= temp;
        scrollbarLast = scrollbar.value;

        if (gameObject.transform.localPosition.y < StartPos.y)
            gameObject.transform.localPosition = StartPos;
    }

}
