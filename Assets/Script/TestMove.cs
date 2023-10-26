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

    // HACK なんかいい処理ないんですか別ファイルで全く同じの使ってる
    // 実行時に値を取得する読み取り専用の変数を生成
    static readonly Color[] color_table = new Color[] {
        Color.white,        // 白.
        Color.green,        // 緑.
        Color.red,          // 赤.
        Color.yellow,       // 黄色.
        Color.blue,         // 青.
        Color.magenta,      // 紫.
        Color.gray,         // 仮でグレーを入れる

        Color.gray,         // おじゃま(グレー)
    };

    // Start is called before the first frame update
    // 初期化処理
    void Start()
    {

        _testInput = new TestInputManager();
        _testInput.Enable();

        //_speed = 5.0f;
        // フィールドを取得
        _fieldObject = GameObject.Find("Board");
        //this.transform.position = transform.position + new Vector3(_cubePos.x, _cubePos.y, 0);
        this.transform.position = Vector3.zero;
        // 自分のポジションから
        // これをしないといろんな位置に行ってしまうので、フィールドに合わせてキューブの位置を初期化する
        this.transform.position = transform.position +transform.position + _fieldObject.GetComponent<TestController>().fieldPos(_cubePos);
        
        //this.transform.position = new Vector3(_cubePos.x, _cubePos.y, 0);
        _cubePostemp = this.transform.position;

        _colorNum = ColorRandam();
        //_testField.GetComponent<TestController>().IsSetCube(_testPos,0);
        _cubeDirection[(int)Direction.Down] = new Vector2Int(0, -1);
        _cubeDirection[(int)Direction.Left] = new Vector2Int(-1, 0);
        _cubeDirection[(int)Direction.Up] = new Vector2Int(0, 1);
        _cubeDirection[(int)Direction.Right] = new Vector2Int(1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //// 子オブジェクトを全て取得する
        //foreach (Transform child in this.transform)
        //{
        //    // 子オブジェクトに対する処理をここに書く
        //    Debug.Log(child.transform.position);
        //}
#if true
        // 雑に回転処理を実装(お試し)
        if (_testInput.Piece.RotationRight.WasPerformedThisFrame())
        {
            _direction++;
            if( _direction >= 4 )
            {
                _direction = 0;
            }
            // HACK 回転処理 ﾋｨ.........
            foreach (Transform child in this.transform)
            {
                child.transform.position = 
                    new Vector3(_cubeDirection[_direction].x + transform.position.x, _cubeDirection[_direction].y + transform.position.y, 0);
                //child.transform.position.y; 
                break;
            }
        }

        Vector2 moveInput = this._testInput.Piece.Move.ReadValue<Vector2>();
        // 急降下で下に落とす(DEBUG機能).
        if (moveInput.y > 0)
        {
            if (_action.WasPressedThisFrame())
            {
                foreach (Transform child in this.transform)
                {
                    // 子オブジェクトに対する処理をここに書く
                    Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, (int)child.transform.position.y) + _cubePos;
                    pos = _fieldObject.GetComponent<TestController>().SteepDescent(pos);
                    _fieldObject.GetComponent<TestController>().IsSetCube(pos, _colorNum);
                }
                //_cubePos = _fieldObject.GetComponent<TestController>().SteepDescent(_cubePos);
                //_fieldObject.GetComponent<TestController>().IsSetCube(_cubePos, _colorNum);

                _cubePos = new Vector2Int(3, 14);
                this.transform.position = _cubePostemp;
                ColorRandam();
            }
        }
#endif
    }
    void FixedUpdate()
    {
        // 方向キーの入力取得
        // 下左右に動かす
        if (!_fieldObject.GetComponent<TestController>().IsCheckField())
        {
            _timer++;

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
                        _cubePos.y--;
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
                        _cubePos.x++;
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
                        _cubePos.x--;
                        CubePos(-1, 0);
                    }
                    _inputframe = 0;
                }
            }
        }
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

        if (_timer > 60 * 1.2 && _fieldObject.GetComponent<TestController>().IsNextCubeY(testpos))
        {
            //_fieldObject.GetComponent<TestController>().IsSetCube(_cubePos, 0);
            //_fieldObject.GetComponent<TestController>().IsSetCube(_cubePos, _colorNum);
            // 子オブジェクトを全て取得する
            foreach (Transform child in this.transform)
            {
                // 子オブジェクトに対する処理をここに書く
                Vector2Int pos = new Vector2Int((int)child.transform.position.x - (int)this.transform.position.x, (int)child.transform.position.y) + _cubePos;
                pos = _fieldObject.GetComponent<TestController>().SteepDescent(pos);
                _fieldObject.GetComponent<TestController>().IsSetCube(pos, _colorNum);

                //Vector2Int pos = new Vector2Int((int)child.transform.position.x, (int)child.transform.position.y);
                //_fieldObject.GetComponent<TestController>().IsSetCube(pos, _colorNum);
            }

            _cubePos = new Vector2Int(3, 14);
            this.transform.position = _cubePostemp;
            ColorRandam();
        }

        // HACK 時間での落下処理(仮)
        if(_timer > 60 * 1.2)
        {
            _cubePos.y--;
            CubePos(0, -1);
            _timer = 0;
        }

    }

    // HACK とりあえず雑に色を決めてるのであとでけします
    private int ColorRandam()
    {
        //　雑に色付けようとしたけどバグってる

        //_colorNum = Random.Range((int)ColorType.Green, (int)ColorType.PuyoMax);
        //   _colorNum = Random.Range((int)ColorType.Green,4);
        // 子オブジェクトを全て取得する
        foreach (Transform child in this.transform)
        {
            _colorNum = Random.Range((int)ColorType.Green, (int)ColorType.Yellow);
            child.GetComponent<Renderer>().material.color = color_table[_colorNum];
        }
        return _colorNum;
    }
    private void CubePos(float x = 0, float y = 0)
    {
        this.transform.position = transform.position + new Vector3(x, y, 0);
    }

    private bool CubeMoveState()
    {
        if (_inputframe > 10 || _action.WasPressedThisFrame())
        {
            return true;
        }
        return false;
    }
}
