using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// ボタンを押したことを検知するテスト用スクリプト
public class TestButton : MonoBehaviourPunCallbacks
{
    // ボタンの処理をするための変数.
    private InputManager _input;
    // Start is called before the first frame update
    void Start()
    {
        _input = new InputManager();
        _input.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            var input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
            transform.Translate(6f * Time.deltaTime * input.normalized);
            // 対象のキーを押した時の処理.
            if (_input.UI.Submit.WasPerformedThisFrame() || Input.GetKeyDown("z"))
            {
                // 色を変える
                transform.position = Vector3.zero;

            }
        }
    }
}
