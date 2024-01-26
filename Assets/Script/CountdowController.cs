using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdowController : MonoBehaviour
{
    // 入れ替える画像の取得
    public Sprite[] _image;
    private GameObject _gameObject;
    public SpriteRenderer _renderer;
    public bool _isGoText = false;
    // タイマー
    private int _timer;
    private int _timerMax = 60;
    // Start is called before the first frame update
    void Start()
    {
        _gameObject = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        _timer++;
        if(_timer > _timerMax)
        {
            _isGoText = true;
            _renderer.sprite = _image[1];
            _timer = 0;
        }
    }
}
