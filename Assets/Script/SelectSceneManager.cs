using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SelectSceneManager : MonoBehaviour
{
    // ボタンの処理をするための変数.
    private InputManager[] _input;
    // セレクトシーンのみで使用.
    public CursorController _select;
    // 警告画像を出すためにキャンバスの取得
    public Canvas _canvas;
    // 警告画像の取得
    public GameObject _warning;
    public GameObject _warninigObject;
    private GameObject _difficulty;
    public GameObject _difficultyObject;
    // 警告画像の表示時間
    private int _warningDisplayTimer = 60;
    // 表示の削除のフラグ
    private bool _isWarningsDisplay = false;
    private bool _isPrevWarningsDisplay = false;
    // 再取得するフラグ
    public bool _isReacquisition = false;
    // 設定画面の取得
    public SettingController _setting;
    // 設定画面を開いているかどうか
    private　bool _isSetting = false;
    // 前フレームのフラグ
    private bool _isPrevFlag = false;
    // シーンが切り替わるときに音を鳴らすためのサウンドの取得
    private SoundManager _soundManager;
    // コントローラーの数取得
    private int _controllerNum = 0;
    // 画像の取得
    public　Image[] _image;
    private int _imageScaleNum = 0;
    // defaultの大きさの取得
    private Vector3 _defaultScale = Vector3.one;
    // スケールを変えたかどうかのフラグ
    private bool _isScale = false;

    public bool _isWarningDestory = false;
    private bool _isDifficultyDestory = false;
    // Start is called before the first frame update
    void Start()
    {
        //GetControllerInit();
        _defaultScale = _image[0].transform.localScale;
        
        //_setting.StartSettingOpen();
        //_setting.SettingCanvasClose();
    }
    // ゲームパッドを受け取る
    public void GetInputInit(InputManager[] input)
    {
        // ゲームパッドの取得.
        _input = input;
    }
    // コントローラーの数取得
    public void InputLength(int input)
    {
        _controllerNum = input;
    }
    // Update is called once per frame
    void Update()
    {
        //SelectSceneUpdate();
    }
    // セッティング画面の更新処理
    public bool IsSettingUpdate()
    {
        _isPrevFlag = _setting.IsSettingCanvas();
        _select.Decision(_isSetting);
        if (_input[0].UI.Pause.WasPerformedThisFrame())
        {
            if (_isSetting)
            {
                _setting.SettingCanvasClose();
            }
            else
            {
                _setting.StartSettingOpen();
            }
        }
        _setting.StartSettingOpenUpdate();
        _isSetting = _setting.IsSettingCanvas();
        if (_setting.IsSettingCanvas())
        {
            return true;
        }
        if (_setting.IsSettingCanvas() != _isPrevFlag)
        {
            return true;
        }
        return false;
    }
    public void SelectSceneUpdate()
    {
        WarningDisplayDestory();
        InputCheck();
    }
    // コントローラーの数の取得
    public bool ControllerCheck()
    {
        if (_controllerNum < 2)
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
    public bool IsWarningDisplay()
    {
        return _warninigObject;
    }

    public bool DisplayUpdate()
    {
        bool iscontroller = false;
        _isReacquisition = false;
        if (_warninigObject)
        {
            if (_warninigObject.transform.localScale.x == 1.0f)
            {
                // 警告画像の削除
                _warningDisplayTimer--;
                if (_warningDisplayTimer < 0)
                {
                    // コントローラーの数の取得
                    if (_controllerNum >= 2)
                    {
                        iscontroller = true;
                    }
                    _isReacquisition = true;
                    _warningDisplayTimer = 60;

                }
            }
            if (_input[0].UI.Cancel.WasPerformedThisFrame())
            {
                _isWarningsDisplay = true;
            }
        }
        return iscontroller;
    }
    // 警告画像の削除処理
    private void WarningDisplayDestory()
    {
        if (_isWarningsDisplay && _isPrevWarningsDisplay != _isWarningsDisplay)
        {
            _select.Decision(false);
            _isWarningsDisplay = false;
            Destroy(_warninigObject);
            _isWarningDestory = true;
        }
        _isPrevWarningsDisplay = _isWarningsDisplay;
    }
    public void ImageScaleChenge(int num)
    {
        if (!_isScale)
        {
            _imageScaleNum = num;
            _isScale = true;
            _select.Decision(true);
            _image[_imageScaleNum].transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutCirc);
        }
    }
    public void ImageScaleDefalutChenge()
    {
        if (_isScale)
        {
            _isScale = false;
            _image[_imageScaleNum].transform.DOScale(_defaultScale, 0.5f).SetEase(Ease.OutCirc);
        }
    }
    public void DifficultyDisplay()
    {
        if (!_difficulty)
        {
            _select.Decision(true);
            _difficulty = Instantiate(_difficultyObject);
            _difficulty.transform.SetParent(_canvas.transform, false);
            _difficulty.transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutExpo);
            //_difficulty.GetComponent<DifficultyManager>()._input = _input;
        }
    }
    // 削除処理
    public void DifficultyDisplayDestory()
    {
        if (_difficulty)
        {
            _isDifficultyDestory = _difficulty.GetComponent<DifficultyManager>()._isButton;
            if (_difficulty.GetComponent<DifficultyManager>()._isButton)
            {
                _select.Decision(false);
                Destroy(_difficulty);
                //_difficulty.GetComponent<DifficultyManager>()._isButton = false;
            }
        }
    }
    // 現在難易度選択が表示されているかどうか
    public bool NowDifficultyDisplay()
    {
        bool display = false;
        if (_difficulty)
        {
            display = _difficulty.GetComponent<DifficultyManager>()._isPushButton;
        }
        return display;
    }
    public bool IsDifficultyObj()
    {
        return _difficulty;
    }
    public bool IsDifficultyDestory()
    {
        return _isDifficultyDestory;
    }
    private void InputCheck()
    {
        if (_warninigObject)
        {
            if (_controllerNum >= 2)
            {
                GameObject child = _warninigObject.transform.GetChild(2).gameObject;
                var color = Color.white;
                color.a = 255;
                child.GetComponent<Image>().color = color;
            }
        }
    }
}
