using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
// オブジェクトをふわふわさせる用のscript.
public class TextFloating : MonoBehaviour
{
    public bool _isLoop = false;
    private Transform _imgTransform;
    private float _startMyPos;
    //private float _myPos;
    public float _limitPos;
    public float _floatingPos = 0.5f;
    private bool _isMove = false;
    // Start is called before the first frame update
    void Start()
    {
        _imgTransform = GetComponent<Transform>();
        _startMyPos = _imgTransform.position.y;
        
        //_myPos = _textTransform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isLoop)
        {
            if (_startMyPos - _limitPos > _imgTransform.position.y)
            {
                _floatingPos *= -1;
            }
            if (_startMyPos + _limitPos < _imgTransform.position.y)
            {
                _floatingPos *= -1;
            }
            _imgTransform.position += new Vector3(0, _floatingPos, 0);
        }
        else
        {
            if (_isMove) return;
            _imgTransform.transform.DOMoveY(_startMyPos - _limitPos, 2.5f).SetEase(Ease.InOutQuad);
            _isMove = true;
        }
    }
}
