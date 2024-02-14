using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class DifficultyManager : MonoBehaviour
{
    // 難易度
    static public int _difficulty;
    public CursorController _cursor;
    // ボタンの処理をするための変数.
    public InputManager[] _input;
    // 押したかどうかのフラグ
    public bool _isPushButton = false;
    public bool _isButton = false;
    // カラーマネージャーの取得
    public ColorSeedCreate _color;
    // カラーの数
    private int _colorNum = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.localScale.x != 1.0f) return;
        for (int i = 0; i < _input.Length; i++)
        {
            if (_input[i].UI.Submit.WasPerformedThisFrame())
            {
                _isPushButton = true;
                _cursor.Decision(_isPushButton);
            }
            else if (_input[i].UI.Cancel.WasPerformedThisFrame())
            {
                _isButton = true;
                _cursor.Decision(false);
            }
        }
        DifficultyUpdate();
    }
    private void DifficultyUpdate()
    {
        if(_cursor.SelectNum() == 0)
        {
            _colorNum = 2;
        }
        else if(_cursor.SelectNum() == 1)
        {
            _colorNum = 1;
        }
        else if(_cursor.SelectNum() == 2)
        {
            _colorNum = 0;
        }
        // カラーの数を渡す
        _color.ColorPreparation(_colorNum);
    }
}
