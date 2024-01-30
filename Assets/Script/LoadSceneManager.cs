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
    // ボタンの処理をするための変数.
    private InputManager[] _input;
    // 仮取得
    //public GameObject _field;
    public FieldData[] _controller;
    // セレクトシーンのみで使用.
    public CursorController _select;
    // 警告画像を出すためにキャンバスの取得
    public Canvas _canvas;
    // 警告画像の取得
    public GameObject _warning;
    private GameObject _warninigObject;
    // 警告画像の表示時間
    private int _warningDisplayTimer = 60;
    // 表示の削除のフラグ
    private bool _isDisplay = false;
    // ボタンを押したかのフラグ
    private bool _buttonPush = false;
    // フェードの取得.
    private Fade _fade;
    // フェードのフラグの取得.
    private FadeManager _fadeManager;
    // シーンが切り替わるときに音を鳴らすためのサウンドの取得
    private SoundManager _soundManager;
    // Start is called before the first frame update
    void Start()
    {
        _fade = GameObject.Find("FadeCanvas").GetComponent<Fade>();
        _fadeManager = GameObject.Find("FadeManager").GetComponent<FadeManager>();
        GetControllerInit();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        SceneUpdate();
    }
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
    // シーンのUpdate処理.
    private void SceneUpdate()
    {
        DisplayUpdate();
        DisplayDestory();
        InputCheck();

        // もし入力したキーがSpaceキーならば、強制的に処理を実行する
        // それ以外はしかるべき時に押したらシーンが移動します
        if (Input.GetKeyDown(KeyCode.Space) || ControllerInput())
        {
            _soundManager.SEPlay(SoundSEData.TitlePushSE);
            if (_select != null && _select.SelectNum() == 1)
            {
                if (ControllerCheck()) return;
            }
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
        if (Input.GetKeyUp(KeyCode.KeypadEnter) || _input[0].UI.Cancel.WasPerformedThisFrame())
        {
            LoadScene(_prevScene);

        }
    }
    // コントローラーの数の取得
    private bool ControllerCheck()
    {
        if (_input.Length < 2)
        {
            if (!_warninigObject)
            {
                _select.Decision(true);
                _warninigObject = Instantiate(_warning);
                _warninigObject.transform.SetParent(_canvas.transform, false);
                _warninigObject.transform.DOScale(Vector3.one, 1.0f).SetEase(Ease.OutCirc);
            }

            return true;
        }
        return false;
    }
    private void DisplayUpdate()
    {
        if (_warninigObject)
        {
            if (_warninigObject.transform.localScale.x == 1.0f)
            {
                // 警告画像の削除
                _warningDisplayTimer--;
                if (_warningDisplayTimer < 0)
                {
                    // コントローラーの数の取得
                    if (_input.Length >= 2)
                    {
                        _buttonPush = true;
                        _fadeManager._isFade = _buttonPush;
                    }

                    GetControllerInit();
                    _warningDisplayTimer = 60;

                }
            }
            if (_input[0].UI.Cancel.WasPerformedThisFrame())
            {
                _isDisplay = true;
            }
        }
    }
    // 警告画像の削除処理
    private void DisplayDestory()
    {
        if (_isDisplay)
        {
            _select.Decision(false);
            _isDisplay = false;
            Destroy(_warninigObject);
        }
    }
    private void InputCheck()
    {
        if (_warninigObject)
        {
            if (_input.Length >= 2)
            {
                GameObject child = _warninigObject.transform.GetChild(2).gameObject;
                var color = Color.white;
                color.a = 255;
                Debug.Log(color.a);
                child.GetComponent<Image>().color = color;
            }
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
