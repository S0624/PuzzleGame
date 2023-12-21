﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectScene : MonoBehaviour
{
    // ボタンの処理をするための変数.
    private InputManager _input;
    // オブジェクトの位置の取得.
    public RectTransform _selecCursorImg;
    public RectTransform[] _selectModeImg;
    private Vector3 _imgPos;
    private int _selectNum = 0;
    // 一瞬だけ押したかどうか.
    private InputAction _isNowAction;
    // 最大値
    private int _selectMax = 0;
    // 最小値
    private int _selectMin = 0;

    // Start is called before the first frame update
    void Start()
    {
        _imgPos = _selecCursorImg.transform.position;
        _input = new InputManager();
        _input.Enable();
        _selectMax = _selectModeImg.Length - 1;
    }

    // Update is called once per frame
    void Update()
    {
        // 入力情報の取得.
        _isNowAction =_input.UI.CursorMove;
        Vector2 moveInput = _input.UI.CursorMove.ReadValue<Vector2>();
        // 左右の入力検知.
        if (moveInput.x > 0 && _isNowAction.WasPressedThisFrame())
        {
            _selectNum++;
        }
        else if (moveInput.x < 0 && _isNowAction.WasPressedThisFrame())
        {
            _selectNum--;
        }
        // カーソルの移動制限
        if(_selectNum < _selectMin)
        {
            _selectNum = _selectMax;
        }
        else if(_selectNum > _selectMax)
        {
            _selectNum = _selectMin;
        }
        // 位置の更新.
        _selecCursorImg.position = _selectModeImg[_selectNum].position;
    }

    // 選んでいる番号返す
    public int SelectNum()
    {
        return _selectNum;
    }
}
