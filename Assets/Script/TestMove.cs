using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestMove : MonoBehaviour
{
    private InputAction _action;
    private TestInputManager _testInput;
    private Rigidbody rb;
    private Vector3 force;
    // 移動速度.
    //private float _speed;
    //private float currentSpeed;

    private GameObject _fieldObject;
    private Vector2Int _cubePos = new Vector2Int(3,14);
    private Vector3 _cubePostemp;
    private int _timer = 0;

    int testtimer = 0;
    private int _colorNum;
    private Vector2Int[] _cubeDirection = new Vector2Int[(int)Direction.max];
    private int _direction = 0;

    // HACK HELP>>>>>>>>>
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
    void Start()
    {
        _testInput = new TestInputManager();
        _testInput.Enable();

        rb = GetComponent<Rigidbody>();
        force = new Vector3(1.0f, 0.0f, 0.0f);
        //_speed = 5.0f;

        _fieldObject = GameObject.Find("Board");
        //this.transform.position = transform.position + new Vector3(_cubePos.x, _cubePos.y, 0);
        this.transform.position = Vector3.zero;
        this.transform.position = transform.position +transform.position + _fieldObject.GetComponent<TestController>().fieldPos(_cubePos);
        
        //this.transform.position = new Vector3(_cubePos.x, _cubePos.y, 0);
        _cubePostemp = this.transform.position;
        //foreach (Transform child in this.transform)
        //{
        //    child.transform.position = child.transform.position - new Vector3(_cubePos.x, 0, 0);
        //    // 子オブジェクトに対する処理をここに書く
        //    Debug.Log(child.transform.position);
        //}

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
                Debug.Log(child);
                child.transform.position = 
                    new Vector3(_cubeDirection[_direction].x + transform.position.x, _cubeDirection[_direction].y + transform.position.y, 0);
                //child.transform.position.y; 
                break;
            }
                //Debug.Log("ぱぁ");
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
        //this.transform.position = transform.position + new Vector3(_testPos.x, _testPos.y, 0);
        //Debug.Log(_testPos);
        // 方向キーの入力取得
        // 下左右に動かす
        //transform.Translate(stickVector2 * _speed * Time.deltaTime);
        //Vector2 a = stickVector2 * _speed * Time.deltaTime;

        if (!_fieldObject.GetComponent<TestController>().IsCheckField())
        {
            _timer++;

            Vector2 moveInput = this._testInput.Piece.Move.ReadValue<Vector2>();
            _action = _testInput.Piece.Move;
            //Debug.Log(_action.IsPressed());
            //Debug.Log(testtimer);
            if (_action.IsPressed())
            {
                testtimer++;
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
                    Debug.Log(checkPos+""+_cubePos);
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
                    testtimer = 0;
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
                    testtimer = 0;
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
                    testtimer = 0;
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

    //void MoveObject2(float test = 1)
    //{
    //    //指定したスピードから現在の速度を引いて加速力を求める
    //    currentSpeed = (_speed - rb.velocity.magnitude) * test;
    //}
    //void MoveObject(Vector3 currentspeed)
    //{
    //    if (rb.velocity.magnitude < 10)
    //    {
    //        Debug.Log(rb.velocity.magnitude);
    //        //調整された加速力で力を加える
    //        rb.AddForce(currentspeed);
    //    }
    //    else
    //    {
    //        rb.AddForce(Vector3.zero);
    //    }
    //}

    // HACK けす
    private int ColorRandam()
    {
        //_colorNum = Random.Range((int)ColorType.Green, (int)ColorType.PuyoMax);
        _colorNum = Random.Range((int)ColorType.Green,4);
        //_colorNum = Random.Range((int)ColorType.Green, (int)ColorType.Yellow);
        //this.GetComponent<Renderer>().material.color = color_table[_colorNum];
        return _colorNum;
    }
    private void CubePos(float x = 0, float y = 0)
    {
        this.transform.position = transform.position + new Vector3(x, y, 0);
    }

    private bool CubeMoveState()
    {
        if (testtimer > 10 || _action.WasPressedThisFrame())
        {
            return true;
        }
        return false;
    }
}
