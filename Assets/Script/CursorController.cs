using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    // 方向の指定
    [Header("方向の指定")] public bool _isVertical;
    // 特定の方向だけを参照する.
    [Header("特定の方向の指定")] public bool _isDirection;
    // オブジェクトの位置の取得.
    public GameObject _cursorObject;
    private GameObject _cursor;
    public RectTransform _selecCursorImg;
    public RectTransform[] _selectModeImg;
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
    // シーンが切り替わるときに音を鳴らすためのサウンドの取得
    private SoundManager _soundManager;

    // 画像の大きさ
    private Vector3 _defaultScale = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 _minScale = Vector3.zero;
    private Vector3 _addScale = new Vector3(0.0f, 0.015f, 0.0f);
    // カーソルを回転させる際に使用する
    private int _timer = 0;
    // 待つ時間
    private int _maxTimer = 15;
    // フラグの取得
    private bool _isAction = false;
    // Start is called before the first frame update
    void Start()
    {
        _selectMax = _selectModeImg.Length - 1;
        _input = new InputManager();
        _input.Enable();
        _inputManager = GameObject.Find("InputManager").GetComponent<InputState>();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isDecision) return;
        // カーソルのアニメーション
        CursorAnim();
        // 入力情報の取得.
        _isNowAction = _input.UI.CursorMove;

        Vector2 moveInput = _inputManager.GetInputMoveDate();

        // 選択した方向の入力値を返す.
        var dir = InputDirection(moveInput);
        if (_inputManager.IsMovePressed())
        {
            _inputframe++;
        }
        if (_isNowAction.WasPressedThisFrame())
        {
            _isAction = true;
        }
        //if (_fadeManager)
        //{
        //    _fadeManager._isTest = _isNowAction.WasPressedThisFrame();
        //}
        // 左右の入力検知.
        if (dir > 0)
        {
            if (IsPressKey() || _isAction)
            {
                _isAction = false;
                _soundManager.SEPlay(SoundSEData.Select);
                _selectNum++;
                _inputframe = 0;
            }
        }
        else if (dir < 0)
        {
            if (IsPressKey() || _isAction)
            {
                _isAction = false;
                _soundManager.SEPlay(SoundSEData.Select);
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
        if (_isDirection)
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
        if (_inputframe > 10)
        {
            return true;
        }
        return false;
    }
    // カーソルのアニメーション(もどき)
    private void CursorAnim()
    {
        if (!_cursorObject) return;
        if (!_selecCursorImg)
        {
            _selecCursorImg.transform.localScale = _defaultScale;
        }
        if (_selecCursorImg.localScale.y >= _defaultScale.y)
        {
            if (!IsTimer()) return;
            _addScale = new Vector3(0.0f, -0.015f, 0.0f); ;
        }
        else if (_selecCursorImg.localScale.y <= _minScale.y)
        {
            _addScale = new Vector3(0.0f, 0.015f, 0.0f); ;
        }
        _selecCursorImg.localScale += _addScale;
    }
    // カーソルがすぐ回転しないように少しだけ待つ処理
    private bool IsTimer()
    {
        _timer++;
        if (_timer > _maxTimer)
        {
            _timer = 0;
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
}
