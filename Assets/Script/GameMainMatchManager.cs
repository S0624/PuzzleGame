using UnityEngine;

public class GameMainMatchManager : MonoBehaviour
{
    // 画像をいれる
    [Header("表示させる画像たち")]
    [SerializeField] private GameObject _GameStartImg;
    [SerializeField] private GameObject GameOverImg;
    [SerializeField] private Transform[] _gameImgPos;
    [SerializeField] private GameObject[] _imgPos;
    [SerializeField] private GameObject AllClearImg;
    [SerializeField] private GameObject GameWinImg;
    // Canvasを入れるよう
    [SerializeField] private GameObject Canvas;
    private GameObject _gameStartText = null;
    private GameObject _gameOverImg = null;
    private GameObject _gameWinImg = null;
    private GameObject[] _allClearTex = new GameObject[2];
    // オブジェクトの取得.
    public ColorSeedCreate _seed;
    public SphereColorManager[] _colorManager;
    public FieldData[] _fieldData;
    public SphereMove[] _moveSphere;
    public TestText[] _testText;
    public PauseController _pause;
    public GameStartController _startCanvas;
    public LoadSceneManager _scene;
    public CreateFish _fish;
    // ゲームスタートの時のテキストを表示させたかどうかのフラグを取得する
    private bool _isStartInit = false;
    private bool _isGameStartText = false;
    // お邪魔スフィアの管理用の変数.
    private int[] _obstacle = new int[2];
    private int[] _obstacleAdd = new int[2];
    private int _obstacleCount = 0;
    private int[] _obstaclePrev = new int[2];
    private int _obstacleMax = 15;
    // 計算のフラグ.
    private bool _isLimit = false;
    // ゲームオーバーかどうかのフラグを取得する.
    private bool _isGameOver = false;
    // サウンドマネージャーの取得
    private SoundManager _soundManager;

    // てすと
    private bool _isCalculation = false;
    private int _total = 0;
    private int[] _isSetCount = new int[2];
    private bool _isAdd = false;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _allClearTex.Length; i++)
        {
            _allClearTex[i] = null;
        }
        _seed.InitColor();
        foreach (var col in _colorManager)
        {
            col.SetColorSeed(_seed);
            col.InitObjectName();
            col.ColorRandam();
        }
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
            // 全消しになったら画像を表示
            GenereteAllClear();
            // 魚の更新
            _fish.FishUpdate();
            // 勝利したら画像を表示
            GenereteGameWin();
            // ゲームオーバーになったら処理を止めるよ.
            if (_isGameOver) return;
            // スフィアの回転処理.
            // キューブの移動処理.
            for (int i = 0; i < _moveSphere.Length; i++)
            {
                // セットしていなかったらうごかせる.
                if (!_fieldData[i].IsSetSphere())
                {
                    _moveSphere[i].SphereUpdate();
                }
            }
            for (int i = 0; i < _moveSphere.Length; i++)
            {
                // 動いてなかったら生成.
                if (!_fieldData[i].MoveObstacleSphere())
                {
                    _moveSphere[i].InstallationProcess(_fieldData[i].IsSetSphere(), _fieldData[i]);
                }
                // スフィアを再生成できるかのフラグが成立していたら
                if (_moveSphere[i]._isRegeneration)
                {
                    // カウントをリセットす
                    _moveSphere[i]._isRegeneration = false;
                    Debug.Log("とおってる？そのに。");
                    _isSetCount[i] = 0;
                }
            }
            foreach (var field in _fieldData)
            {
                // フィールドの更新処理.
                field.FieldUpdate();
            }
            // お邪魔計算.
            ObstacleCalculation();
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
        if (_startCanvas.IsStartCanvas()) return;
        if (GenereteGameStart()) return;
        // ポーズ画面を開いていたら処理を止める.
        if (_pause.IsPause()) return;
        // ゲームオーバーになったら処理を止めるよ.
        if (_isGameOver) return;
        // キューブの移動処理.
        for (int i = 0; i < _moveSphere.Length; i++)
        {
            // セットしていなかったらうごかせる.
            if (!_fieldData[i].IsSetSphere())
            {
                if(i == 0)
                {
                    Debug.Log("ぜろだよ");
                }
                //_isSetCount[i] = 0;
                _moveSphere[i].FreeFallUpdate();
            }
        }
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
                foreach (var move in _moveSphere)
                {
                    move.SphereInit();
                    move.SphereReGenerete();
                }
                _isStartInit = true;
            }
        }
        return false;
    }
    // テスト用 ゲームオーバーになったら画像を表示
    private void GenereteGameOver()
    {
        if (_gameOverImg == null)
        {
            for (int i = 0; i < _fieldData.Length; i++)
            {
                if (_fieldData[i].IsGameOver())
                {
                    _isGameOver = true;
                    _gameOverImg = Instantiate(GameOverImg);
                    _gameOverImg.transform.SetParent(_gameImgPos[i], false);
                }
            }
        }
    }
    // テスト用 ゲームオーバーになってないほうにやったの画像を表示
    private void GenereteGameWin()
    {
        if (_gameWinImg == null && _isGameOver)
        {
            for (int i = 0; i < _fieldData.Length; i++)
            {
                if (!_fieldData[i].IsGameOver())
                {
                    _gameWinImg = Instantiate(GameWinImg);
                    _gameWinImg.transform.SetParent(_gameImgPos[i], false);
                }
            }
        }
    }
    // テスト用 全消しになったら画像を表示
    private void GenereteAllClear()
    {
        for (int i = 0; i < _fieldData.Length; i++)
        {
            if (_allClearTex[i] == null)
            {
                if (_fieldData[i].FieldAllClear())
                {
                    _soundManager.SEPlay(SoundSEData.AllClear);
                    Debug.Log("ぜんけし");
                    _allClearTex[i] = Instantiate(AllClearImg, _imgPos[i].transform.position, Quaternion.identity);
                }
            }
            else
            {
                if (!_fieldData[i].FieldAllClear())
                {
                    Destroy(_allClearTex[i]);
                }
            }
        }
    }
    // 邪魔スフィアの計算(相殺処理)
    private void ObstacleCalculation()
    {
        //        // 連鎖が終わった時に落とす.
        //        int[] add = new int[2];
        //        int total = 0;
        //        for (int i = 0; i < _fieldData.Length; i++)
        //        {
        //            // とりあえず値を取得するよ.
        //            _obstacleAdd[i] = _fieldData[i].GetObstacle();
        //            // 今持っている数値が大きかったら代入するよ
        //            if (_obstacle[i] < _obstacleAdd[i])
        //            {
        //                _obstacle[i] = _obstacleAdd[i];
        //            }
        //        }

        //        // 計算するよ
        //        // 減らしていく処理はまだ、つまりまだこれは計算だけなの
        //        Debug.Log("あぶれたひと" + _obstacleCount);
        //        if (_isLimit && !_isCalculation)
        //        {
        //            Debug.Log("ここに通ってる");
        //            total = (_obstacle[0] - _obstacle[1]) + _obstacleCount;
        //            //_calculation = false;
        //            _isCalculation = true;
        //            _obstacleCount = 0;
        //        }
        //        else
        //        {
        //            //Debug.Log("ここに通ってる");
        //            total = (_obstacle[0] - _obstacle[1]);
        //            //_obstacleCount = 0;
        //        }
        //        Debug.Log("いず" + _isLimit + "いず" + _isCalculation);

        //        // 最大数は30にしたいので30より多い数を送らないようにする.
        //        total = MaxLimit(total);

        //#if true
        //        // デバック用におじゃまの数表示
        //        TestObsText(total);
        //#endif
        //        //Debug.Log(_testController[0].IsInstallaion());
        //        if (total == 0)
        //        {
        //            //Debug.Log("均衡中...");
        //        }
        //        else if (total > 0)
        //        {
        //            //Debug.Log(_testController[1].IsInstallaion());
        //            if (!_fieldData[0].IsFieldUpdate() && _fieldData[1].IsInstallaion())
        //            {
        //                _fieldData[1].SetObstacle(total);
        //                _fieldData[1].GetInstallation(false);
        //                //Debug.Log("右に" + total + "と" + _obstacleCount);
        //            }
        //            else
        //            {
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            if (!_fieldData[1].IsFieldUpdate() && _fieldData[0].IsInstallaion())
        //            {
        //                _fieldData[0].SetObstacle(total * -1);
        //                Debug.Log("おくるよ");
        //                _fieldData[0].GetInstallation(false);

        //            }
        //            else
        //            {
        //                return;
        //            }
        //        }
        //        // 初期化
        //        for (int i = 0; i < _fieldData.Length; i++)
        //        {
        //            // とりあえず値を取得するよ.
        //            _obstacle[i] = 0;
        //            _obstacleAdd[i] = 0;
        //        }
        //        _isCalculation = false;

        //        //_calculation = false;
        Test();
    }
    // 最大数の制限処理
    private int MaxLimit(int total)
    {
        //if (_obstacleCount + _obstacleMax != _obstaclePrevCount && !_isAdd)
        //{
        //    Debug.Log("だぁ");
        //    total += _obstacleCount;
        //    _isAdd = true;
        //}
        //_obstaclePrevCount = total;
        // 最大数は30にしたいので30より多い数を送らないようにする.
        if (total > _obstacleMax)
        {
            _obstacleCount = total - _obstacleMax;
            total = _obstacleMax;
            _isLimit = true;
        }
        else if (total < -_obstacleMax)
        {
            _obstacleCount = total + _obstacleMax;
            //_obstacleCount *= -1;
            total = -_obstacleMax;
            _isLimit = true;
        }
        else
        {
            //_obstacleCount = 0;
            _isLimit = false;
        }
        return total;
    }
    // テスト用実装
    private void Test()
    {
        // 連鎖が終わった時に落とす.

        for (int i = 0; i < _fieldData.Length; i++)
        {
            // とりあえず値を取得するよ.
            _obstacleAdd[i] = _fieldData[i].GetObstacle();
            // 今持っている数値が大きかったら代入するよ
            if (_obstacle[i] < _obstacleAdd[i])
            {
                _obstacle[i] = _obstacleAdd[i];
            }
            
            if (_obstaclePrev[i] != _obstacle[i])
            {
                _total += (_obstacle[0] - _obstacle[1]);
            }
            _obstaclePrev[i] = _obstacle[i];
        }
        //// 計算するよ
        //// 減らしていく処理はまだ、つまりまだこれは計算だけなの
        //_total = (_obstacle[0] - _obstacle[1]) + _obstacleCount;
        //if (_isLimit && _isCalculation)
        //{
        //    Debug.Log("ここに通ってる");
        //    _total = (_obstacle[0] - _obstacle[1]) + _obstacleCount;
        //    //_obstacleCount = 0;
        //    _isCalculation = false;
        //}
        //else
        //{

        //if (!_isAdd)
        //{
        //if (_total > _obstacleCount)
        
        {
            //_total += (_obstacle[0] - _obstacle[1]);
        }
        //else
        {
            //_total = (_obstacle[0] - _obstacle[1]);
        }
        //_isAdd = true;
        //}

        //// 最大数は30にしたいので30より多い数を送らないようにする.
        _total = MaxLimit(_total);
        //Debug.Log(_total + "おじゃま" + _obstacleCount);
        //Debug.Log(_total);
#if true
        // デバック用におじゃまの数表示
        TestObsText();
#endif

        if (_total > 0)
        {
            if (!_fieldData[0].IsFieldUpdate() && _fieldData[1].IsInstallaion() && _isSetCount[1] == 0)
            {
                _fieldData[1].SetObstacle(_total);
                _fieldData[1].GetInstallation();
                _isCalculation = true;
                _isSetCount[1]++;
                //_obstacle[0] = 0;
            }
            else if(_fieldData[1].IsInstallaion())
            {
                _fieldData[1].GetInstallation();
                return;

            }
            else
            {
                return;
            }
        }
        else if (_total < 0)
        {
            if (!_fieldData[1].IsFieldUpdate() && _fieldData[0].IsInstallaion()　&& _isSetCount[0] == 0)
            {
                _fieldData[0].SetObstacle(_total * -1);
                _fieldData[0].GetInstallation();
                _isCalculation = true;
                _isSetCount[0]++;
                Debug.Log("とおってる？");
                //_obstacle[1] = 0;

            }
            else if (_fieldData[0].IsInstallaion())
            {
                _fieldData[0].GetInstallation();
                return;

            }
            else
            {
                return;
            }
        }

        // 初期化
        for (int i = 0; i < _fieldData.Length; i++)
        {
            // とりあえず値を取得するよ.
            _obstacle[i] = 0;
            _obstacleAdd[i] = 0;
        }
        _isAdd = false;
        //_total = 0;
        //_calculation = false;
        _total = _obstacleCount;
        _obstacleCount = 0;
    }

    // テキスト用
    private void TestObsText()
    {
        if (_total < 0  || _obstacleCount < 0)
        {
            _testText[0].SetObstacleCount((_total + _obstacleCount) * -1);
        }
        else if (_total > 0 || _obstacleCount > 0)
        {
            _testText[1].SetObstacleCount(_total + _obstacleCount);
        }
        else
        {
            _testText[0].SetObstacleCount(0);
            _testText[1].SetObstacleCount(0);
        }

    }
    //// テキスト用
    //private void TestObsText(int total)
    //{
    //    if (total < 0)
    //    {
    //        _testText[0].SetObstacleCount((total + _obstacleCount) * -1);
    //    }
    //    else if (total > 0)
    //    {
    //        _testText[1].SetObstacleCount(total + _obstacleCount);
    //    }
    //    else
    //    {
    //        _testText[0].SetObstacleCount(0);
    //        _testText[1].SetObstacleCount(0);
    //    }

    //}
}
