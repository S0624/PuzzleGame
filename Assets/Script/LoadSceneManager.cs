using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
// HACK仮でシーンをつなげてるスクリプト（仮なのであとでちゃんとフラグ受け取る）
public class LoadSceneManager : MonoBehaviour
{
    //  読み込むシーン
    [SerializeField] private string[] _nextScene;
    [SerializeField] private string _prevScene;
    [SerializeField] private string _pauseScene;
    [SerializeField] private string _demoScene;
    // ボタンの処理をするための変数.
    private InputManager[] _input;
    // 仮取得
    //public GameObject _field;
    public FieldData[] _controller;
    // セレクトシーンのみで使用.
    public CursorController _select;
    public SelectSceneManager _selectManager;
    // ボタンを押したかのフラグ
    private bool _buttonPush = false;
    private bool _isPrevFlag  = false;
    // フェードの取得.
    private Fade _fade;
    // フェードのフラグの取得.
    private FadeManager _fadeManager;
    // シーンが切り替わるときに音を鳴らすためのサウンドの取得
    private SoundManager _soundManager;
    // フルスクかどうかのフラグ
    private bool _isFullDisplay = false;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        _fade = GameObject.Find("FadeCanvas").GetComponent<Fade>();
        _fadeManager = GameObject.Find("FadeManager").GetComponent<FadeManager>();
        GetControllerInit();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        if (_selectManager)
        {
            _selectManager.GetInputInit(_input);
        }

    }
    // コントローラーの更新
    private void GetControllerInit()
    {
        // ゲームパッドの取得.
        _input = Gamepad.all.Select(pad =>
        {
            var control = new InputManager();
            control.devices = new UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputDevice>(new[] { pad });
            control.Enable();
            return control;
        }).ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームのディスプレイのサイズの変更処理.
        GameDisplaySizeChenge();
        // 更新処理.
        SceneUpdate();
    }
    // ゲームをフルスクにするかの処理
    private void GameDisplaySizeChenge()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_isFullDisplay)
            {
                // ディスプレイモードに切り替えます
                Screen.fullScreen = false;
                _isFullDisplay = false;
            }
            else
            {
                // フルスクリーンモードに切り替えます
                Screen.fullScreen = true;
                _isFullDisplay = true;
            }

        }

    }
    // シーンのUpdate処理.
    private void SceneUpdate()
    {
        // セレクトシーンにいるときのみ処理を行う
        if (_selectManager)
        {
            // セレクトシーンの時のみ処理を行う更新処理
            SelectSceneUpdate();
            if (_selectManager.IsSettingUpdate()) return;
        }
        SceneTransition();

    }
    private void SceneTransition()
    {
        // もし入力したキーがSpaceキーならば、強制的に処理を実行する
        // それ以外はしかるべき時に押したらシーンが移動します
        if (ControllerInput())
        {
            _soundManager.SEPlay(SoundSEData.TitlePushSE);
            if (_select != null && _select.SelectNum() == 1)
            {
                if (_selectManager.ControllerCheck()) return;
            }
            _buttonPush = true;
            _fadeManager._isFade = _buttonPush;
        }
        if (_buttonPush)
        {
            SceneSwitching();
        }
        // 前のシーンの情報がなかったら処理を飛ばす.
        if (_prevScene == "") return;
        // 戻るボタンを押したら一個前のシーンに戻る
        if (Input.GetKeyUp(KeyCode.KeypadEnter) || _input[0].UI.Cancel.WasPerformedThisFrame())
        {
            _buttonPush = true;
            _isPrevFlag = true;
            _fadeManager._isFade = _buttonPush;
        }
    }
    // セレクトシーンの更新処理
    private void SelectSceneUpdate()
    {
        // 更新処理.
        _selectManager.SelectSceneUpdate();
        // コントローラーの数取得.
        _selectManager.InputLength(_input.Length);
        // 警告文が表示されているときの処理.
        if (_selectManager.DisplayUpdate())
        {
            // コントローラーの接続が確認されたらフェードを行う
            _buttonPush = _selectManager.DisplayUpdate();
        }
        // コントローラーの再取得処理
        if (_selectManager._isReacquisition)
        {
            GetControllerInit();
        }
        // 警告文が表示されていたらフェードやカーソル移動を制限するために処理を飛ばす
        if (_selectManager._warninigObject) return;
    }
    // ボタンを押されたらシーンを切り替える処理
    private void SceneSwitching()
    {
        if (_select == null)
        {
            // Sceneを切り替える

            LoadScene(_nextScene[0]);
        }
        // 前に戻るを押された
        else if(_isPrevFlag)
        {
            LoadScene(_prevScene);

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


    }
    private bool ControllerInput()
    {
        foreach (var controller in _controller)
        {
            if (controller != null)
            {
                if (controller.IsGameOver() && _input[0].UI.Submit.WasPerformedThisFrame())
                {
                    return true;
                }
            }
            else if (_input[0].UI.Submit.WasPerformedThisFrame())
            {
                return true;
            }
        }
        return false;
    }
    // Demo動画のところでのみ使用する処理
    public void DemoMoveSceneChenge()
    {
        _fadeManager._isFade = true;
        LoadScene(_nextScene[0]);
    }
    public void TitleChenge()
    {
        _fadeManager._isFade = true;
        LoadScene(_demoScene);
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
    public float FadeNumericalValue()
    {
        return _fade.cutoutRange;
    }
}
