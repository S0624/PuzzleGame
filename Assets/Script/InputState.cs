using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputState : MonoBehaviour
{
    private InputManager[] _inputManager;
    private int _index = 0;
    private Vector2[] _move;
    // ボタンの処理が行われているかどうかの変数
    private bool[] _isPress;
    private bool[,] _rota;

    private bool [] _fallDown = new bool[2];
    void Start()
    {
        // ゲームパッドの取得.
        InputInit();
    }

    private void InputInit()
    {
        // ゲームパッドの取得.
        _inputManager = Gamepad.all.Select(pad =>
        {
            var control = new InputManager();
            control.devices = new UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputDevice>(new[] { pad });
            control.Enable();
            return control;
        }).ToArray();
        _move = new Vector2[_inputManager.Length];
        _isPress = new bool[_inputManager.Length];
        _rota = new bool[_inputManager.Length, (int)RotaState.max];
    }
    void Update()
    {
        InputStateUpdate();
        //IsInputIndexCheck();
    }
    private bool IsInputIndexCheck()
    {
        if(_index < 0) 
        {
            return false;
        }
        return true;
    }
    // ゲームパッド(入力)のUpdate処理.
    public Vector2 InputStateUpdate()
    {
        // 何番のパッドの処理を行ったか
        int index = 0;
        foreach (var control in _inputManager)
        {
            // 移動量の取得.
            var move = control.Piece.Move.ReadValue<Vector2>();
            // キーを押しっぱなしかどうかのの取得.
            var isPress = control.Piece.Move.IsPressed();
            // 移動キーの処理.
            SetInputMoveDate(index, move,isPress);
            // 回転キーの処理.
            RotationStateUpdate(control, index);

            // デバック用
            _fallDown [index] = _inputManager[_index].Piece.Move.WasPressedThisFrame();
            ++index;
        }
        return Vector2.zero;
    }
    // 回転の状態のUpdate処理.
    private void RotationStateUpdate(InputManager control,int index)
    {
        // 右.
        if(control.Piece.RotationRight.WasPerformedThisFrame())
        {
            SetInputRotaDate(index, RotaState.right,true);
        }
        else
        {
            SetInputRotaDate(index, RotaState.right,false);
        }
        // 左.
        if(control.Piece.RotationLeft.WasPerformedThisFrame())
        {
            SetInputRotaDate(index, RotaState.left,true);
        }
        else
        {
            SetInputRotaDate(index, RotaState.left, false);
        }
    }
    // 何Pのパッドが何をしたかを記憶する.
    private void SetInputMoveDate(int index, Vector2 move, bool isPress)
    {
        //_index = index;
        _move[index] = move;
        _isPress[index] = isPress;
    }
    // 何Pのパッドが回転をしたかを記憶する.
    private void SetInputRotaDate(int index, RotaState state,bool isRota)
    {
        _index = index;
        _rota[index,(int)state] = isRota;
    }
    // プレイヤーが触っているパッドの番号を取得する.
    public void GetInputPlayerPadNum(int playerIndex = 0)
    {
        _index = playerIndex;
    }
    // 移動した量を返す.
    public Vector2 GetInputMoveDate(bool isSelect = false,int index = 0 , int indexMax = 0)
    {
        if(isSelect)
        {
            if(_move.Length != indexMax)
            {
                InputInit();
            }
            return _move[index];
        }
        return _move[_index];
    }
    // 移動キーを押しっぱなしかどうかを検知する.
    public bool IsMovePressed()
    {
        return _isPress[_index];
    }
    // 回転の値を返す.
    public bool GetInputRotaDate(RotaState rota)
    {
        return _rota[_index, (int)rota];
    }
#if true
    // デバック実装.
    public bool DGetInputWasPressData()
    {
        return _fallDown[_index];
    }
#endif
}

