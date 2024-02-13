using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputController : MonoBehaviour
{
    // 方向の指定
    [Header("方向の指定")] public bool _isVertical;
    // 特定の方向だけを参照する.
    [Header("特定の方向の指定")] public bool _isDirection;

    // ボタンの処理をするための変数.
    private InputState _inputManager;
    private InputManager _input;
    private int _selectNum = 0;
    // 一瞬だけ押したかどうか.
    private InputAction _isNowAction;
    // 押し続けた時の処理
    private int _inputframe = 0;
    // 決定したかどうか
    private bool _isDecision = false;
    // 最大値
    private int _selectMax = 0;
    // 最小値
    private int _selectMin = 0;

    // Start is called before the first frame update
    void Start()
    {
        _input = new InputManager();
        _input.Enable();
        _inputManager = GameObject.Find("InputManager").GetComponent<InputState>();
    }
    public void InputInit(int max,int min)
    {
        _selectMax = max;
        _selectMin = min;
    }

    public void InputUpdate()
    {
        // 入力情報の取得.
        _isNowAction = _input.UI.CursorMove;
        Vector2 moveInput = _inputManager.GetInputMoveDate();

        // 選択した方向の入力値を返す.
        var dir = InputDirection(moveInput);
        if (_inputManager.IsMovePressed())
        {
            _inputframe++;
        }
        // 左右の入力検知.
        if (dir > 0)
        {
            if (IsPressKey() || _isNowAction.WasPressedThisFrame())
            {
                _selectNum++;
                _inputframe = 0;
            }
        }
        else if (dir < 0)
        {
            if (IsPressKey() || _isNowAction.WasPressedThisFrame())
            {
                _selectNum--;
                _inputframe = 0;
            }
        }
        // カーソルの移動制限
        if (_selectNum < _selectMin)
        {
            _selectNum = _selectMax;
        }
        else if (_selectNum > _selectMax)
        {
            _selectNum = _selectMin;
        }

    }
    //  キーの入力状態.
    public bool IsPressKey()
    {
        if (_inputframe > 15)
        {
            return true;
        }
        return false;
    }
    
    // 選んでいる番号返す
    public int SelectNum()
    {
        return _selectNum;
    }
    // ボタンを押したかどうか
    public void Decision(bool push)
    {
        _isDecision = push;
    }
    // 選択した方向の入力値を返す.
    private float InputDirection(Vector2 input)
    {
        //縦にチェックが入っていたらy方向を返す.
        if (_isVertical)
        {
            return input.y * -1;
        }
        // そうじゃなかったら横に移動なのでxを返す.
        else
        {
            return input.x;
        }
    }
    // 決定
    public void InputDecision()
    {
        _input.UI.Submit.WasPerformedThisFrame();
    }    
    // 戻る
    public void InputCancel()
    {
        _input.UI.Cancel.WasPerformedThisFrame();
    }
    // ポーズ
    public void InputPause()
    {
        _input.UI.Pause.WasPerformedThisFrame();
    }


}
