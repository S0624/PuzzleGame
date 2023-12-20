using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectScene : MonoBehaviour
{
    // ボタンの処理をするための変数.
    private InputManager _input;
    // オブジェクトの位置の取得.
    public RectTransform _selectImg;
    private Vector3 _imgPos;
    private int _selectNum = 1;
    // 一瞬だけ押したかどうか.
    private bool _isAction = false;
    // 最大値
    private int _selectMac = 3;
    // 最小値
    private int _selectMin = 1;

    // Start is called before the first frame update
    void Start()
    {
        _imgPos = _selectImg.transform.position;
        _input = new InputManager();
        _input.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // 入力情報の取得.
        _isAction =_input.UI.CursorMove.WasPerformedThisFrame();
        Vector2 moveInput = _input.UI.CursorMove.ReadValue<Vector2>();
        // 左右の入力検知.
        if (moveInput.x > 0 && _isAction)
        {
            _selectNum++;
        }
        else if (moveInput.x < 0 && _isAction)
        {
            _selectNum--;
        }
        // カーソルの移動制限
        if(_selectNum < _selectMin)
        {
            _selectNum = _selectMac;
        }
        else if(_selectNum > _selectMac)
        {
            _selectNum = _selectMin;
        }
        // 位置の更新.
        _selectImg.position = new Vector3(_imgPos.x * _selectNum, _imgPos.y, _imgPos.z);
    }
    public int SelectNum()
    {
        return _selectNum - 1;
    }
}
