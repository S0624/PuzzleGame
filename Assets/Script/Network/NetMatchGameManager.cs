using Photon.Pun;
using TMPro;
using UnityEngine;

public class NetMatchGameManager : MonoBehaviourPunCallbacks
{
    // 画像をいれる
    [Header("表示させる画像たち")]
    [SerializeField] private GameObject _GameStartImg;
    [SerializeField] private GameObject GameOverImg;
    [SerializeField] private Transform[] _gameImgPos;
    [SerializeField] private GameObject[] _imgPos;
    [SerializeField] private GameObject AllClearImg;
    [SerializeField] private GameObject GameWinImg;
    [SerializeField] private GameObject[] _allClearEffect;
    // Canvasを入れるよう
    [SerializeField] private GameObject Canvas;
    private GameObject _gameStartText = null;
    private GameObject _gameOverImg = null;
    private GameObject _gameWinImg = null;
    private GameObject[] _allClearTex = new GameObject[2];
    // オブジェクトの取得.
    public FieldData[] _fieldData;
    public SphereMove[] _moveSphere;
    public TestText[] _obstacleText;
    public GameStartController _startCanvas;
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
    // ゲームオーバーかどうかのフラグを取得する.
    private bool _isGameOver = false;
    // サウンドマネージャーの取得
    private SoundManager _soundManager;

    // おじゃまのトータル数
    private int _total = 0;
    // お邪魔を落としたというフラグ
    private bool[] _isSetFall = new bool[2];
    // 左のプレイヤー
    private int _leftPlayer = (int)PlayerNumber.LeftPlayer;
    // 右のプレイヤー
    private int _rightPlayer = (int)PlayerNumber.RightPlayer;

    // ボタンの取得
    private bool _isDecisionButtonPush = false;
    // オブジェクトの取得.
    public ColorSeedCreate _seed;
    public SphereColorManager[] _colorManager;
    private bool _isColorInit = false;
    public SphereMove[] _sphere;

    // テスト用名前の表示
    public TextMeshProUGUI[] _text;
    private Vector2 _testPos = Vector2.zero;

    private Vector2Int _tempPos = Vector2Int.zero;
    private Vector2Int _localSpherePos = Vector2Int.zero;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _allClearTex.Length; i++)
        {
            _allClearTex[i] = null;
        }
        ColorSeedInit();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    // Update is called once per frame
    void Update()
    {
        ColorSeedInit();


        // ネットワークテスト用関数
        TestUpdate();
        NetTestUpdate();

    }
    

    private void FixedUpdate()
    {
        // リストの情報を取得
        var players = PhotonNetwork.PlayerList;
        PhotonNetwork.LocalPlayer.ButtonDown(_isDecisionButtonPush);
        int actor = 0;
        foreach (var player in players)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == player.ActorNumber)
            {
                PhotonNetwork.LocalPlayer.SetSphereCoordinate(_sphere[actor]._spherePos);
            }
            actor++; ;
        }
        PhotonNetwork.LocalPlayer.CustomUpdate();
        if (photonView.IsMine)
        {
            //foreach (var player in players)
            //{
            //    Debug.Log($"{player.NickName}({player.ActorNumber}) - {player.GetButtonState()}");
            //}
        }
        foreach (var player in players)
        {
            Debug.Log($"{player.NickName}({player.ActorNumber}) - {player.GetButtonState()}{player.GetSphereCoordinate()}");
        }
        // テスト用
        for (int i = 0; i < players.Length; i++)
        {
            _text[i].text = players[i].GetButtonState().ToString();
        }
        int test = 0;
        foreach (var player in players)
        {
            Debug.Log("おなかすいた" + player.ActorNumber);
            if (PhotonNetwork.LocalPlayer.ActorNumber != player.ActorNumber)
            {
                var pos = player.GetSphereCoordinate();
                Vector2Int testa = Vector2Int.zero;
                testa.x = (int)pos.x;
                testa.y = (int)pos.y;
                _sphere[test]._spherePos = testa;
                
                Debug.Log("ラメ" + pos + "ｔら" + _sphere[test]._spherePos);
                _localSpherePos = testa - _tempPos;

                _sphere[test].SpherePos(_localSpherePos.x, _localSpherePos.y);
                _tempPos = testa;
            }
            test++;
        }
        TestFixUpdate();
    }
    private void TestUpdate()
    {
        // ゲームスタート画面の更新処理.
        if (_startCanvas.IsStartCanvas())
        {
            _startCanvas.StartSettingUpdate();
            return;
        }
        if (GenereteGameStart()) return;

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
                if (_moveSphere[i]._playerIndex >= 0)
                {
                    _moveSphere[i].SphereUpdate();
                }
            }
        }
        for (int i = 0; i < _moveSphere.Length; i++)
        {
            // 動いてなかったら生成.
            if (!_fieldData[i].MoveObstacleSphere())
            {
                //if (_moveSphere[i]._playerIndex >= 0)
                {
                    _moveSphere[i].InstallationProcess(_fieldData[i].IsSetSphere(), _fieldData[i]);
                }
            }
            // スフィアを再生成できるかのフラグが成立していたら
            if (_moveSphere[i]._isRegeneration)
            {
                // カウントをリセットす
                _moveSphere[i]._isRegeneration = false;
                _isSetFall[i] = false;
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
    private void TestFixUpdate()
    {
        // ゲームスタート画面の更新処理.
        if (_startCanvas.IsStartCanvas()) return;
        if (GenereteGameStart()) return;
        // ゲームオーバーになったら処理を止めるよ.
        if (_isGameOver) return;
        // キューブの移動処理.
        for (int i = 0; i < _moveSphere.Length; i++)
        {
            // セットしていなかったらうごかせる.
            if (!_fieldData[i].IsSetSphere())
            {
                if (_moveSphere[i]._playerIndex >= 0)
                {
                    _moveSphere[i].FreeFallUpdate();
                }
            }
        }
    }
    private void NetTestUpdate()
    {
        // ネットワークテスト用関数
        if (Input.GetKeyDown("z"))
        {
            if (_isDecisionButtonPush)
            {
                _isDecisionButtonPush = false;
            }
            else
            {
                _isDecisionButtonPush = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _testPos.x++;
            Debug.Log("やあ");
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _testPos.x--;
            Debug.Log("うん？");
        }
    }

    /// <summary>
    /// 色の初期化と同期
    /// </summary>
    private void ColorSeedInit()
    {
        if (_isColorInit) return;
        // リストの情報を取得
        var players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == players[0].ActorNumber)
            {
                _seed.InitNetworkColor();
                ColorSeedInin(_seed);
                PhotonNetwork.LocalPlayer.SetCreateSeed(_seed._upSeed, _seed._downSeed);
                Debug.Log("ここにとおってる");
                _isColorInit = true;
                _moveSphere[0]._playerIndex = 0;
                break;
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == players[i].ActorNumber)
            {
                if (_seed.NetworkInitColor(players[i].GetUpDColorSeed(), players[i].GetDownDColorSeed()))
                {
                    Debug.Log("通ってる" + PhotonNetwork.LocalPlayer.ActorNumber);
                    _isColorInit = true;
                    ColorSeedInin(_seed);
                    _moveSphere[1]._playerIndex = 0;
                }
            }
        }
    }
    /// <summary>
    /// 色の種の初期化処理
    /// </summary>
    /// <param name="seed"></param>
    private void ColorSeedInin(ColorSeedCreate seed)
    {
        foreach (var col in _colorManager)
        {
            col.SetColorSeed(seed);
            col.InitObjectName();
            col.ColorRandam();
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
                    Debug.Log("通ってる");
                    _soundManager.SEPlay(SoundSEData.AllClear);
                    _allClearTex[i] = Instantiate(AllClearImg, _imgPos[i].transform.position, Quaternion.identity);
                    Instantiate(_allClearEffect[i]);
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
                _total += (_obstacle[_leftPlayer] - _obstacle[_rightPlayer]);
            }
            _obstaclePrev[i] = _obstacle[i];
        }
        //// 最大数は30にしたいので30より多い数を送らないようにする.
        _total = MaxLimit(_total);
#if true
        // デバック用におじゃまの数表示
        ObsText();
#endif

        if (_total > 0)
        {
            if (!_fieldData[_leftPlayer].IsFieldUpdate() && _fieldData[_rightPlayer].IsInstallaion() && !_isSetFall[_rightPlayer])
            {
                if (!_fieldData[_rightPlayer].IsFieldUpdate())
                {
                    _fieldData[_rightPlayer].SetObstacle(_total);
                    _fieldData[_rightPlayer].GetInstallation();
                    _isSetFall[_rightPlayer] = true;
                }
                else
                {
                    return;
                }
            }
            else if (_fieldData[_rightPlayer].IsInstallaion())
            {
                _fieldData[_rightPlayer].GetInstallation();
                return;

            }
            else
            {
                return;
            }
        }
        else if (_total < 0)
        {
            if (!_fieldData[_rightPlayer].IsFieldUpdate() && _fieldData[_leftPlayer].IsInstallaion() && !_isSetFall[_leftPlayer])
            {
                if (!_fieldData[_leftPlayer].IsFieldUpdate())
                {
                    _fieldData[_leftPlayer].SetObstacle(_total * -1);
                    _fieldData[_leftPlayer].GetInstallation();
                    _isSetFall[_leftPlayer] = true;
                }
                else
                {
                    return;
                }
            }
            else if (_fieldData[_leftPlayer].IsInstallaion())
            {
                _fieldData[_leftPlayer].GetInstallation();
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
        _total = _obstacleCount;
        _obstacleCount = 0;
    }
    // 最大数の制限処理
    private int MaxLimit(int total)
    {
        // 最大数は30にしたいので30より多い数を送らないようにする.
        if (total > _obstacleMax)
        {
            _obstacleCount = total - _obstacleMax;
            total = _obstacleMax;
        }
        else if (total < -_obstacleMax)
        {
            _obstacleCount = total + _obstacleMax;
            total = -_obstacleMax;
        }
        return total;
    }
    // テキスト用
    private void ObsText()
    {
        if (_total < 0 || _obstacleCount < 0)
        {
            _obstacleText[_leftPlayer].SetObstacleCount((_total + _obstacleCount) * -1);
        }
        else if (_total > 0 || _obstacleCount > 0)
        {
            _obstacleText[_rightPlayer].SetObstacleCount(_total + _obstacleCount);
        }
        else
        {
            _obstacleText[_leftPlayer].SetObstacleCount(0);
            _obstacleText[_rightPlayer].SetObstacleCount(0);
        }
    }
}
