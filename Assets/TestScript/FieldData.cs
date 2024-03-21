using UnityEngine;


// フィールドの管理スクリプト
public class FieldData : MonoBehaviour
{
    // ボードの横の最大値(6 * 13).
    private const int _borad_Width = 6;
    // ボードの最大値から一つ引いたもの
    private const int _boradWidthMax = _borad_Width - 1;
    // ボードの縦の最大値.
    private const int _borad_Height = 13;
    private const int _boradHeightMax = _borad_Height - 1;

    [SerializeField] private GameObject _prefabSphere = default;
    [SerializeField] private GameObject _disturbanceSphere = default;
    [SerializeField] private GameObject _popEffect = default;
    [SerializeField] private GameObject _bubbleEffect = default;
    [SerializeField] private GameObject _starEffect = default;

    private int[,] _board = new int[_borad_Height, _borad_Width];
    private GameObject[,] _sphere = new GameObject[_borad_Height, _borad_Width];

    private int[,] _eraseBoard = new int[_borad_Height, _borad_Width];
    private Vector2Int[] _sphereDirection = new Vector2Int[(int)Direction.max];
    private bool _isEraseNowFlag = false;
    private bool _isEraseFlag = false;
    // 色をいじるために使用.
    private ParticleSystem _popParticle;
    private GameObject _eraseEffect;
    private GameObject _chainEffect;
    // カラーの取得.
    public ColorTable _colorTable;

    private Vector2 _erasePos;
    // その場所に置いたかどうかのフラグ
    private bool[] _isSet = new bool[_borad_Width];
    // 点滅周期[s]
    //[SerializeField] private float _cycle = 1;
    // カラーの取得
    private Color _sphereColor;
    private double _timeMax = 0.8f;
    private double _totaTime = 0.9f;
    // 点滅の回数
    private int _frashMax = 3;
    private int _flashcount = 0;

    // 点滅中かどうか
    private bool _isFlashAnimation;

    float _timeCount = 0.3f;

    // スコアのカウント用
    private int _score = 10;
    private int _eraseCount = 0;

    // 連鎖カウント
    private int _chainCount = 0;
    private int _prevChainCount = 0;
    private int _bonus = 0;
    // 設置が終わったかどうかのフラグ.
    private bool _isSetEnd = false;
    // 妨害用のスフィアのかず
    private int _obstacleNum;
    // 妨害用のスフィアの数.
    private int _obstacleCount = 0;

    // 設置したかのフラグ.
    private bool _isInstallaion = false;
    private bool _isSetSphere = false;
    // HACK テスト用のフラグ(処理が終わったよ、のフラグ)
    private bool _isProcess = false;

    // フィールドの処理のフラグ
    private bool _isField = false;
    // サウンドの取得
    private SoundManager _soundManager;
    // ボードの中を全消しする(クリアする).
    private void ClearAll()
    {
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                _board[y, x] = (int)FieldContentsData.None; ;

                if (_sphere[y, x] != null) Destroy(_sphere[y, x]);
                _sphere[y, x] = null;
            }
        }
    }

    // 初期化処理.
    public void Start()
    {
        // HACK テストテストテストテスト
        // タテヨコ方向のチェックをforで回せないか実装してみるためのテスト実装(四つ分配列でもつ)
        _sphereDirection[(int)Direction.Right] = new Vector2Int(1, 0);
        _sphereDirection[(int)Direction.Left] = new Vector2Int(-1, 0);
        _sphereDirection[(int)Direction.Up] = new Vector2Int(0, 1);
        _sphereDirection[(int)Direction.Down] = new Vector2Int(0, -1);
        ResetIsSet();
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    // フラグのリセット
    private void ResetIsSet()
    {
        for(int i = 0; i < _borad_Width; i++)
        {
            _isSet[i] = false;
        }
    }
    // 生成する.
    private void Generate()
    {
        // 生成する前に消す.
        ClearAll();

        // ランダムにフィールド上すべてに生成する
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                IsNormalSphere(new Vector2Int(x, y), Random.Range(1, (int)ColorType.PuyoMax));
            }
        }
    }
    // テスト用でおじゃまを降らせる処理.
    public void DisturbanceFall()
    {
        // 下から詰めるけどランダムに降らせたい.
        // そんでたぶんこのままだと大変なことになる気がする.

        if (_obstacleNum >= _borad_Width)
        {
            BlockObstruction(_obstacleNum / _borad_Width);
        }
        // ランダムにフィールド上すべてに生成する
        int remainder = _obstacleNum % _borad_Width;
        for (int i = 0; i < remainder; i++)
        {
            _obstacleNum--;
            int rand = IndexXCheck();
            int indexY = SearchDown(rand);
            IsDisturbanceSphere(new Vector2Int(rand, indexY));
        }
        // フラグの初期化
        ResetIsSet();
    }

    private int IndexXCheck()
    {
        int rand = Random.Range(0, _boradWidthMax);
        if(_isSet[rand])
        {
            IndexXCheck();
        }
        else
        {
            _isSet[rand] = true;
        }

        return rand;
    }
    // 6個塊で以上落すときの処理
    private void BlockObstruction(int block)
    {
        // ランダムにフィールド上に生成する
        for (int x = 0; x < _borad_Width; x++)
        {
            int indexY = SearchDown(x);
            for (int y = indexY; y < block + indexY; y++)
            {
                _obstacleNum--;
                IsDisturbanceSphere(new Vector2Int(x, y));
            }
        }
    }
    // 一番下から置ける場所を探す処理.
    private int SearchDown(int indexX)
    {
        for (int y = 0; y < _borad_Height; y++)
        {
            // スフィアがあったら次の場所を探してもらう.
            if (_board[y, indexX] != (int)FieldContentsData.None)
            {
                continue;
            }
            else
            {
                return y;
            }
        }
        return 0;
    }
    // 範囲外じゃないかの判定
    public bool IsValidated(Vector2Int pos)
    {
        if (0 <= pos.x && pos.x < _borad_Width
        && 0 <= pos.y && pos.y < _borad_Height)
        {
            // 判定外
            return true;
        }
        // 判定内
        return false;
    }
    // フィールド内におけるかどうかの判定
    public bool IsCanSetSphere(Vector2Int pos)
    {
        if (!IsValidated(pos)) return false;

        return 0 == _board[pos.y, pos.x];
    }
    // フィールド内にセットする
    public bool IsNormalSphere(Vector2Int pos, int val)
    {
        // 再設置を避けるための処理
        if (_isSetEnd) return false;
        // 範囲外ではないかのチェック
        if (!IsCanSetSphere(pos)) return false;
        // 色番号をセット.
        _board[pos.y, pos.x] = val;

        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0.0f);
        _sphere[pos.y, pos.x] = Instantiate(_prefabSphere, world_position, _prefabSphere.transform.rotation, transform);
        _sphere[pos.y, pos.x].GetComponent<SphereData>().SetColorType((ColorType)val);

        // 設置したよ
        _isInstallaion = true;
        _isSetSphere = true;

        return true;
    }
    /// <summary>
    /// おじゃまの設置
    /// </summary>
    /// <param name="pos"><位置/param>
    /// <returns>設置できるかどうか</returns>
    public bool IsDisturbanceSphere(Vector2Int pos)
    {
        int val = (int)ColorType.hindrance;
        if (!IsCanSetSphere(pos)) return false;

        _isInstallaion = true;
        // 色番号をセット.
        _board[pos.y, pos.x] = val;

        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0.0f);
        world_position.y += _borad_Height;
        _sphere[pos.y, pos.x] = Instantiate(_disturbanceSphere, world_position, _disturbanceSphere.transform.rotation, transform);
        _sphere[pos.y, pos.x].GetComponent<SphereData>().SetColorType((ColorType)val);

        // 設置したよ
        _isSetSphere = true;
        return true;
    }
    // Y軸(下方向)におけるかどうかのチェック
    public bool IsNextSphereY(Vector2Int pos, int add)
    {
        // ゼロだったらおけるようにする
        if (pos.y == 0)
        {
            return true;
        }
        // 範囲外じゃないかチェックする.
        if(pos.y < 0 || pos.y > _borad_Height)
        {
            return false;
        }
        if (_sphere[pos.y - 1, pos.x + add] != null)
        {
            return true;
        }
        return false;
    }
    // X軸(左右方向)におけるかどうかのチェック
    public bool IsNextSphereX(Vector2Int pos, int add)
    {
        //Debug.Log("X" + (pos.x + add) + "Y :" + pos.y);
        // 範囲外じゃないかどうか
        if (pos.x + add < 0 || pos.x + add > _boradWidthMax)
        {
            //Debug.Log("範囲外やで");
            return true;
        }
        // 範囲外じゃないかチェックする.
        if (pos.x < 0 || pos.x > _borad_Width)
        {
            Debug.Log("範囲外");
            return false;
        }
        if (_sphere[pos.y, pos.x + add] != null)
        {
            //Debug.Log("??");
            return true;
        }
        return false;
    }

    // 消す場所を保存する変数を初期化する.
    private void EraseBoardClearAll()
    {
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                _eraseBoard[y, x] = (int)FieldContentsData.None;
            }
        }

    }
    // 全消ししたかどうかを見る(テスト)
    public bool FieldAllClear()
    {
        // 中身が入っているかのフラグ
        bool isEmpty = true;
        isEmpty = _isInstallaion;
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                // 中身があることになる
                if (_sphere[y, x] != null)
                {
                    isEmpty = false;
                    break;
                }
            }
            // 中身が入っていたらfor文を抜ける.
            if (!isEmpty)
            {
                break;
            }
        }
        return isEmpty;
    }


    public bool IsCheckField()
    {
        int eraseCount = 0;
        ColorType sphereColor;
        int[,] tempBorad = new int[_borad_Height, _borad_Width];
        bool isFrash = false;
        // 仮で保存する変数を初期化する
        EraseBoardClearAll();
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                ClearTempBorad(tempBorad);

                if (IsSphereFallDown(x, y))
                {
                    return true;
                }
                if (_sphere[y, x] == null)
                {
                    // 何も入っていないので処理をとばす
                    sphereColor = ColorType.None;
                    continue;
                }
                // カラーの番号を取得
                sphereColor = _sphere[y, x].GetComponent<SphereData>().GetColorType();
                // 消えるかどうかの判定
                IsRecursionCheckField(tempBorad, x, y, sphereColor);
                // 消せる個数をチェックする
                eraseCount = CountTempField(tempBorad);
                // 指定された数よりも消せる数が多かったら
                if (eraseCount >= 4)
                {
                    // 点滅？処理.
                    isFrash = true;
                    TempEraseField(tempBorad);
                }
            }
        }

        FrashField(_eraseBoard, isFrash);

        if (_isEraseNowFlag)
        {
            return true;
        }
        return false;
    }
    private void Update()
    {
        
    }
    public void FieldUpdate()
    {
        _isField = IsCheckField();
        if (!_isField)
        {
            _chainCount = 0;
            _bonus = 0;
        }
        if (!_isField)
        {
            // 連鎖が終わっている.
            if (_prevChainCount > 0 && _prevChainCount > _chainCount)
            {
                //Debug.Log("れんさしてない");
                _isSetEnd = true;
            }
            // 設置のみを行った.
            else if (_isSetSphere)
            {
                _isSetEnd = true;
            }
        }
        else
        {
            // 連鎖中.
            _isSetEnd = false;
            //Debug.Log("れんさしてる");
        }
        // 初期化する.
        _isSetSphere = false;
    }
    // 邪魔落下中は動きを止めたい
    public bool MoveObstacleSphere()
    {
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                if (_board[y, x] == (int)ColorType.hindrance && MoveCheck(x,y))
                {
                    // デバック文表示してるよ
                    //Debug.Log("落下途中やで");
                    return true;
                }
            }
        }
        return false;
    }
    private bool MoveCheck(int x,int y)
    {
        if (_sphere[y, x] != null)
        {
            bool move = _sphere[y, x].GetComponent<SphereData>().IsMoveSphere();
            return move;
        }
        return false;
    }
    public bool IsFieldUpdate()
    {
        return _isField;
    }
    public bool IsInstallaion()
    {
        //Debug.Log(_isInstallaion);
        return _isInstallaion;
    }
    public void GetInstallation()
    {
        _isInstallaion = false;
    }
    public bool SetInstallation()
    {
        return _isProcess;
    }
    private void ClearTempBorad(int[,] tempField)
    {
        for (int x = 0; x < _borad_Width; x++)
        {
            for (int y = 0; y < _borad_Height; y++)
            {
                tempField[y,x] = (int)FieldContentsData.None;
            }
        }
    }
    // チェックしようとしているところにあるキューブが同じ色かどうか
    private bool IsSameColor(ColorType colorType, int x, int y)
    {
        if (x < 0) return false;
        if (x > _boradWidthMax) return false;
        if (y < 0) return false;
        if (y > _boradHeightMax) return false;


        if(_sphere[y, x] == null) return false;
        //指定した位置に指定された色が置かれているかチェックをする
        if (_sphere[y, x].GetComponent<SphereData>().GetColorType() == colorType) return true;

        return false;
    }

    private bool IsRecursionCheckField(int[,] tempField, int x, int y, ColorType color)
    {
        //違う色なので終了
        if (!IsSameColor(color, x, y))
        {
            return false;
        }
        // おじゃまだった場合2をいれる.
        if (color == ColorType.hindrance)
        {
            tempField[y, x] = (int)FieldContentsData.Obstacle;
        }
        else
        {
            //同じ色の場合
            tempField[y, x] = (int)FieldContentsData.Octopus;        //同じ色がつながっている
        }
        for (int dir = 0; dir < (int)Direction.max; dir++)
        {
            int indexX = x + _sphereDirection[dir].x;
            int indexY = y + _sphereDirection[dir].y;

            //範囲外はチェックしない
            if (indexX < 0) continue;
            if (indexX > _boradWidthMax) continue;
            if (indexY < 0) continue;
            if (indexY > _boradHeightMax) continue;

            if (tempField[indexY,indexX] == 0)//すでにつながっている判定されている部分はチェックしない
            {
                // 位置をずらしてチェックする
                IsRecursionCheckField(tempField,indexX, indexY,color);
            }
        }
        return false;
    }
    // 仮で変数に保存した消せる場所をカウントする
    private int CountTempField(int[,] tempField)
    {
        int count = 0;
        for (int x = 0; x < _borad_Width; x++)
        {
            for (int y = 0; y < _borad_Height; y++)
            {
                if (tempField[y,x] == (int)FieldContentsData.Octopus)
                {
                    count++;
                }
            }
        }
        return count;
    }
    // 仮で変数に保存した消せる場所をカウントする
    private void TempEraseField(int[,] tempField)
    {
        for (int x = 0; x < _borad_Width; x++)
        {
            for (int y = 0; y < _borad_Height; y++)
            {
                if (tempField[y, x] == (int)FieldContentsData.Octopus)
                {
                    _eraseBoard[y, x] = (int)FieldContentsData.Octopus;
                }
                // おじゃまのデータを入れる.
                else if (_board[y, x] == (int)ColorType.hindrance)
                {
                    _eraseBoard[y, x] = (int)FieldContentsData.Obstacle;
                }
            }
        }
    }
    // HACK 光らせたいからそのテスト
    private void FrashField(int[,] tempField,bool frash)
    {
        _isFlashAnimation = frash;
        // HACK テスト実装
        // 内部時刻を経過させる
        if (_isFlashAnimation)
        {
            _totaTime += _timeCount;
        }

        var alpha = _totaTime;

        for (int x = 0; x < _borad_Width; x++)
        {
            for (int y = 0; y < _borad_Height; y++)
            {
                if (tempField[y, x] == (int)FieldContentsData.Octopus)
                {
                    _isEraseNowFlag = true;
                     //点滅？処理.
                    _sphere[y, x].GetComponent<SphereData>().ChangeColor((float)alpha);
                }
            }
        }
        if (alpha < 0 || alpha > 1)
        {
            _timeCount *= -1;
        }
        // HACK なんか気持ち悪い処理になってる なんか、なんかちがう
        if (alpha >= 1.0f)
        {
            _flashcount++;
        }
        // 三回点滅したら.
        if (_flashcount >= _frashMax && _isFlashAnimation)
        {
            _isEraseFlag = true;
            _flashcount = 0;
            _isFlashAnimation = false;
            _totaTime = _timeMax;
            _timeCount *= -1;
        }
        if (_isEraseFlag)
        {
            EraseSphere(tempField);
            _isEraseFlag = false;
        }
        else if (!_isFlashAnimation)
        {
            _isEraseNowFlag = false;
            //_chainCount = 0;
        }

    }
    // スフィアを消す処理
    private void EraseSphere(int[,] tempField)
    {
        int fallDown = 0;
        for (int x = 0; x < _borad_Width; x++)
        {
            //fallDown = 0;
            for (int y = 0; y < _borad_Height; y++)
            {
                if (tempField[y, x] == (int)FieldContentsData.Octopus)
                {
                    _erasePos = new Vector2(x, y);
                    _soundManager.SEPlay(SoundSEData.EraseSE);
                    // こわす(消す)処理.
                    if (_sphere[y, x] != null)
                    {
                        // 消したときの演出.
                        EraseEffect(x, y);
                        // 消去処理
                        SphereDestory(x, y);
                        tempField[y, x] = (int)FieldContentsData.None;
                    }
                    EraseDisturbance(x, y, tempField);
                    _eraseCount++;
                }
            }
            FallDownField(x, fallDown);
            fallDown++;
        }
        EraseChainEffect();
        _chainCount++;
    }
    /// <summary>
    /// スフィア消去処理
    /// </summary>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    private void SphereDestory(int x,int y)
    {
        Destroy(_sphere[y, x]);
        _sphere[y, x] = null;
        _board[y, x] = (int)FieldContentsData.None;
    }
    // 消すときのエフェクト表示.
    private void EraseEffect(int posX,int posY)
    {
        Vector3 pos = transform.position + new Vector3(posX, posY, 0.0f);
        _sphereColor = _colorTable.GetColor(_board[posY, posX]);
        _eraseEffect = Instantiate(_popEffect, pos, Quaternion.identity, transform);
        // 泡のエフェクト
        Instantiate(_bubbleEffect, pos, Quaternion.identity, transform);

        ParticleSystem.MainModule effect = _eraseEffect.GetComponent<ParticleSystem>().main;
        effect.startColor = _sphereColor;
    }
    private void EraseChainEffect()
    {
        if (!_chainEffect)
        {
            _chainEffect = Instantiate(_starEffect);
            ParticleSystem.MainModule effect = _chainEffect.GetComponent<ParticleSystem>().main;
            effect.startColor = _sphereColor;
            effect.maxParticles = _chainCount + 1;
            //_chainEffect = Instantiate(_starEffect);
        }
    }
    // おじゃまスフィアの消去処理.
    private void EraseDisturbance(int x, int y, int[,] tempField)
    {
        // てすと実装
        Vector2Int[] sphereDirection = new Vector2Int[(int)Direction.max];
        sphereDirection[(int)Direction.Down] = new Vector2Int(0, -1);
        sphereDirection[(int)Direction.Left] = new Vector2Int(-1, 0);
        sphereDirection[(int)Direction.Up] = new Vector2Int(0, 1);
        sphereDirection[(int)Direction.Right] = new Vector2Int(1, 0);
        // きえるぷよのタテヨコ方向におじゃまがあったら消す処理.
        for (int i = 0; i < sphereDirection.Length; i++)
        {
            int dirX = x + sphereDirection[i].x;
            int dirY = y + sphereDirection[i].y;
            // 範囲外じゃないか
            if (dirX >= 0 && dirX < _borad_Width && dirY >= 0 && dirY < _borad_Height)
            {
                if (tempField[dirY, dirX] == (int)FieldContentsData.Obstacle)
                {
                    // 消去処理
                    SphereDestory(dirX, dirY);
                    //tempField[dirY, dirX] = (int)FieldContentsData.None;
                }
            }
        }
    }
    // 消えた時に落とす処理.
    private void FallDownField(int x ,int falldown = 0)
    {
        // 落す処理(配列の情報をずらす(現在のフィールドにあるキューブを落す))
        int hight = 0;
        for (int y = 0; y < _boradHeightMax; y++)
        {
            if (_board[y, x] == (int)FieldContentsData.None)
            {
                _sphere[y, x] = _sphere[y + 1, x];
                _sphere[y + 1, x] = null;
                _board[y, x] = _board[y + 1, x];
                _board[y + 1, x] = (int)FieldContentsData.None;
            }
            if (_board[y, x] != (int)FieldContentsData.None)
            {
                hight = y + 1;
                //hight = y;
            }
        }
        if(FallCheck(x , hight))
        {
            FallDownField(x, falldown);
        }
    }
    // すべて落したかの確認.
    private bool FallCheck(int x,int indexY)
    {
        //var max = 0;
        //for (int y = 0; y < _boradHeightMax; y++)
        //{
        //    if (_board[y, x] == (int)FieldContentsData.None)
        //    {
        //        max = y;
        //    }
        //    else
        //    {
        //        //if (max != 0) return true;
        //        if (max != 0)
        //        {
        //            Debug.Log("Y" + y + "X" + x + "borad" + _board[y,x]);
        //            return true;
        //        }
        //    }
        //}
        ////Debug.Log(max);
        ////if(max != indexY)
        ////{
        ////    Debug.Log("あああ");
        ////    return true;
        ////}
        //return false;
        for (int y = 0; y < indexY; y++)
        {
            if (_board[y, x] == (int)FieldContentsData.None)
            {
                return true;
            }
        }
        return false;
    }
    // キューブが落下中かどうか
    // HACK 落下中だと移動できなくしたいんだけどこまったことになってる
    private bool IsSphereFallDown(int x, int y)
    {
        if (_isEraseNowFlag)
        {
            if (_sphere[y, x] != null && _sphere[y, x].GetComponent<SphereData>().IsMoveSphere())
            {
                return true;
            }
        }
        return false;
    }

    //#if DEBUG
    // クイック処理
    public Vector2Int SteepDescent(Vector2Int pos, int dir = 0)
    {
        Vector2Int result = pos;
        // 下まで急降下させる
        for (int y = _boradHeightMax; y >= 0; y--)
        {
            if (_sphere[y, pos.x] == null)
            {
                result.x = pos.x;
                result.y = y;
            }
            else
            {
                break;
            }
        }

        if (pos.y > 0)
        {
            result.y = result.y + pos.y;
            return result;
        }
        // HACK とんでもないゴリ押しの処理
        // 上向いてる時だけ無理やり処理を強制してる
        var posY = result.y - 2;
        if (dir == (int)Direction.Up)
        {
            if (posY < 0)
            {
                Debug.Log(posY);
                //result.y = _boradHeightMax;
                result.y = 0;
                posY = _boradHeightMax;
                //return result;
            }
            if (_sphere[posY, result.x] == null)
            {
                result.y = posY;
            }
        }
        EraseScore();
        return result;
    }

//#endif
    // うごかすキューブをフィールドの座標に変換する
    public Vector3 fieldPos(Vector2Int pos)
    {
        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0);
        return world_position;
    }
    /// <summary>
    /// 回転可能かどうかの処理
    /// </summary>
    /// <param name="pos">現在の位置</param>
    /// <param name="direction">方向</param>
    /// <returns></returns>
    public bool IsSphereRota(Vector2Int pos, int direction)
    {
        // 方向をキャスト
        var dir = (Direction)direction;
        // 現在地から左右のポジションの取得
        var leftpos = pos.x - 1;
        var rightpos = pos.x + 1;
        // 指定範囲外に出たら回せないようにする
        if (pos.y > _boradHeightMax - 1)
        {
            Debug.Log("ちょちょちょ");
            return false;
        }
        // 上下の時のみ処理を行う
        if (Direction.Down == dir || Direction.Up == dir)
        {
            if (pos.x == 0 && _sphere[pos.y, rightpos] != null)
            {
                return false;
            }
            if (pos.x == _boradWidthMax && _sphere[pos.y, leftpos] != null)
            {
                return false;
            }
            if (pos.x < 0 || pos.x >= _boradWidthMax) return true;
            if (_sphere[pos.y, rightpos] != null && _sphere[pos.y, leftpos])
            {
                return false;
            }
        }
        
        return true;

    }
    // 範囲外に行かないように調整
    // HACK ここで回したときに壁じゃなくスフィアに当たった場合どうするか
    //public int MoveRotaCheck(Vector2Int pos, Vector2Int direction)
    public Vector2Int MoveRotaCheck(Vector2Int pos, Vector2Int direction)
    {
        Vector2Int rotaPos = new Vector2Int();
        if(pos.x < 0)
        {
            rotaPos = new Vector2Int(1,0);
            return rotaPos;
        }
        if (pos.x >= _borad_Width)
        {
            rotaPos = new Vector2Int(-1,0);
            return rotaPos;
        }
        if (pos.y < 0)
        {
            rotaPos = new Vector2Int(0, 1);
            return rotaPos;
        }
        if (pos.y > _boradHeightMax) return rotaPos;
        if (_sphere[pos.y,pos.x] != null)
        {
            rotaPos = -direction;
            return rotaPos;
        }
        // なにもなければ0でいいわよ
        return rotaPos;
    }
    // 消した分のスコアを計算する
    public int EraseScore()
    {
        // HACK きれいにする
        ScoreCalculation();
        return (_score * _eraseCount) * (_bonus);
    }
    // スコアの計算を行っている.
    private void ScoreCalculation()
    {
        // HACK きれいにする
        // 連鎖数が1の時はボーナスなし
        // 2以上は倍数でふやし、8連鎖以上だと+32で固定する
        if (_chainCount == 1)
        {
            _bonus = 4;
        }
        // 前のフレームより連鎖数が増えていたら処理をする.
        if (_chainCount > _prevChainCount && _chainCount > 1 && _chainCount < 8)
        {
            _bonus *= 2;
        }
        if (_chainCount >= 8 && _chainCount > _prevChainCount)
        {
            _bonus += 32;
        }
        // お邪魔計算.
        _obstacleCount = (int)((_score * _eraseCount) * (_bonus) * 0.0125f);
        
        //Debug.Log("連鎖数:" + _chainCount + "ボーナス:" + _bonus);
        //Debug.Log(_obstacleCount);
        //Debug.Log(_bonus);
        _prevChainCount = _chainCount;
    }
    public void SetObstacle(int num)
    {
        _obstacleNum= num;
    }
    public int GetTotalObstacle()
    {
        return _obstacleNum;
    }
    public void SetScore()
    {
        _eraseCount = 0;
    }
    // テスト用の連鎖数を返す関数
    public int SetChain()
    {
        return _chainCount;
    }
    public bool IsSetSphere()
    {
        return _isSetEnd;
    }
    public void IsSetReset()
    {
        _isSetEnd = false;
    }
    // おじゃまの数の取得用.
    public int GetObstacle()
    {
        return _obstacleCount;
    }
    // ゲームオーバーフラグ
    public bool IsGameOver()
    {
        // HACK とりあえず雑にテストでゲームオーバー処理をしようとしてる
        // 2をマイナスしている理由は12のところがばってんで13は使用しないので12をみたいため
        if(_sphere[_boradHeightMax - 1, 2] != null || _sphere[_boradHeightMax - 1, 3] != null)
        {
            return true;
        }
        return false;
    }
    public Vector2 ErasePos()
    {
        Vector2 pos = _erasePos;
        pos.x = RangeCheck((int)_erasePos.x,_borad_Width);
        pos.y = RangeCheck((int)_erasePos.y,_borad_Height);

        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0.0f);
        return world_position;
    }
    public int RangeCheck(int pos,int max)
    {
        if(pos < 2)
        {
            pos = 2;
        }
        else if(pos > max - 2)
        {
            pos = max - 2;
        }
        return pos;
    }
}
