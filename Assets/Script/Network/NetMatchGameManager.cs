using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
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
    private bool _isGameStart = true;
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
    private bool _isStop = false;
    private int _obstacleCount = 0;
    private int[] _obstaclePrev = new int[2];
    private int[] _obstacleAddPrev = new int[2];
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
    private　bool _isTestUpdate = false;
    private Vector2 _prevTemppos = Vector2.zero;
    private int _prevDir = 0;

    private Vector2Int _tempPos = Vector2Int.zero;
    private Vector2Int _localSpherePos = Vector2Int.zero;
    private bool _isTestSet = false;
    //private bool _isSetFlag = false;
    private bool[] _isSetFlag = new bool[2];

    private int _testFrame = 1;
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
        GameFixUpdate();

        // リストの情報を取得
        var players = PhotonNetwork.PlayerList;
        PhotonNetwork.LocalPlayer.ButtonDown(_isDecisionButtonPush);
        // 何か変更があった時のみ同期させるためにフラグを用意
        bool isupdate = false;
        // 何番目か
        int actor = 0;
        foreach (var player in players)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == player.ActorNumber)
            {
                // 取得する処理
                if (player.IsGetSphereCoordinate() != _sphere[actor]._spherePos)
                //if (player.GetSphereCoordinate() != _sphere[actor]._spherePos && !_isTestUpdate && !_isTestSet)
                {
                    PhotonNetwork.LocalPlayer.IsSetSphereCoordinate(_sphere[actor].transform.localPosition);
                    isupdate = true;
                }
                ////てすと
                //if (player.GetSphereCoordinate() != _sphere[actor]._spherePosTemp)
                ////if (player.TestGetSphereCoordinate() != _sphere[actor]._spherePosTemp || player.TestGetSphereCoordinate().x == -1)
                //{
                //    if(player.GetSphereCoordinate().x == -1)
                //    {
                //        _isTestSet = false;
                //    }

                //    //Debug.Log("はずれ" + _sphere[actor]._spherePosTemp);
                //    PhotonNetwork.LocalPlayer.SetSphereCoordinate(_sphere[actor]._spherePosTemp);
                //    isupdate = true;
                //}
                if (player.GetSphereDirection() != _sphere[actor]._direction)
                {
                    PhotonNetwork.LocalPlayer.SetSphereDirection(_sphere[actor]._direction);
                    isupdate = true;
                }
                if (player.GetSphereColor() != _sphere[actor]._direction)
                {
                    PhotonNetwork.LocalPlayer.SetSphereDirection(_sphere[actor]._direction);
                    isupdate = true;
                }

                if (player.GetSphereColor() != _colorManager[actor]._seedCount)
                {
                    // テスト用
                    PhotonNetwork.LocalPlayer.SetSphereColor(_colorManager[actor]._seedCount);
                    isupdate = true;
                }
                //if (Input.GetKeyDown("z"))
                //if (player.GetObstaclWidth(actor) != _fieldData[actor]._tempBoard)
                if (player.GetFieldData(actor) != _fieldData[actor].NetFieldUpdate())
                {
                    //Debug.Log(player.GetObstaclWidth()[0,0]);
                    // テスト用
                    PhotonNetwork.LocalPlayer.SetFieldData(_fieldData[actor].NetFieldUpdate());
                    isupdate = true;

                    //Debug.Log(_fieldData[actor]._isSetEnd);
                }
                if (player.GetObstacle() != _obstacleAdd[actor])
                {
                    //if (_fieldData[actor].GetObstacle() != 0)
                    {
                        //Debug.Log(player.GetObstaclWidth()[0,0]);
                        // テスト用
                        PhotonNetwork.LocalPlayer.SetObstacleData(_obstacleAdd[actor]);
                        Debug.Log("わんだほい");
                        isupdate = true;
                    }

                }
                    //    //Debug.Log(_fieldData[actor]._isSetEnd);
                    //}

                    //if (player.GetObstacle() != _fieldData[actor].GetObstacle())
                    //{
                    //    if (_fieldData[actor].GetObstacle() != 0)
                    //    {
                    //        //Debug.Log(player.GetObstaclWidth()[0,0]);
                    //        // テスト用
                    //        PhotonNetwork.LocalPlayer.SetObstacleData(_fieldData[actor].GetObstacle());
                    //        _obstacle[actor] = _fieldData[actor].GetObstacle();
                    //        if (_isStop)
                    //        {
                    //            _obstacleAdd[actor] += _obstacle[actor];
                    //        }
                    //        else
                    //        {
                    //            _obstacleAdd[actor] = _obstacle[actor];
                    //        }
                    //        Debug.Log("とおった" + _fieldData[actor].GetObstacle() + _obstacleAdd[actor]);

                    //        isupdate = true;
                    //        _isStop = false;
                    //    }
                    //    else
                    //    {
                    //        _isStop = true;
                    //    }

                    //    //Debug.Log(_fieldData[actor]._isSetEnd);
                    //}

                    //if (player.GetSphereSet() != _fieldData[actor]._isSetEnd)
                    //{
                    //    // テスト用
                    //    if (player.GetSphereSet()) {_isTestUpdate = true;}
                    //    PhotonNetwork.LocalPlayer.SetSphereSet(_fieldData[actor]._isSetEnd);
                    //    isupdate = true;
                    //    Debug.Log(_fieldData[actor]._isSetEnd);
                    //}

                    //if (player.GetSphereSet() != _fieldData[actor]._isSetFlag)
                    //{
                    //    // テスト用
                    //    if (player.GetSphereSet()) {_isTestUpdate = true;}
                    //    PhotonNetwork.LocalPlayer.SetSphereSet(_fieldData[actor]._isSetFlag);
                    //    isupdate = true;
                    //    Debug.Log(_fieldData[actor]._isSetFlag);
                    //}
                    //Debug.Log("てってれー" + _fieldData[actor]._isSetEnd);
                }
            actor++;
        }
        if(isupdate) 
        {
            PhotonNetwork.LocalPlayer.CustomUpdate();
        }
        //if (photonView.IsMine)
        //{
        //    //foreach (var player in players)
        //    //{
        //    //    Debug.Log($"{player.NickName}({player.ActorNumber}) - {player.GetButtonState()}");
        //    //}
        //}

        // テスト用
        for (int i = 0; i < players.Length; i++)
        {
            _text[i].text = players[i].GetButtonState().ToString();
        }
        int playernum = 0;
        foreach (var player in players)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != player.ActorNumber)
            {
                // テスト
                _sphere[playernum]._spherePos = new Vector2Int(3, 11);
                // 位置の代入
                var pos = player.IsGetSphereCoordinate();
                Vector2Int temppos = Vector2Int.zero;
                temppos.x = (int)pos.x;
                temppos.y = (int)pos.y;
                _sphere[playernum]._spherePos = temppos;

                //_localSpherePos = temppos - _tempPos;

                //_sphere[playernum].SpherePos(_localSpherePos.x, _localSpherePos.y);
                _sphere[playernum].NetSpherePos();
                //_sphere[playernum]._spherePos = temppos;

                //_tempPos = temppos;

                // 方向の代入
                var dir = player.GetSphereDirection();
                _sphere[playernum]._direction = dir;
                _sphere[playernum].SphereRotation();

                //_colorManager[0]._seedCount++;
                
                // 設置フラグの代入
                //if (player.GetSphereCoordinate().x != -1)
                //{
                    //_sphere[playernum]._spherePosTemp.x = (int)player.GetSphereCoordinate().x;
                    //_sphere[playernum]._spherePosTemp.y = (int)player.GetSphereCoordinate().y;
                    //Debug.Log("とっと" + _isTestSet);

                    //if (_prevDir != dir)
                    //{
                    //    _sphere[playernum]._direction = _prevDir;
                    //}

                    //if (!_isTestSet)
                    //{
                    //    if (_isGameStart) return;
                    //    //if (!player.GetSphereSet()) return;
                    //    _sphere[playernum]._spherePos.x = _sphere[playernum]._spherePosTemp.x;
                    //    _sphere[playernum]._isSetInt = true;
                    //    _sphere[playernum].Installation();
                    //    _moveSphere[playernum].InstallationProcess(_fieldData[playernum].IsSetSphere(), _fieldData[playernum]);
                    //    //_moveSphere[playernum].InstallationProcessTest(_fieldData[playernum].IsSetSphere(), _fieldData[playernum]);
                    //    Debug.Log("らら");
                    //    _isTestSet = true;
                    //    _sphere[playernum]._spherePos = new Vector2Int(3, 11);
                    //    _isSetFlag[playernum] = true;

                    //    // テスト用
                    //    PhotonNetwork.LocalPlayer.TestSetIsSphereSet(false);
                    //    PhotonNetwork.LocalPlayer.CustomUpdate();
                    //}
                //}
                // 色の同期処理
                if (player.GetSphereColor() != _colorManager[playernum]._seedCount)
                {
                    _colorManager[playernum]._seedCount = player.GetSphereColor();
                    _sphere[playernum].SphereColorReset();
                }

                if (player.GetFieldData(playernum) != _fieldData[playernum].NetFieldUpdate())
                {
                    for(int a = 0; a < _fieldData[playernum].NetFieldUpdate().Length; a++)
                    {
                        //Debug.Log("aaaa");
                        if (player.GetFieldData(playernum)[a] != _fieldData[playernum].NetFieldUpdate()[a])
                        {
                            _fieldData[playernum].FieldSynchronization(player.GetFieldData(playernum)[a],a);
                        }
                    }
                    //for (int y = 0; y < _fieldData[playernum].testH().Length; y++)
                    //{
                    //    if(player.GetObstaclWidth(playernum)[y] != _fieldData[playernum].testH()[y])
                    //    {
                    //        Debug.Log(y);
                    //    }
                    //        Debug.Log((player.GetObstaclWidth(playernum).Length + "やっほー" + _fieldData[playernum].testH().Length));
                    // }

                    //_fieldData[playernum].testT(player.GetObstaclWidth(playernum));
                }
                if (player.GetObstacle() != _obstacleAdd[playernum])
                {
                    // テスト用
                    _obstacleAdd[playernum] = player.GetObstacle();
                }

                //_obstacle[playernum] = player.GetObstacle();

                //_sphere[playernum]._spherePosTemp = new Vector2Int(-1, -1);
                //PhotonNetwork.LocalPlayer.TestSetSphereCoordinate(_sphere[playernum]._spherePosTemp);
                // 設置フラグの代入
                //var isset = player.GetSphereSet();
                //if (isset && !_isTestSet)
                //{
                //    Vector2Int prevtemppos = Vector2Int.zero;
                //    prevtemppos.x = (int)_prevTemppos.x;
                //    prevtemppos.y = (int)_prevTemppos.y;
                //    if(temppos.y > prevtemppos.y) 
                //    {
                //        _sphere[playernum]._spherePos = prevtemppos;
                //    }
                //    if(_prevDir != dir)
                //    {
                //        _sphere[playernum]._direction = _prevDir;
                //    }
                //    Debug.Log("ばしょ" + _prevTemppos);
                //    _sphere[playernum].Installation();
                //    //_sphere[playernum].NetInstallation(temppos);
                //    _sphere[playernum].test();
                //    _moveSphere[playernum].InstallationProcess(_fieldData[playernum].IsSetSphere(), _fieldData[playernum]);
                //    //_moveSphere[playernum].InstallationProcessTest(_fieldData[playernum].IsSetSphere(), _fieldData[playernum]);
                //    _isTestSet = true;
                //    //_moveSphere[playernum].SphereReGenerete();
                //    _moveSphere[playernum]._spherePos = new Vector2Int(3, 11);
                //    _tempPos = Vector2Int.zero;
                //}

                //_prevTemppos = _tempPos;
                //_prevDir = dir;
                //_fieldData[playernum]._isSetEnd = isset;

                //if (_moveSphere[playernum]._isReGenereteSpher) { _isTestSet = false; }

                //Debug.Log("add" + _fieldData[add]._isSetEnd);

            }
            //else
            //{
            //    //if (_isSetFlag[playernum])
            //    //{
            //    //    Debug.Log("color");
            //    //    // テスト用
            //    //    PhotonNetwork.LocalPlayer.TestSetIsSphereSet(true);
            //    //    PhotonNetwork.LocalPlayer.CustomUpdate();
            //    //}

            //    //_isSetFlag[playernum] = player.TestGetSphereSet();
            //}
            playernum++;
        }

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
        // リストの情報を取得
        var players = PhotonNetwork.PlayerList;
        var add = 0;
        foreach (var player in players)
        {
        // キューブの移動処理.
            if (PhotonNetwork.LocalPlayer.ActorNumber == player.ActorNumber)
            {
                if (_obstaclePrev[add] != _fieldData[add].GetObstacle())
                {
                    _obstacle[add] = _fieldData[add].GetObstacle();
                    if (_obstacle[add] != 0)
                    {
                        Debug.Log("だだ" + _obstacle[add]);
                        // テスト用
                        //if (_isStop)
                        {
                            _obstacleAdd[add] += _obstacle[add];
                        }
                        //else
                        {
                            //_obstacleAdd[add] = _obstacle[add];
                        }

                        _isStop = false;
                    }
                    else
                    {
                        _isStop = true;
                    }
                }
                _obstaclePrev[add] = _obstacle[add];
                    //Debug.Log(_fieldData[actor]._isSetEnd);
                //for (int i = 0; i < _moveSphere.Length; i++)
                {
            // セットしていなかったらうごかせる.
            if (!_fieldData[add].IsSetSphere())
            {
                _moveSphere[add].SphereUpdate();
            }
        }
                //for (int i = 0; i < _moveSphere.Length; i++)
                {
                    // 動いてなかったら生成.
                    if (!_fieldData[add].MoveObstacleSphere())
                    {
                        //if (!_sphere[i]._isSetInt) return;
                        //_moveSphere[i].InstallationProcessTest(_fieldData[i].IsSetSphere(), _fieldData[i]);
                        //if (_testFrame == 0)
                        //{
                        //    _testFrame = 1;
                        //    return;
                        //}
                        //_testFrame--;
                        //_moveSphere[i].InstallationProcessTest(_fieldData[i].IsSetSphere(), _fieldData[i]);
                        // test = _fieldData[i].IsSetSphere();
                        // テスト用
                        //var players = PhotonNetwork.PlayerList;
                        //foreach (var player in players)
                        //{
                        //    var a = 0;
                        //    if (PhotonNetwork.LocalPlayer.ActorNumber != player.ActorNumber && test && !_isSetFlag[a])
                        //    //if (PhotonNetwork.LocalPlayer.ActorNumber != player.ActorNumber && _fieldData[i].IsSetSphere())
                        //    {
                        //        _isSetFlag[a] = false;
                        //        PhotonNetwork.LocalPlayer.TestSetIsSphereSet(true);
                        //        PhotonNetwork.LocalPlayer.CustomUpdate();
                        //    }
                        //    a++;
                        //    //_isSetFlag[a] = false;
                        //}
                        //Debug.Log(_isSetFlag[i]);
                        //if (!_isSetFlag[i]) return;

                        //if (add != 1)
                        //{
                        //    _moveSphere[add].InstallationProcess(_fieldData[add].IsSetSphere(), _fieldData[add]);
                        //}

                        _moveSphere[add].InstallationProcess(_fieldData[add].IsSetSphere(), _fieldData[add]);
                        _isSetFlag[add] = false;
                    }
                    // スフィアを再生成できるかのフラグが成立していたら
                    if (_moveSphere[add]._isRegeneration)
                    {
                        // カウントをリセットす
                        _moveSphere[add]._isRegeneration = false;
                        _isSetFall[add] = false;
                    }
                }
                foreach (var field in _fieldData)
                {
                    // フィールドの更新処理.
                    field.FieldUpdate();
                }
            }
                // お邪魔計算.
                ObstacleCalculation(add);
            add++;
        }
    }
    private void GameFixUpdate()
    {
        // ゲームスタート画面の更新処理.
        if (_startCanvas.IsStartCanvas()) return;
        if (GenereteGameStart()) return;
        // ゲームオーバーになったら処理を止めるよ.
        if (_isGameOver) return;
        // キューブの移動処理.
        for (int i = 0; i < _moveSphere.Length; i++)
        {
            if (_isSetFlag[i]) return;
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
                _isColorInit = true;
                _moveSphere[0]._playerIndex = 0;
                break;
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == players[i].ActorNumber)
            {
                if (_seed.NetworkInitColor(players[i].GetUpDColorSeed(), players[i].GetDownDColorSeed()))
                {
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
            _isGameStart = true;
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
        _isGameStart = false;
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
    private void ObstacleCalculation(int add)
    {
        //////連鎖が終わった時に落とす.
        ////for (int i = 0; i < _fieldData.Length; i++)
        ////{
        ////    //とりあえず値を取得するよ.
        ////if (_obstacle[add] != 0)
        ////    {
        ////        _obstacle[add] = _fieldData[add].GetObstacle();
        ////        Debug.Log(add + "表示" + _obstacle[add]);
        ////    }
        ////    _obstacleAdd[add] = _fieldData[add].GetObstacle();
        ////    // 今持っている数値が大きかったら代入するよ
        ////    if (_obstacle[add] < _obstacleAdd[add])
        ////    {
        ////        _obstacle[add] = _obstacleAdd[add];
        ////    }

        ////    if (_obstaclePrev[add] != _obstacle[add])
        ////    {
        ////        _total += (_obstacle[_leftPlayer] - _obstacle[_rightPlayer]);
        ////    }
        ////    _obstaclePrev[add] = _obstacle[add];
        ////}

        ////// 最大数は30にしたいので30より多い数を送らないようにする.
        //_total = MaxLimit(_total);
        if (_obstacleAdd[0] > _obstacleAdd[1])
        {
            if (_obstacleAddPrev[0] != _obstacleAdd[0])
            {
                _obstacleAdd[0] = _obstacleAdd[0] - _obstacleAdd[1];
                _obstacleAdd[1] = 0;
                Debug.Log("right");
            }
        }
        else if (_obstacleAdd[1] > _obstacleAdd[0])
        {
            if (_obstacleAddPrev[1] != _obstacleAdd[1])
            {
                Debug.Log(_obstacleAdd[1] + "a" + _obstacleAdd[0]);
                _obstacleAdd[1] = _obstacleAdd[1] - _obstacleAdd[0];
                _obstacleAdd[0] = 0;
                Debug.Log("left");
            }
        }
        else if (_obstacleAdd[0] == _obstacleAdd[1])
        {
            _obstacleAdd[0] = 0;
            _obstacleAdd[1] = 0;
            Debug.Log("こんにちわ");
        }
        for (int i = 0; i < _obstacleAdd.Length; i++)
        {
            _obstacleAddPrev[i] = _obstacleAdd[i];
        }

#if true
        //デバック用におじゃまの数表示
        ObsText();
#endif

        //if (_total > 0)
        //{
        //    if (!_fieldData[_leftPlayer].IsFieldUpdate() && _fieldData[_rightPlayer].IsInstallaion() && !_isSetFall[_rightPlayer])
        //    {
        //        if (!_fieldData[_rightPlayer].IsFieldUpdate())
        //        {
        //            _fieldData[_rightPlayer].SetObstacle(_total);
        //            _fieldData[_rightPlayer].GetInstallation();
        //            _isSetFall[_rightPlayer] = true;
        //            Debug.Log("阿弥陀");
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }
        //    else if (_fieldData[_rightPlayer].IsInstallaion())
        //    {
        //        _fieldData[_rightPlayer].GetInstallation();
        //        return;

        //    }
        //    else
        //    {
        //        return;
        //    }
        //}
        //else if (_total < 0)
        //{
        //    if (!_fieldData[_rightPlayer].IsFieldUpdate() && _fieldData[_leftPlayer].IsInstallaion() && !_isSetFall[_leftPlayer])
        //    {
        //        if (!_fieldData[_leftPlayer].IsFieldUpdate())
        //        {
        //            _fieldData[_leftPlayer].SetObstacle(_total * -1);
        //            _fieldData[_leftPlayer].GetInstallation();
        //            _isSetFall[_leftPlayer] = true;
        //            Debug.Log("家紋");
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }
        //    else if (_fieldData[_leftPlayer].IsInstallaion())
        //    {
        //        _fieldData[_leftPlayer].GetInstallation();
        //        return;

        //    }
        //    else
        //    {
        //        return;
        //    }
        //}

        //// 初期化
        //for (int i = 0; i < _fieldData.Length; i++)
        //{
        //    // とりあえず値を取得するよ.
        //    _obstacle[i] = 0;
        //    _obstacleAdd[i] = 0;
        //}
        //_total = _obstacleCount;
        //_obstacleCount = 0;
    }
    // 最大数の制限処理
    private int MaxLimit(int total)
    {
        // 最大数は30にしたいのでMAXより多い数を送らないようにする.
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
        for(int i = 0 ; i < _obstacleText.Length; i++)
        {
            _obstacleText[i].SetObstacleCount(_obstacleAdd[i]);
        }

        //if (_total < 0 || _obstacleCount < 0)
        //{
        //    _obstacleText[_leftPlayer].SetObstacleCount((_total + _obstacleCount) * -1);
        //}
        //else if (_total > 0 || _obstacleCount > 0)
        //{
        //    _obstacleText[_rightPlayer].SetObstacleCount(_total + _obstacleCount);
        //}
        //else
        //{
        //    _obstacleText[_leftPlayer].SetObstacleCount(0);
        //    _obstacleText[_rightPlayer].SetObstacleCount(0);
        //}
    }
}
