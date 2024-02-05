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
    // 警告画像の表示時間
    private int _warningDisplayTimer = 60;
    // 表示の削除のフラグ
    private bool _isDisplay = false;
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
    // Start is called before the first frame update
    void Start()
    {
        //GetControllerInit();
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
        SelectSceneUpdate();
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
                //_setting.SettingCanvasClose();
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

        DisplayDestory();
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

    public bool DisplayUpdate()
    {
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
                    if (_input.Length >= 2)
                    {
                        return true;
                    }
                    _isReacquisition = true;
                    _warningDisplayTimer = 60;

                }
            }
            if (_input[0].UI.Cancel.WasPerformedThisFrame() || _input[0].UI.Submit.WasPerformedThisFrame())
            {
                _isDisplay = true;
            }
        }
        return false;
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
            if (_controllerNum >= 2)
            {
                GameObject child = _warninigObject.transform.GetChild(2).gameObject;
                var color = Color.white;
                color.a = 255;
                Debug.Log(color.a);
                child.GetComponent<Image>().color = color;
            }
        }
    }
}
