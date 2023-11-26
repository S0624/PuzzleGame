using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// HACK仮でシーンをつなげてるスクリプト（仮なのであとでちゃんとフラグ受け取る）
public class LoadSceneManager : MonoBehaviour
{
    //  読み込むシーン
    [SerializeField] private string _scene;
    // ボタンの処理をするための変数.
    private TestInputManager _input;
    // 仮取得
    private TestController _controller;
    // Start is called before the first frame update
    void Start()
    {
        _input = new TestInputManager();
        _input.Enable();
        if (GameObject.Find("Field"))
        { 
            _controller = GameObject.Find("Field").GetComponent<TestController>();
        } 
    }

    // Update is called once per frame
    void Update()
    {
        // もし入力したキーがSpaceキーならば、強制的に処理を実行する
        // それ以外はしかるべき時に押したらシーンが移動します
        if (Input.GetKeyDown(KeyCode.Space)|| TestInput())
        {
            // SampleSceneに切り替える
            SceneManager.LoadSceneAsync(_scene);
        }
    }
    private bool TestInput()
    {
        if (_controller != null)
        {
            if (_controller.IsGameOver() && _input.UI.Submit.WasPerformedThisFrame())
            {
                return true;
            }
        }
        else if(_input.UI.Submit.WasPerformedThisFrame())
        {
            return true;
        }
        return false;
    }
}
