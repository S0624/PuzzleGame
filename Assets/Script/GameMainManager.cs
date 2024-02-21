using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainManager : MonoBehaviour
{
    // 画像をいれる
    [Header("表示させる画像たち")]
    [SerializeField] private GameObject _GameStartImg;
    [SerializeField] private GameObject GameOverImg;
    [SerializeField] private GameObject AllClearImg;
    [SerializeField] private GameObject _allClearEffect;
    // Canvasを入れるよう
    [SerializeField] private GameObject Canvas;
    private GameObject _gameStartText = null;
    private GameObject _gameOverTex = null;
    private GameObject _allClearTex = null;
    // オブジェクトの取得.
    public ColorSeedCreate _seed;
    public SphereColorManager _colormanager;
    public FieldData _field;
    public SphereMove _move;
    public PauseController _pause;
    public GameStartController _startCanvas;
    public LoadSceneManager _scene;
    public CreateFish _fish;
    // ゲームスタートの時のテキストを表示させたかどうかのフラグを取得する
    private bool _isGameStartText = false;
    private bool _isStartInit = false;
    // ゲームオーバーかどうかのフラグを取得する.
    private bool _isGameOver = false;
    // サウンドマネージャーの取得
    private SoundManager _soundManager;
    // Start is called before the first frame update
    void Start()
    {
        _seed.InitColor();
        _colormanager.SetColorSeed(_seed);
        _colormanager.InitObjectName();
        _colormanager.ColorRandam();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        // ゲームスタート画面の更新処理.
        if (_startCanvas.IsStartCanvas())
        {
            _startCanvas.StartSettingUpdate();
            return;
        }
        if (GenereteGameStart()) return;
        // ポーズ画面を開いていたら処理を止める.
        if (!_pause.IsPause())
        {
            // ゲームオーバーになったら画像を表示
            GenereteGameOver();
            GenereteAllClear();
            _fish.FishUpdate();
            // ゲームオーバーになったら処理を止めるよ.
            if (_isGameOver) return;
            // キューブの回転処理.
            if (!_field.IsSetSphere())
            {
                _move.SphereUpdate();
            }
            _move.InstallationProcess(_field.IsSetSphere(), _field);
            // スフィアを再生成できるかのフラグが成立していたら
            if (_move._isRegeneration)
            {
                // カウントをリセットする
                _move._isRegeneration = false;
            }
            // フィールドの更新処理.
            _field.FieldUpdate();
        }
        // pauseの更新処理.
        _pause.PauseUpdate();
        if (_pause.IsSelectPush())
        {
            _scene.PauseTransitionScene();
        }
    }
    void FixedUpdate()
    {
        // ゲームスタート画面の更新処理.
        if (_startCanvas.IsStartCanvas())   return;
        if(GenereteGameStart()) return;
        // ポーズ画面を開いていたら処理を止める.
        if (_pause.IsPause()) return;
        // ゲームオーバーになったら処理を止めるよ.
        if (_isGameOver) return;
        // キューブの移動処理.
        _move.FreeFallUpdate();
    }
    // ゲーム開始時に画像を表示
    private bool GenereteGameStart()
    {
        if (!_isGameStartText)
        {
            _gameStartText = Instantiate(_GameStartImg);
            _isGameStartText = true;
        }
        if (_gameStartText)
        {
            return true;
        }
        else
        {
            if (!_isStartInit)
            {
                // 一回だけ初期化処理を行うよ
                _move.SphereInit();
                _move.SphereReGenerete();
                _isStartInit = true;
            }
        }
        return false;
    }
    private void GenereteGameOver()
    {
        if (_gameOverTex == null)
        {
            if (_field.IsGameOver())
            {
                _soundManager.SEPlay(SoundSEData.GameOver);
                _isGameOver = true;
                _gameOverTex = Instantiate(GameOverImg);
                _gameOverTex.transform.SetParent(Canvas.transform, false);
            }
        }
    }
    // 全消しになったら画像を表示
    private void GenereteAllClear()
    {
        if (_allClearTex == null)
        {
            if (_field.FieldAllClear())
            {
                _soundManager.SEPlay(SoundSEData.AllClear);
                _allClearTex = Instantiate(AllClearImg);
                Instantiate(_allClearEffect);
                //_allClearTex.transform.SetParent(Canvas.transform, false);
            }
        }
        else
        {
            if (!_field.FieldAllClear())
            {
                Destroy(_allClearTex);
            }
        }
    }
}
