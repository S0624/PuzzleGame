using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    // オブジェクトのカラーの取得
    public Image _cursorImage;
    public Image _backImge;
    // テキストの取得
    public Text _soudText = null;
    // サウンドのvolumeの取得
    public Scrollbar _bgmVol;
    // Start is called before the first frame update
    void Start()
    {
        
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
    public float BGMVolume()
    {
        return _bgmVol.value;
    }
}
