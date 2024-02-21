using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// オブジェクトをふわふわさせる用のscript.
public class TextFloating : MonoBehaviour
{
    public bool _isLoop = false;
    private Transform _imgTransform;
    private float _startMyPos;
    //private float _myPos;
    public float _limitPos;
    public float _floatingPos = 0.5f;
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
            _imgTransform.position -= new Vector3(0, _floatingPos, 0);
            if (_imgTransform.position.y < _limitPos)
            {
                _imgTransform.position = new Vector3(_imgTransform.position.x,_limitPos, _imgTransform.position.z);
            }
        }
    }
}
