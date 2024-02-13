using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class TextAnim : MonoBehaviour
{
    public TextMeshProUGUI[] _text;
    public CursorController _cursor;
    public string[] _textContent;
    private string[] _textAdd;
    private int _prevNum = -1;
    // 時間の計測
    private float _timer = 0.01f;
    // 文字数のカウント
    int _count = 0;
    // Start is called before the first frame update
    void Start()
    {
        _textAdd = new string[_text.Length];
        TextNewLine();
    }

    // Update is called once per frame
    void Update()
    {
        TextContentUpdate();
        if (_prevNum != _cursor.SelectNum())
        {
            TextInit();
            _prevNum = _cursor.SelectNum();
        }
        TextDisplayUpdate();
    }
    // 初期化
    private void TextInit()
    {
        for (int i = 0; i < _text.Length; i++)
        {
            _textAdd[i] = "";
            _text[i].text = "";
        }
        _count = 0;
    }
    // テキストの内容更新
    private void TextContentUpdate()
    {
        _text[_cursor.SelectNum()].text = _textAdd[_cursor.SelectNum()];
    }
    // 改行処理
    private void TextNewLine()
    {
        for (int i = 0; i < _textContent.Length; i++)
        {
            // 改行する文字が含まれていたら
            if (_textContent[i].Contains("\\n"))
            {
                // その場所で開業を行う
                _textContent[i] = _textContent[i].Replace(@"\n", Environment.NewLine);
            }
        }
    }
    // １文字ごと表示する。
    private void TextDisplayUpdate()
    {
        // 半角スペースで文字を分割する。
        var words = _textContent[_cursor.SelectNum()].Split(' ');
        if (words.Length - 1 == _count) return;
        _timer += 0.02f;
        if (_timer > 0.03f)
        {
            // 0.03秒刻みで１文字ずつ表示する
            _textAdd[_cursor.SelectNum()] = _textAdd[_cursor.SelectNum()] + words[_count];
            _count++;
            _timer = 0;
        }

    }
}
