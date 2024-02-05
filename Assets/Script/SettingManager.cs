using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    // 背景の取得
    public Image _backMinImge;
    private SpriteRenderer _backImge;
    // テキストの取得
    public ImageTextUpdate _soundText;
    // select画面の設定画面
    public bool _isSelectSetting = false;
    // サウンドのvolumeの取得
    [Header("ボリューム関係")]
    public Slider[] _soundVol;
    // 背景の取得
    public Sprite[] _backSprite;

    // Start is called before the first frame update
    void Start()
    {
        _backImge = GameObject.Find("BackImage").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //_backImge.sprite = "";

    }
    // サウンドの番号のテキストを変更する
    public void SoundImageUpdate(int image)
    {
        image = image + 1;
        _soundText.UIImageUpdate(image);
    }

    // 音量を変更する
    public void BGMVolumeChenge(float vol)
    {
        _soundVol[0].value = vol;
    }
    // サウンドの音量を変更する
    public float BGMVolume()
    {
        return _soundVol[0].value;
    }
    // 音量を変更する
    public void SEVolumeChenge(float vol)
    {
        _soundVol[1].value = vol;
    }
    // サウンドの音量を変更する
    public float SEVolume()
    {
        return _soundVol[1].value;
    }
    // 背景を変更する.
    public void ChengeBack(int backnum)
    {
        if (_backImge == null) return;
        if(backnum != 0 && backnum != 1)
        {
            _backImge.transform.localScale = new Vector3(3, 3, 3);
        }
        else
        {
            _backImge.transform.localScale = new Vector3(2, 2, 2);
        }
        _backMinImge.sprite = _backSprite[backnum];
        _backImge.sprite = _backSprite[backnum];
    }

}
