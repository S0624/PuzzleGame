using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    // オブジェクトのカラーの取得
    public Image _cursorImage;
    public Image _backMinImge;
    private Image _backImge;
    // テキストの取得
    public Text _soudText = null;
    // サウンドのvolumeの取得
    public Scrollbar _bgmVol;
    // 背景の取得
    public Sprite[] _backSprite;
    // Start is called before the first frame update
    void Start()
    {
        _backImge = GameObject.Find("Back").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //_backImge.sprite = "";
    }
    // 必要な時にカーソルのカラーを変える
    public void ImageColorChenge(bool color)
    {
        if (color)
        {
            _cursorImage.color = Color.red;
        }
        else
        {
            _cursorImage.color = Color.white;
        }
    }
    // サウンドの番号のテキストを変更する
    public void SoundTextUpdate(int text)
    {
        _soudText.text = text.ToString();
    }
    // サウンドの音量を変更する
    public float BGMVolume()
    {
        return _bgmVol.value;
    }
    // 背景を変更する.
    public void ChengeBack(int backnum)
    {
        _backMinImge.sprite = _backSprite[backnum];
        _backImge.sprite = _backSprite[backnum];
    }

}
