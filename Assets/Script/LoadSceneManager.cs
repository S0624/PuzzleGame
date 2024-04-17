using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Linq;
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
    public CursorController _selectCursor;
    public SelectSceneManager _selectManager;
    // ボタンを押したかのフラグ
    private bool _buttonPush = false;
    // 放置していたら移行するフラグ
    private bool _autoState = false;
    private bool _isPrevFlag  = false;
    // フェードの取得.
    private Fade _fade;
    // フェードのフラグの取得.
    private FadeManager _fadeManager;
    // シーンが切り替わるときに音を鳴らすためのサウンドの取得
    private SoundManager _soundManager;
    // フルスクかどうかのフラグ
    private bool _isFullDisplay = false;
    static public bool _isSceneChenge = false;
    // Start is called before the first frame update
    void Start()
    {
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
        // シーン移行が可能かどうかのフラグの更新処理
        SceneChengeFlagUpdate();
        // ゲームのディスプレイのサイズの変更処理.
        GameDisplaySizeChenge();
        // 更新処理.
        SceneUpdate();
    }
    /// <summary>
    /// シーン移行が可能かどうかのフラグの更新処理
    /// </summary>
    private void SceneChengeFlagUpdate()
    {
        if (_fade.cutoutRange == 0.0f)
        {
            _isSceneChenge = false;
        }
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
        if (_buttonPush)
        {
            SceneSwitching();
        }
        if (_autoState) return;
        // しかるべき時に押したらシーンが移動します
        if (_fade.cutoutRange == 0.0f)
        {
            if (ControllerInput())
            {
                _soundManager.SEPlay(SoundSEData.TitlePushSE);
                if (_selectCursor != null)
                {
                    //// 仮実装
                    //if (_selectCursor.SelectNum() == 2)
                    //{
                    //    return;
                    //}
                    // 選んだ画像を拡大する
                    _selectManager.ImageScaleChenge(_selectCursor.SelectNum());
                    if (_selectCursor.SelectNum() == 1)
                    {
                        _selectCursor.Decision(true);
                        if (_selectManager.ControllerCheck()) return;
                    }
                    DifficultyUpdate();
                }
                _buttonPush = true;
            }
            if (_selectCursor != null)
            {
                if(_selectManager._isWarningDestory)
                {
                    // 選択した画像をデフォルトの大きさに戻す
                    _selectManager.ImageScaleDefalutChenge();
                    _selectManager._isWarningDestory = false;
                }
                if (_selectManager.IsWarningDisplay() && !_selectManager.DisplayUpdate())
                {
                    _selectCursor.Decision(true);
                    return;
                }
                if (_selectManager.IsDifficultyObj())
                {
                    _selectManager.DifficultyDisplayDestory();
                    if (!_selectManager.NowDifficultyDisplay()) return;
                }
                // 難易度の壊すフラグが立っていたら
                if (_selectManager.IsDifficultyDestory())
                {
                    // 選択した画像をデフォルトの大きさに戻す
                    _selectManager.ImageScaleDefalutChenge();
                    _buttonPush = false;
                }
            }
            _fadeManager._isFade = _buttonPush;
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
    }
    // ボタンを押したら難易度を選んでもらう処理
    private void DifficultyUpdate()
    {
        _selectManager.DifficultyDisplay();
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
            _selectCursor.Decision(true);
            // コントローラーの接続が確認されたらフェードを行う
            DifficultyUpdate();
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
        if (_selectCursor == null)
        {
            // Sceneを切り替える
            LoadScene(_nextScene[0]);
        }
        // 前に戻るを押された
        else if (_isPrevFlag)
        {
            LoadScene(_prevScene);

        }
        else
        {
            // ボタンを押したかどうかのフラグを渡す.
            _selectCursor.Decision(_buttonPush);
            //// ネットワークは準備中なので押せないようにするよ
            //if (_select.SelectNum() == 2) return;

            // Sceneを切り替える
            LoadScene(_nextScene[_selectCursor.SelectNum()]);
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
        _autoState = true;
        _fadeManager._isFade = _autoState;
        LoadScene(_nextScene[0]);
    }
    // タイトルからDemo動画に移行する処理
    public void TitleChenge()
    {
        _autoState = true;
        _fadeManager._isFade = _autoState;
        LoadScene(_demoScene);
    }
    // ポーズ画面からモードセレクトに戻るを押された場合の処理.
    public void PauseTransitionScene()
    {
        _buttonPush = true;
        _fadeManager._isFade = true;
        LoadScene(_pauseScene);
        Time.timeScale = 1f;
    }
    // シーンをロードする
    private void LoadScene(string scenename)
    {
        // シーンを移行してもよいかどうかをチェックする
        if (IsLoadSceneChenge())
        {
            // シーン移行したのでシーン移行をできないようにフラグを立てる
            _isSceneChenge = true;
            SceneManager.LoadSceneAsync(scenename);
        }
    }
    /// <summary>
    /// シーンをロードしてもいいかの検知
    /// </summary>
    /// <returns>シーンをロード可能</returns>
    private bool IsLoadSceneChenge()
    {
        // チェンジ可能かどうかのフラグ
        bool isChenge = true;
        // もし条件にあってなかったらfalseを返す
        // フェードが終わりきっているかどうか
        if (_fade.cutoutRange != 1.0f) return false;
        // フェードのフラグがたっているかどうか
        if (!_fadeManager._isFade) return false;
        // シーンを移行しても可能かどうかのフラグ
        if (_isSceneChenge) return false;
        // ここまで来れたらすべての条件をクリアしている
        return isChenge;
    }
    public float FadeNumericalValue()
    {
        return _fade.cutoutRange;
    }
}
