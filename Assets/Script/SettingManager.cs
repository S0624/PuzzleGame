﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    // オブジェクトの取得
    public Image _cursorImage;
    public GameObject _subCursor;
    public Image _subCursorImage;
    public Image _backMinImge;
    private Image _backImge;
    // テキストの取得
    public Text _soudText = null;
    // サウンドのvolumeの取得
    [Header("ボリューム関係")]
    public Slider[] _soundVol;
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
        text = text + 1;
        _soudText.text = text.ToString();
    }
    // サブカーソルを表示させるかどうか
    public void SubCursorDisplay(bool active = false)
    {
        _subCursor.SetActive(active);
    }
    // サブカーソルの更新処理.
    public bool SubCursorUpdate(bool color)
    {
        _subCursor.GetComponent<CursorController>().Decision(color);
        if (color)
        {
            _subCursorImage.color = Color.red;
        }
        else
        {
            _subCursorImage.color = Color.white;
        }
        return color;
    }
    public int SubCursorNum()
    {
        return _subCursor.GetComponent<CursorController>().SelectNum();
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
        _backMinImge.sprite = _backSprite[backnum];
        _backImge.sprite = _backSprite[backnum];
    }

}
