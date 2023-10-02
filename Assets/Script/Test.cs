using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // 移動速度.
    private float _speed;
    // Start is called before the first frame update
    void Start()
    {
        _speed = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        // HACK とりあえず手を動かす用の処理(後でもっといい方法に書き直したいな).
        //if (Input.GetAxis("Vertical") > 0)
        //{
        //    // 上.
        //    transform.position += transform.up * _speed * Time.deltaTime;
        //}
        if (Input.GetAxis("Vertical") < 0)
        {
            // 下.
            this.transform.localPosition -= transform.up * _speed * Time.deltaTime;
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            // 右.
            this.transform.localPosition += transform.right * _speed * Time.deltaTime;
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            // 左.
            this.transform.localPosition -= transform.right * _speed * Time.deltaTime;
        }

        this.transform.localPosition += new Vector3(0.0f,-0.01f,0.0f);
    }
}
