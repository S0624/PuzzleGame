using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class DemoMoveScene : MonoBehaviour
{
    // ビデオの取得
    public VideoPlayer _video;
    // 画像の取得
    public Image _demoImage;
    
    // 変更するカラー値
    private float _chengeColor = 1;
    // 時間
    private double _time;
    // フェード値の取得
    public LoadSceneManager _loadSceneManager;
    // 再生したかどうかのフラグ
    private bool _isPlaying = false;
    // Start is called before the first frame update
    void Start()
    {
        //_video.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (_loadSceneManager.FadeNumericalValue() != 0.0f && !_isPlaying)
        {
            return;
        }
        if (!_isPlaying)
        {
            _video.Play();
            _isPlaying = true;
        }
        Debug.Log(_video.isPlaying);
        if (!_video.isPlaying)
        {
            _loadSceneManager.DemoMoveSceneChenge();
        }
        // カラーの変更処理
        ColorChengeUpdate();
    }
    // カラーのチェンジ
    private void ColorChengeUpdate()
    {
        // 内部時刻を経過させる
        _time += Time.deltaTime;

        // 周期cycleで繰り返す波のアルファ値計算
        var alpha = Mathf.Cos((float)(2 * Mathf.PI * _time / _chengeColor)) * 0.5f + 0.5f;

        // 内部時刻timeにおけるアルファ値を反映
        var color = _demoImage.color;
        color.a = alpha;
        _demoImage.color = color;
    }
}
