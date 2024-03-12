using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class DifficultyManager : MonoBehaviour
{
    public RectTransform _selecCursorImg;
    public RectTransform[] _selectModeImg;
    // 難易度
    static public int _difficulty;
    public DifficultyController _cursor;
    // ボタンの処理をするための変数.
    public InputManager _input;
    // 押したかどうかのフラグ
    public bool _isPushButton = false;
    public bool _isButton = false;
    // カラーの数
    private int _colorNum = 0;
    // 一瞬だけ押したかどうか.
    private InputAction _isNowAction;
    // 最大値
    private int _selectMax = 0;
    // 最小値
    private int _selectMin = 0;
    // 選んでいる番号
    public int _selectNum = 0;
    // 決定したかどうか
    private bool _isDecision = false;
    // シーンが切り替わるときに音を鳴らすためのサウンドの取得
    private SoundManager _soundManager;
    // 押し続けた時の処理
    private int _inputframe = 0;
    // カラーマネージャーの取得
    public ColorSeedCreate _color;
    // Start is called before the first frame update
    void Start()
    {
        _input = new InputManager();
        _input.Enable();
        _selectMax = _selectModeImg.Length - 1;
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.localScale.x != 1.0f) return;
        if (_isDecision) return;
        ButtonMove();
        // カーソルの移動制限
        if (_selectNum < _selectMin)
        {
            _selectNum = _selectMax;
        }
        else if (_selectNum > _selectMax)
        {
            _selectNum = _selectMin;
        }
        // ボタンの押した処理
        ButtonPushUpdate();
        DifficultyUpdate();
        ImageUpdate();
    }
    private void ButtonMove()
    {
        // 入力情報の取得.
        _isNowAction = _input.UI.CursorMove;
        Vector2 moveInput = _input.UI.CursorMove.ReadValue<Vector2>();
        if (_isNowAction.IsPressed())
        {
            _inputframe++;
        }
        // 左右の入力検知.
        if (moveInput.y < 0)
        {
            if (IsPressKey() || _isNowAction.WasPressedThisFrame())
            {
                _soundManager.SEPlay(SoundSEData.Select);
                _selectNum++;
                _inputframe = 0;
            }
        }
        else if (moveInput.y > 0)
        {
            if (IsPressKey() || _isNowAction.WasPressedThisFrame())
            {
                _soundManager.SEPlay(SoundSEData.Select);
                _selectNum--;
                _inputframe = 0;
            }
        }

    }
    private void ButtonPushUpdate()
    {
        if (_input.UI.Submit.WasPerformedThisFrame())
        {
            _isPushButton = true;
            _isDecision = true;
        }
        else if (_input.UI.Cancel.WasPerformedThisFrame())
        {
            _isButton = true;
            _isDecision = false;
        }
    }
    private void ImageUpdate()
    {
        if (_isDecision)
        {
            _selecCursorImg.position = new Vector3(_selecCursorImg.position.x, _selectModeImg[_selectNum].position.y, _selecCursorImg.position.z);
        }
        else
        {
            // 位置の更新.
            _selecCursorImg.position = _selectModeImg[_selectNum].position;
        }
    }
    //  キーの入力状態.
    private bool IsPressKey()
    {
        if (_inputframe > 15)
        {
            return true;
        }
        return false;
    }
    private void DifficultyUpdate()
    {
        if(_selectNum == 0)
        {
            _colorNum = 2;
        }
        else if(_selectNum == 1)
        {
            _colorNum = 1;
        }
        else if(_selectNum == 2)
        {
            _colorNum = 0;
        }
        // カラーの数を渡す
        _color.ColorPreparation(_colorNum);
    }
}
