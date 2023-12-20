using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
// オブジェクトをふわふわさせる用のscript.
public class TextFloating : MonoBehaviour
{
    private RectTransform _textTransform;
    private float _startMyPos;
    //private float _myPos;
    public int _limitPos;
    public float _floatingPos = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        _textTransform = GetComponent<RectTransform>();
        _startMyPos = _textTransform.position.y;
        //_myPos = _textTransform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        _textTransform.position += new Vector3(0,_floatingPos,0);
        if (_startMyPos - _limitPos > _textTransform.position.y)
        {
            _floatingPos *= -1;
        }
        if (_startMyPos + _limitPos < _textTransform.position.y)
        {
            _floatingPos *= -1;
        }
    }
}
