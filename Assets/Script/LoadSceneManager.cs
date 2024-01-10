using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
// HACK仮でシーンをつなげてるスクリプト（仮なのであとでちゃんとフラグ受け取る）
public class LoadSceneManager : MonoBehaviour
{
    //  読み込むシーン
    [SerializeField] private string[] _nextScene;
    [SerializeField] private string _prevScene;
    [SerializeField] private string _pauseScene;
    // ボタンの処理をするための変数.
    private InputManager _input;
    // 仮取得
    //public GameObject _field;
    public FieldData[] _controller;
    // セレクトシーンのみで使用.
    public CursorController _select;
    // ボタンを押したかのフラグ
    private bool _buttonPush = false;
    // フェードの取得.
    private Fade _fade;
    // フェードのフラグの取得.
    private FadeManager _fadeManager;

    // Start is called before the first frame update
    void Start()
    {
        _fade = GameObject.Find("FadeCanvas").GetComponent<Fade>();
        _fadeManager = GameObject.Find("FadeManager").GetComponent<FadeManager>();
        _input = new InputManager();
        _input.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        SceneUpdate();
    }
    // シーンのUpdate処理.
    private void SceneUpdate()
    {
        // もし入力したキーがSpaceキーならば、強制的に処理を実行する
        // それ以外はしかるべき時に押したらシーンが移動します
        if (Input.GetKeyDown(KeyCode.Space) || ControllerInput())
        {
            _buttonPush = true;
            _fadeManager._isFade = _buttonPush;
        }
        if (_buttonPush)
        {
            SceneSwitching();
        }
        
    }
    // ボタンを押されたらシーンを切り替える処理
    private void SceneSwitching()
    {
        if (_select == null)
        {
            // Sceneを切り替える
            LoadScene(_nextScene[0]);
        }
        else
        {
            // ボタンを押したかどうかのフラグを渡す.
            _select.Decision(_buttonPush);
            //// ネットワークは準備中なので押せないようにするよ
            //if (_select.SelectNum() == 2) return;
            // Sceneを切り替える
            LoadScene(_nextScene[_select.SelectNum()]);
        }

        // 前のシーンの情報がなかったら処理を飛ばす.
        if (_prevScene == "") return;
        // 戻るボタンを押したら一個前のシーンに戻る
        if (Input.GetKeyUp(KeyCode.KeypadEnter) || _input.UI.Cancel.WasPerformedThisFrame())
        {
            LoadScene(_prevScene);

        }
    }
    private bool ControllerInput()
    {
        foreach (var controller in _controller)
        {
            if (controller != null)
            {
                if (controller.IsGameOver() && _input.UI.Submit.WasPerformedThisFrame())
                {
                    return true;
                }
            }
            else if (_input.UI.Submit.WasPerformedThisFrame())
            {
                return true;
            }
        }
        return false;
    }
    // ポーズ画面からモードセレクトに戻るを押された場合の処理.
    public void PauseTransitionScene()
    {
        _fadeManager._isFade = true;
        LoadScene(_pauseScene);
    }
    // シーンをロードする
    private void LoadScene(string scenename)
    {
        if (_fade.cutoutRange == 1.0f && _fadeManager._isFade)
        {
            SceneManager.LoadSceneAsync(scenename);
        }
    }
}
