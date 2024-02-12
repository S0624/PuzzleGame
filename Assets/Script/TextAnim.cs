using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class TextAnim : MonoBehaviour
{
    public TextMeshProUGUI _text;
    public CursorController _cursor;
    public string[] _textContent;
    private string _textAdd;
    private int _prevNum = -1;
    private bool _isText = false;
    // Start is called before the first frame update
    void Start()
    {
        TextNewLine();
    }

    // Update is called once per frame
    void Update()
    {
        TextContentUpdate();
        _cursor.IsTextUpdateNow(_isText);
        if (_prevNum != _cursor.SelectNum())
        {
            _isText = true;
            TextInit();
            _prevNum = _cursor.SelectNum();
            StartCoroutine(TextDisplayUpdate());
        }
    }
    private void TextInit()
    {
        _textAdd = "";
        _text.text = "";
    }
    // テキストの内容更新
    private void TextContentUpdate()
    {
        _text.text = _textAdd;
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
    // コルーチンを使って、１文字ごと表示する。
    IEnumerator TextDisplayUpdate()
    {
        // 半角スペースで文字を分割する。
        var words = _textContent[_cursor.SelectNum()].Split(' ');
        foreach (var word in words)
        {
            // 0.03秒刻みで１文字ずつ表示する
            _textAdd = _textAdd + word;
            yield return new WaitForSeconds(0.02f);
        }
        _isText = false;

    }
}
