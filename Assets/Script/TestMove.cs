using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestMove : MonoBehaviour
{
    // ボタンの処理が行われているかどうかの変数
    private InputAction _action;
    // ボタンの処理をするための変数.
    private TestInputManager _testInput;
   
    // 移動速度.
    //private float _speed;
    //private float currentSpeed;

    // フィールドの情報を受け取るための変数
    private GameObject _fieldObject; 
    // 色の情報を受け取るための変数
    private GameObject _colorManager;
    // 移動情報
    private Vector2Int _cubePos = new Vector2Int(3,14);
    // 仮で戻す位置を覚えておく変数
    private Vector3 _cubePostemp;
    // 時間を図る変数(数秒たつと自動で落下させるために使用)
    private int _timer = 0;
    // ボタンを押し続けているか図る(これがないと一瞬で移動してしまうため)
   private int _inputframe = 0;
    // 色の番号
    private int _colorNum;

    // どの向きに回転させるか（回転処理に使用）
    private Vector2Int[] _cubeDirection = new Vector2Int[(int)Direction.max];
    // 何回ボタンを押されたかを図るために使用する変数
    private int _direction = 0;

    // Start is called before the first frame update
    // 初期化処理
    void Start()
    {

        _testInput = new TestInputManager();
        _testInput.Enable();

        //_speed = 5.0f;
        // フィールドを取得
        _fieldObject = GameObject.Find("Board");
        _colorManager = GameObject.Find("ColorManager");
        //this.transform.position = transform.position + new Vector3(_cubePos.x, _cubePos.y, 0);
        this.transform.position = Vector3.zero;
        // 自分のポジションから
        // これをしないといろんな位置に行ってしまうので、フィールドに合わせてキューブの位置を初期化する
        this.transform.position = transform.position +transform.position + _fieldObject.GetComponent<TestController>().fieldPos(_cubePos);
        
        //this.transform.position = new Vector3(_cubePos.x, _cubePos.y, 0);
        _cubePostemp = this.transform.position;

        //_testField.GetComponent<TestController>().IsSetCube(_testPos,0);
        _cubeDirection[(int)Direction.Down] = new Vector2Int(0, -1);
        _cubeDirection[(int)Direction.Left] = new Vector2Int(-1, 0);
        _cubeDirection[(int)Direction.Up] = new Vector2Int(0, 1);
        _cubeDirection[(int)Direction.Right] = new Vector2Int(1, 0);
    }

    // Update is called once per frame
    void Update()
    {

#if true
        // 方向キーの入力取得
        // 下左右に動かす
        if (!_fieldObject.GetComponent<TestController>().IsCheckField())
        {
            if (!_fieldObject.GetComponent<TestController>().IsGameOver())
            {
                MoveState();
            }
        }
        // HACK 雑に回転処理を実装(お試し)
        if (_testInput.Piece.RotationRight.WasPerformedThisFrame())
        {
            _direction++;
            if( _direction >= 4 )
            {
                _direction = 0;
            }
            // HACK 回転処理 ﾋｨ.........
            CubeRotation();
        }

        Vector2 moveInput = this._testInput.Piece.Move.ReadValue<Vector2>();
        // 急降下で下に落とす(DEBUG機能).
        if (moveInput.y > 0)
        {
            if (_action.WasPressedThisFrame())
            {
                Installation();
            }
        }
#endif
    }
    void FixedUpdate()
    {
        _timer++;
        //// 方向キーの入力取得
        //// 下左右に動かす
        //if (!_fieldObject.GetComponent<TestController>().IsCheckField())
        //{
        //    MoveState();
        //}

        //else
        //{
        //    Debug.Log("処理を止める想定");
        //}
        //else
        //{
        //    //MoveObject2(0);
        //    //MoveObject(new Vector3(0.0f, 0.0f, 0.0f));
        //}

        // 下にキューブがあったら進まないようにしたい
        Vector2Int testpos = new Vector2Int(0, 14);
        foreach (Transform child in this.transform)
        {
            // 子オブジェクトに対する処理をここに書く
            Vector2Int pos = new Vector2Int(0, (int)child.transform.position.y);
            if (testpos.y > pos.y)
            {
                testpos = pos;
            }
            testpos = new Vector2Int(0, testpos.y - (int)this.transform.position.y) + _cubePos;
        }

        if (!_fieldObject.GetComponent<TestController>().IsCheckField())
        {
            if (_timer > 60 * 1.2 && _fieldObject.GetComponent<TestController>().IsNextCubeY(testpos))
            {
                Installation();
            }

            // HACK 時間での落下処理(仮)
            if (_timer > 60 * 1.2)
            {
                //_cubePos.y--;
                CubePos(0, -1);
                _timer = 0;
            }
            //Quaternion.Slerp();
        }
    }

    private void MoveState()
    {

        Vector2 moveInput = this._testInput.Piece.Move.ReadValue<Vector2>();
        _action = _testInput.Piece.Move;
        if (_action.IsPressed())
        {
            _inputframe++;
        }

        // HACK 斜めってどうやんねん(？？？？？？)
        // 下.
        //if (moveInput.y < 0 && _action.WasPressedThisFrame())
        if (moveInput.y < 0)
        {
            if (CubeMoveState())
            {
                Vector2Int checkPos = new Vector2Int(0, 14);

                foreach (Transform child in this.transform)
                {
                    // 子オブジェクトに対する処理をここに書く
                    Vector2Int pos = new Vector2Int(0, (int)child.transform.position.y);
                    if (checkPos.y > pos.y)
                    {
                        checkPos = pos;
                    }
                    checkPos = new Vector2Int(0, checkPos.y - (int)this.transform.position.y) + _cubePos;
                }
                if (!_fieldObject.GetComponent<TestController>().IsNextCubeY(checkPos))
                {
                    //_cubePos.y--;
                    _timer = 0;
                    CubePos(0, -1);
                }
                else
                {
                    // 下を押されたら落下時間を無視
                    _timer = (int)(60 * 1.2f);
                }
                _inputframe = 0;
            }
        }

        // 右.
        if (moveInput.x > 0)
        {
            if (CubeMoveState())
            {
                Vector2Int checkPos = new Vector2Int(0, 0);

                foreach (Transform child in this.transform)
                {
                    // 子オブジェクトに対する処理をここに書く
                    Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, 0) + _cubePos;
                    if (checkPos.x < pos.x)
                    {
                        checkPos = pos;
                    }
                }
                if (!_fieldObject.GetComponent<TestController>().IsNextCubeX(checkPos, 1))
                {
                    //_cubePos.x++;
                    CubePos(1, 0);
                }
                _inputframe = 0;
            }
        }
        // 左.
        else if (moveInput.x < 0)
        {
            if (CubeMoveState())
            {
                Vector2Int checkPos = new Vector2Int(13, 0);
                foreach (Transform child in this.transform)
                {
                    Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, 0) + _cubePos;
                    if (checkPos.x > pos.x)
                    {
                        checkPos = pos;
                    }
                }
                if (!_fieldObject.GetComponent<TestController>().IsNextCubeX(checkPos, -1))
                {
                    //_cubePos.x--;
                    CubePos(-1, 0);
                }
                _inputframe = 0;
            }
        }
    }

    // 設置するときの処理
    private void Installation()
    {
        int childcount = 0;
        foreach (Transform child in this.transform)
        {
            // 子オブジェクトに対する処理をここに書く
            //Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, (int)child.transform.position.y - (int)this.transform.position.y) + _cubePos;
            Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x + _cubePos.x,
                (int)child.transform.position.y - (int)this.transform.position.y);
            
            //Debug.Log(pos);

            // HACK テスト用.
            // こいつが悪さをしている
            // こいつはforで置ける一番低い場所を取ってしまうので0からいくとそうなる（説明下手だな？）
            pos = _fieldObject.GetComponent<TestController>().SteepDescent(pos,_direction);
            _colorNum = _colorManager.GetComponent<TestColorManager>().GetColorNumber(child.name);
            //Debug.Log(child.position + "wa"+ child.name +"  " + pos.y + " " + _colorNum);
            //Debug.Log(pos);
            _fieldObject.GetComponent<TestController>().IsSetCube(pos, _colorNum);
            childcount++;
        }

        _cubePos = new Vector2Int(3, 14);
        this.transform.position = _cubePostemp;
        // 回転の角度をもとに戻してあげる.
        _direction = 0;
        CubeRotation();
        // HACK うーん・・・とりあえず色替えはできてるけどバグが発生してるぅ
        _colorManager.GetComponent<TestColorManager>().ColorChenge();
    }

    private void CubePos(int x = 0, int y = 0)
    {
        this.transform.position = transform.position + new Vector3(x, y, 0);
        _cubePos.x += x;
        _cubePos.y += y;
    }
    // キューブの移動状態.
    private bool CubeMoveState()
    {
        if (_inputframe > 10 || _action.WasPressedThisFrame())
        {
            return true;
        }
        return false;
    }
    // 回転処理.
    private void CubeRotation()
    {
        foreach (Transform child in this.transform)
        {
            child.transform.position =
                new Vector3(_cubeDirection[_direction].x + transform.position.x, _cubeDirection[_direction].y + transform.position.y, 0);
            //Debug.Log(child.transform.position);
            //child.transform.position.y;
            break;
        }

        Vector2Int checkPos = new Vector2Int(0, 0);
        foreach (Transform child in this.transform)
        {
            // 子オブジェクトに対する処理をここに書く
            Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, 0) + _cubePos;

            // HACK 壁に貫通しないように処理
            checkPos.x = _fieldObject.GetComponent<TestController>().MoveRotaCheck(pos.x);
            //_cubePos += checkPos;
            CubePos(checkPos.x,0);
        }
    }
}
