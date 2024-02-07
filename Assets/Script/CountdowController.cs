using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
// ゲーム開始時に画像を変える処理をするスクリプト
public class CountdowController : MonoBehaviour
{
    // 入れ替える画像の取得
    public Sprite[] _image;
    private GameObject _gameObject;
    private Vector3 _defaultScale = Vector3.one;
    private Vector3 _stopPos;
    public SpriteRenderer _renderer;
    private bool _isRota = false;
    //public bool _isGoText = false;
    // タイマー
    private int _timer;
    private int _timerMax = 30;
    // HACK テスト実装 スケールの取得
    private bool _isScaleMin = false;
    // サウンドマネージャーの取得
    private SoundManager _soundManager;
    // Start is called before the first frame update
    void Start()
    {
        _gameObject = this.gameObject;
        _stopPos = _gameObject.transform.position;
        _defaultScale = _gameObject.transform.localScale;
        _gameObject.transform.position = new Vector3(_gameObject.transform.position.x, 10, _gameObject.transform.position.z);

        _gameObject.transform.DOMoveY(_stopPos.y, 0.8f).SetEase(Ease.InOutQuad);
        //_gameObject.transform.DORotate(new Vector3(0, 180.0f, 0), 2.0f).SetEase(Ease.InOutQuad);
        //_gameObject.transform.DORotate(new Vector3(0, -10.0f, 0), 2.0f).SetEase(Ease.InOutQuad);

        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _soundManager.SEPlay(SoundSEData.Ready);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsImageSlide()) return;
        ImageRontaUpdate();
        ImageScaleChenge();
        if (IsTimer())
        {
            DisplayImageTimerUpdate();
        }
    }
    // 画像のスライドがおわったかどうか.
    private bool IsImageSlide()
    {
        if (_stopPos.y == _gameObject.transform.position.y)
        {
            return true;
        }
        return false;
    }

    // 画像の回転処理
    private void ImageRontaUpdate()
    {
        if (!_isScaleMin)
        {
            // 画像を回転させる処理
            _gameObject.transform.Rotate(new Vector3(0, 20, 0));
        }

        if (!_isRota)
        {
            // 画像の拡大率をいじる処理
            _gameObject.transform.DOScale(0, 1.0f).SetEase(Ease.InOutSine);
            
            _isRota = true;
        }

    }
    // 画像の拡大処理
    private void ImageScaleChenge()
    {
        if (_gameObject.transform.localScale.x == 0)
        {
            _soundManager.SEPlay(SoundSEData.Go);
            // 画像の大きさをもとに戻す処理.
            _isScaleMin = true;
            // 画像の種類変更
            _renderer.sprite = _image[1];
            _gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            _gameObject.transform.DOScale(_defaultScale, 0.5f).SetEase(Ease.OutBack);
            //_gameObject.transform.localScale = _defaultScale;
        }
    }
    // タイマーを作動させる条件を満たしているかどうかをチェックする
    private bool IsTimer()
    {
        if (_gameObject.transform.localScale != _defaultScale)
        {
            return false;
        }
        return true;
    }
    // 表示させる時間を経過させる処理
    private void DisplayImageTimerUpdate()
    {
        _timer++;
        if (_timer > _timerMax)
        {
            _timer = 0;
            Destroy(_gameObject);
        }
    }

}
