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
    private float _speed;
    private float currentSpeed;

    private GameObject _fieldObject;
    private Vector2Int _cubePos = new Vector2Int(0,14);
    private Vector3 _cubePostemp;
    private int _timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        _testInput = new TestInputManager();
        _testInput.Enable();

        rb = GetComponent<Rigidbody>();
        force = new Vector3(1.0f, 0.0f, 0.0f);
        _speed = 5.0f;

        this.transform.position = transform.position + new Vector3(_cubePos.x, _cubePos.y, 0);
        _cubePostemp = this.transform.position;
        _fieldObject = GameObject.Find("Board");
        //_testField.GetComponent<TestController>().IsSetCube(_testPos,0);

    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        _timer++;
        //this.transform.position = transform.position + new Vector3(_testPos.x, _testPos.y, 0);
        //Debug.Log(_testPos);
        // 方向キーの入力取得
        Vector2 moveInput = this._testInput.Piece.Move.ReadValue<Vector2>();
        // 上下左右に動かす
        //transform.Translate(stickVector2 * _speed * Time.deltaTime);
        //Vector2 a = stickVector2 * _speed * Time.deltaTime;
        // HACK とりあえず手を動かす用の処理(後でもっといい方法に書き直したいな).
        _action = _testInput.Piece.Move;
        if (moveInput.y < 0 && _action.WasPressedThisFrame())
        {
            if (!_fieldObject.GetComponent<TestController>().IsNextCube(_cubePos))
            //if (_cubePos.y > 0)
            {
                _cubePos.y--;
                _timer = 0;
                CubePos(0, -1);
            }
            else
            {
                _timer = (int)(60 * 1.2f);
            }
            //_testField.GetComponent<TestController>().ClearAll();
            //_testField.GetComponent<TestController>().IsSetCube(_testPos,0);
            // 下.
            //MoveObject2(-1);
            //MoveObject(new Vector3(0.0f, currentSpeed, 0.0f));
        }
        else if (moveInput.x > 0 && _action.WasPressedThisFrame())
        {
            _cubePos.x++;
            CubePos(1, 0);
            //_testField.GetComponent<TestController>().ClearAll();
            //_testField.GetComponent<TestController>().IsSetCube(_testPos,0);
            // 右.
            //MoveObject2(10);
            //MoveObject(new Vector3(currentSpeed, 0.0f, 0.0f));
        }
        else if (moveInput.x < 0 && _action.WasPressedThisFrame())
        {
            _cubePos.x--;
            CubePos(-1, 0);
            //_testField.GetComponent<TestController>().ClearAll();
            //_testField.GetComponent<TestController>().IsSetCube(_testPos,0);
            // 左.
            //MoveObject2(-10);
            //MoveObject(new Vector3(currentSpeed, 0.0f, 0.0f));
        }
        else
        {
            //MoveObject2(0);
            //MoveObject(new Vector3(0.0f, 0.0f, 0.0f));
        }
        // 下にキューブがあったら進まないようにしたい
        if(_timer > 60 * 1.2 && _fieldObject.GetComponent<TestController>().IsNextCube(_cubePos))
        {
            _fieldObject.GetComponent<TestController>().IsSetCube(_cubePos, 0);
            _cubePos = new Vector2Int(0, 14);
            this.transform.position = _cubePostemp;
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

    private void CubePos(float x = 0, float y = 0)
    {
        this.transform.position = transform.position + new Vector3(x, y, 0);
    }
}
