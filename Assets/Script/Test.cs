using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // �ړ����x.
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
        // HACK �Ƃ肠������𓮂����p�̏���(��ł����Ƃ������@�ɏ�������������).
        //if (Input.GetAxis("Vertical") > 0)
        //{
        //    // ��.
        //    transform.position += transform.up * _speed * Time.deltaTime;
        //}
        if (Input.GetAxis("Vertical") < 0)
        {
            // ��.
            this.transform.localPosition -= transform.up * _speed * Time.deltaTime;
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            // �E.
            this.transform.localPosition += transform.right * _speed * Time.deltaTime;
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            // ��.
            this.transform.localPosition -= transform.right * _speed * Time.deltaTime;
        }

        this.transform.localPosition += new Vector3(0.0f,-0.01f,0.0f);
    }
}
