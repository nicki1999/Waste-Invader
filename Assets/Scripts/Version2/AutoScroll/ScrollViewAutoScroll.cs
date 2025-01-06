using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewAutoScroll : MonoBehaviour
{
[SerializeField] private RectTransform _viewportRectTransform;
[SerializeField] private RectTransform _content;
[SerializeField] private float _transitionDuration = 0.2f;
private TransitionHelper _transitionHelper = new TransitionHelper();

void Update (){
    if(_transitionHelper.InProgress == true){
        _transitionHelper.Update();
        _content.transform.localPosition = _transitionHelper.posCurrent;

    }
}

public void HandleOnSelectChange(GameObject gObj){
     // Check if the selected object is the first child
    if (gObj.transform.GetSiblingIndex() == 0)
    {
        // Reset content position to the top
        _content.transform.localPosition = Vector2.zero;
        _transitionHelper.Clear();
        return;
    }

float viewportTopBorderY = GetBorderTopYLocal(_viewportRectTransform.gameObject);
float viewportBottomBorderY = GetBorderBottomYLocal(_viewportRectTransform.gameObject);


//top
float targetTopBorderY = GetBorderTopYRelative(gObj);
float targetTopYWithViewportOffset = targetTopBorderY + viewportTopBorderY;

//bottom
float targetBottomBorderY = GetBorderBottomYRelative(gObj);
float targetBottomYWithViewportOffset = targetBottomBorderY - viewportBottomBorderY;


//top difference
float topDiff = targetTopYWithViewportOffset - viewportTopBorderY;
if(topDiff > 0f){
    MoveContentObjectYByAmount((topDiff*100f) + GetGridLayoutGroup().padding.top);
}


//bottom difference
float bottomDif = targetBottomYWithViewportOffset - viewportBottomBorderY;
if(bottomDif < 0f){
    MoveContentObjectYByAmount((bottomDif * 100f) - GetGridLayoutGroup().padding.bottom);
}
}

private float GetBorderTopYLocal(GameObject gObj){
    Vector3 pos = gObj.transform.localPosition/100f;
    return pos.y;
}


private float GetBorderBottomYLocal(GameObject gObj){
    Vector2 rectSize = gObj.GetComponent<RectTransform>().rect.size *0.01f;
    Vector3 pos = gObj.transform.localPosition/100f;
     pos.y -=rectSize.y;
     return pos.y;
}

private float GetBorderTopYRelative(GameObject gObj){
    float contentY = _content.transform.localPosition.y/100f;
    float targetBorderUpYLocal = GetBorderTopYLocal(gObj);
    float targetBorderUpYRelative = targetBorderUpYLocal + contentY;

    return targetBorderUpYRelative;
}


private float GetBorderBottomYRelative(GameObject gObj){
    float contentY = _content.transform.localPosition.y/100f;
    float targetBorderBottomYLocal = GetBorderBottomYLocal(gObj);
    float targetBorderBottomYRelative = targetBorderBottomYLocal + contentY;
    return targetBorderBottomYRelative;
}

private void MoveContentObjectYByAmount(float amount){
    Vector2 posScrollFrom = _content.transform.localPosition;
    Vector2 posScrollTo = posScrollFrom;
    posScrollTo.y -= amount;
    _transitionHelper.TransitionPositionFromTo(posScrollFrom,posScrollTo,_transitionDuration);
}

private GridLayoutGroup  GetGridLayoutGroup(){
    GridLayoutGroup gridLayoutGroup = _content.GetComponent<GridLayoutGroup>();
    return gridLayoutGroup;
}

//
    private class TransitionHelper {
        private float _duration = 0f;
        private float _timeElapsed = 0f;
        private float _progress = 0f;

        private bool _inProgress = false;

        private Vector2 _posCurrrent;
        private Vector2 _posFrom;
        private Vector2 _poseTo;

        public bool InProgress {
            get => _inProgress;
        }
        public Vector2 posCurrent{
            get => _posCurrrent;
        }
            // Update is called once per frame
    public void Update()
    {
        Tick();
        CalculatePosition();

    }

    public void Clear(){
        _duration = 0f;
        _timeElapsed = 0f;
        _progress = 0f;

        _inProgress = false;
    }

    public void TransitionPositionFromTo(Vector2 posFrom, Vector2 posTo, float duration){
        Clear();
        _posFrom = posFrom;
        _poseTo = posTo;
        _duration = duration;

        _inProgress = true;
    }

        private void CalculatePosition()
        {
           _posCurrrent.x = Mathf.Lerp(_posFrom.x, _poseTo.x, _progress);
           _posCurrrent.y = Mathf.Lerp(_posFrom.y, _poseTo.y, _progress);
        }
//
        private void Tick()
        {
            if(_inProgress == false){
                return;
            }

            _timeElapsed += Time.deltaTime;
            _progress = _timeElapsed / _duration;

            if(_progress > 1f){
                _progress = 1f;
            }
            if(_progress >= 1f){
                TransitionComplete();
            }
        }

        private void TransitionComplete(){
            _inProgress = false;
        }
    }
}
