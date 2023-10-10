using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestMove : MonoBehaviour
{
    private TestInputManager _testInput;
    private Rigidbody rb;
    private Vector3 force;
    // 移動速度.
    private float _speed;
    float currentSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _testInput = new TestInputManager();
        _testInput.Enable();

        rb = GetComponent<Rigidbody>();
        force = new Vector3(1.0f, 0.0f, 0.0f);
        _speed = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        // 方向キーの入力取得
        Vector2 stickVector2 = this._testInput.Piece.Move.ReadValue<Vector2>();
        // 上下左右に動かす
        //transform.Translate(stickVector2 * _speed * Time.deltaTime);
        //Vector2 a = stickVector2 * _speed * Time.deltaTime;
        // HACK とりあえず手を動かす用の処理(後でもっといい方法に書き直したいな).
        if (stickVector2.y < 0)
        {
            Debug.Log("動いて");
            // 下.
            MoveObject2(-1);
            MoveObject(new Vector3(0.0f, currentSpeed, 0.0f));
        }
        if (stickVector2.x > 0)
        {
            // 右.
            this.transform.position += new Vector3(5.0f, 0.0f, 0.0f);
            //MoveObject2(10);
            //MoveObject(new Vector3(currentSpeed, 0.0f, 0.0f));
        }
        if (stickVector2.x < 0)
        {
            // 左.
            MoveObject2(-10);
            MoveObject(new Vector3(currentSpeed, 0.0f, 0.0f));
        }


        //// HACK 時間での落下処理(仮)
        //MoveObject2(-1);
        //MoveObject(new Vector3(0.0f, currentSpeed, 0.0f));

    }

    void MoveObject2(float test = 1)
    {
        //指定したスピードから現在の速度を引いて加速力を求める
        currentSpeed = (_speed - rb.velocity.magnitude) * test;
    }
    void MoveObject(Vector3 currentspeed)
    {
        if (rb.velocity.magnitude < 10)
        {
            //調整された加速力で力を加える
            rb.AddForce(currentspeed);
        }
    }
    

}
