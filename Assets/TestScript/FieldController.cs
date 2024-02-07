using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

// コメントがないよ？

public class FieldController : MonoBehaviour
{
    // ボードの横の最大値(6 * 13).
    private const int _borad_Width = 6;
    // ボードの縦の最大値.
    private const int _borad_Height = 13;

    [SerializeField] private GameObject _prefabSphere = default;
    [SerializeField] private GameObject _disturbanceSphere = default;

    private int[,] _board = new int[_borad_Height, _borad_Width];
    private GameObject[,] _sphere = new GameObject[_borad_Height, _borad_Width];

    private int[,] _eraseBoard = new int[_borad_Height, _borad_Width];
    private Vector2Int[] _sphereDirection = new Vector2Int[(int)Direction.max];
    private bool _isEraseNowFlag = false;
    private bool _isEraseFlag = false;

    // 点滅周期[s]
    //[SerializeField] private float _cycle = 1;

    private double _totaTime = 0.9f;

    private int flashcount = 0;

    // 点滅中かどうか
    private bool _isFlashAnimation;

    float _timeCount = 0.1f;

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
    private int  _obstacleNum;
    // 妨害用のスフィアの数.
    private int _obstacleCount = 0;

    // 設置したかのフラグ.
    private bool _isInstallaion = false;
    private bool _isSetSphere = false;
    // HACK テスト用のフラグ(処理が終わったよ、のフラグ)
    private bool _isProcess = false;

    // フィールドの処理のフラグ
    private bool _isField = false;

    // ボードの中を全消しする(クリアする).
    private void ClearAll()
    {
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                _board[y, x] = 0;

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
        //_obstacleNum = 1;
        //if (!_isInstallaion) return;

        if (_obstacleNum >= _borad_Width)
        {
            BlockObstruction(_obstacleNum / _borad_Width);
        }
        // ランダムにフィールド上すべてに生成する
        int remainder = _obstacleNum % _borad_Width;
        for(int i = 0; i < remainder; i++)
        {
            _obstacleNum--;
            int rand = Random.Range(0, _borad_Width);
            int indexY = SearchDown(rand);
            IsDisturbanceSphere(new Vector2Int(rand, indexY));
        }
    }
    // 6個塊で以上落すときの処理
    private void BlockObstruction(int block)
    {
        // ランダムにフィールド上すべてに生成する
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
            // 下におじゃまがあったら次の場所を探してもらう.
            if (_board[y, indexX] != 0)
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
        if (!IsCanSetSphere(pos)) return false;

        // 色番号をセット.
        _board[pos.y, pos.x] = val;
        // もし中身が入っていたらエラー表記を出

        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0.0f);
        _sphere[pos.y, pos.x] = Instantiate(_prefabSphere, world_position, Quaternion.identity, transform);
        _sphere[pos.y, pos.x].GetComponent<Test>().SetColorType((ColorType)val);

        // 設置したよ
        _isInstallaion = true;
        _isSetSphere = true;
        return true;
    }
    public bool IsDisturbanceSphere(Vector2Int pos)
    {
        int val = (int)ColorType.hindrance;
        if (!IsCanSetSphere(pos)) return false;

        // 色番号をセット.
        _board[pos.y, pos.x] = val;
        // もし中身が入っていたらエラー表記を出

        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0.0f);
        _sphere[pos.y, pos.x] = Instantiate(_disturbanceSphere, world_position, Quaternion.identity, transform);
        _sphere[pos.y, pos.x].GetComponent<Test>().SetColorType((ColorType)val);

        // 設置したよ
        _isInstallaion = true;
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
        if (_sphere[pos.y - 1, pos.x + add] != null)
        {
            return true;
        }
        return false;
    }
    // X軸(左右方向)におけるかどうかのチェック
    public bool IsNextSphereX(Vector2Int pos, int add)
    {
        // 範囲外じゃないかどうか
        if (pos.x + add < 0 || pos.x + add > _borad_Width - 1)
        {
            return true;
        }
        if (_sphere[pos.y, pos.x + add] != null)
        {
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
                _eraseBoard[y, x] = 0;
            }
        }

    }
    // 全消ししたかどうかを見る(テスト)
    public bool FieldAllClear()
    {
        // 中身が入っているかのフラグ
        bool isEmpty  = true;
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
            if(!isEmpty)
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
                sphereColor = _sphere[y, x].GetComponent<Test>().GetColorType();
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
                //Debug.Log("!せっちのみ");
                _isSetEnd = true;
            }
        }
        else
        {
            // 連鎖中.
            _isSetEnd = false;
            //Debug.Log("れんさしてる");
        }
        //Debug.Log("_isField:" + _isField);
        //Debug.Log("_isSetSphere:" + _isSetSphere);
        //Debug.Log("_isChainEnd:" + _isSetEnd);
        _isSetSphere = false;
        //_isInstallaion = false;

        //_isSetEnd = false;
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
    public void GetInstallation(bool flag)
    {
        _isInstallaion = flag;
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
                tempField[y,x] = 0;
            }
        }
    }
    // チェックしようとしているところにあるキューブが同じ色かどうか
    private bool IsSameColor(ColorType colorType, int x, int y)
    {
        if (x < 0) return false;
        if (x > _borad_Width - 1) return false;
        if (y < 0) return false;
        if (y > _borad_Height - 1) return false;


        if(_sphere[y, x] == null) return false;
        //指定した位置に指定された色が置かれているかチェックをする
        if (_sphere[y, x].GetComponent<Test>().GetColorType() == colorType) return true;

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
            tempField[y, x] = 2;
        }
        else
        {
            //同じ色の場合
            tempField[y, x] = 1;        //同じ色がつながっている
        }
        for (int dir = 0; dir < (int)Direction.max; dir++)
        {
            int indexX = x + _sphereDirection[dir].x;
            int indexY = y + _sphereDirection[dir].y;

            //範囲外はチェックしない
            if (indexX < 0) continue;
            if (indexX > _borad_Width - 1) continue;
            if (indexY < 0) continue;
            if (indexY > _borad_Height - 1) continue;

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
                if (tempField[y,x] == 1)
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
                if (tempField[y, x] == 1)
                {
                    _eraseBoard[y, x] = 1;
                }
                // おじゃまのデータを入れる.
                else if (_board[y, x] == (int)ColorType.hindrance)
                {
                    _eraseBoard[y, x] = 2;
                }
            }
        }
    }
    // HACK 光らせたいからそのテスト
    private void FrashField(int[,] tempField,bool frash)
    {

        ////var alpha = 0.0f;
        //_isFlashAnimation = true;
        //float blinkSpeed = 5.0f; // 点滅の速さ
        //// 内部時刻を経過させる
        ////_time += test ;
        ////_time ++;
        ////_time += 0.1f;
        ////_time += Time.deltaTime;
        //// 周期cycleで繰り返す波のアルファ値計算
        //var alpha = 0.5f + 0.5f * Mathf.Sin(blinkSpeed * (float)_time);
        ////var alpha = _time;
        //Debug.Log(_time + "alphaの値" + alpha);
        //for (int x = 0; x < _borad_Width; x++)
        //{
        //    for (int y = 0; y < _borad_Height; y++)
        //    {
        //        if (tempField[y, x] == 1)
        //        {
        //            _time += Time.deltaTime;
        //            // 点滅？処理.
        //            _Cube[y, x].GetComponent<Test>().ChangeColor((float)alpha);
        //            //if (_Cube[y, x] != null) Destroy(_Cube[y, x]);
        //        }
        //    }
        //}
        ////if(alpha < 0 || alpha > 1)
        ////{
        ////    test *= -1;
        ////}
        //// HACK なんか気持ち悪い処理になってる なんか、なんかちがう
        //if (alpha >= 0.99f)
        ////if (alpha >= 1.0f)
        //{
        //    flashcount++;
        //}
        //// 三回点滅したら.
        //if (flashcount >= 3)
        //{
        //    flashcount = 0;
        //    _isFlashAnimation = false;
        //    //_time = 0.0f;
        //    alpha = 1.0f;
        //    EraseField(tempField);
        //}

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
                if (tempField[y, x] == 1)
                {
                    _isEraseNowFlag = true;
                     //点滅？処理.
                    _sphere[y, x].GetComponent<Test>().ChangeColor((float)alpha);
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
            flashcount++;
        }
        // 三回点滅したら.
        if (flashcount >= 3 && _isFlashAnimation)
        {
            _isEraseFlag = true;
            flashcount = 0;
            _isFlashAnimation = false;
            _totaTime = 0.8f;
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
    // キューブを消す処理
    private void EraseSphere(int[,] tempField)
    {
        int fallDown = 0;
        for (int x = 0; x < _borad_Width; x++)
        {
            fallDown = 0;
            for (int y = 0; y < _borad_Height; y++)
            {
                if (tempField[y, x] == 1)
                {
                    // こわす(消す)処理.
                    if (_sphere[y, x] != null) Destroy(_sphere[y, x]);
                    _sphere[y, x] = null;
                    _board[y, x] = 0;
                    tempField[y, x] = 0;
                    fallDown++;
                    _eraseCount++;
                    EraseDisturbance(x, y, tempField);
                }
            }
            FallDownField(x, fallDown);
        }
        _chainCount++;
    }
    private void EraseDisturbance(int x, int y, int[,] tempField)
    {
        // てすと実装
        Vector2Int[] _sphereDirection = new Vector2Int[4];
        _sphereDirection[(int)Direction.Down] = new Vector2Int(0, -1);
        _sphereDirection[(int)Direction.Left] = new Vector2Int(-1, 0);
        _sphereDirection[(int)Direction.Up] = new Vector2Int(0, 1);
        _sphereDirection[(int)Direction.Right] = new Vector2Int(1, 0);
        // きえるぷよのタテヨコ方向におじゃまがあったら消す処理.
        for (int i = 0; i < _sphereDirection.Length; i++)
        {
            int dirX = x + _sphereDirection[i].x;
            int dirY = y + _sphereDirection[i].y;
            if (dirX >= 0 && dirX < _borad_Width && dirY >= 0 && dirY < _borad_Height)
            {
                if (tempField[dirY, dirX] == 2)
                {
                    Destroy(_sphere[dirY, dirX]);
                    _sphere[dirY, dirX] = null;
                    _board[dirY, dirX] = 0;
                }
            }
        }
    }
    // 消えた時に落とす処理.
    private void FallDownField(int x ,int falldown)
    {
        // 落す処理(配列の情報をずらす(現在のフィールドにあるキューブを落す))
        int hight = 0;
        for (int y = 0; y < _borad_Height - 1; y++)
        {
            if (_board[y, x] == 0)
            {
                _sphere[y, x] = _sphere[y + 1, x];
                _sphere[y + 1, x] = null;
                _board[y, x] = _board[y + 1, x];
                _board[y + 1, x] = 0;
            }
        }
        for (int y = 0; y < _borad_Height - 1; y++)
        {
            if (_board[y, x] != 0)
            {
                    hight = y;
            }
        }
        if(FallCheck(x , hight))
        {
            FallDownField(x, 0);
        }
    }
    // すべて落したかの確認.
    private bool FallCheck(int x,int indexY)
    {
        for (int y = 0; y < indexY; y++)
        {
            if (_board[y, x] == 0)
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
            if (_sphere[y, x] != null && _sphere[y, x].GetComponent<Test>().IsMoveSphere())
            {
                //Debug.Log("とおったよHey");
                //_isTestEraseFlag = false;
                return true;
            }
        }
        return false;
    }

//#if DEBUG
    // クイック処理
    public Vector2Int SteepDescent(Vector2Int pos, int dir)
    {
        Vector2Int result = new Vector2Int();
        // 下まで急降下させる
        for (int y = _borad_Height - 1; y >= 0; y--)
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

        // HACK とんでもないゴリ押しの処理
        // 上向いてる時だけ無理やり処理を強制してる
        if (pos.y > 0)
        {
            result.y = result.y + pos.y;
            return result;
        }
        if(dir == (int)Direction.Up)
        {
            if (_sphere[result.y - 2, result.x] == null)
            {
                result.y = result.y - 2;
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

    // 範囲外に行かないように調整
    // HACK ここで回したときに壁じゃなくキューブに当たった場合どうするか
    //public int MoveRotaCheck(Vector2Int pos, Vector2Int direction)
    public Vector2Int MoveRotaCheck(Vector2Int pos, Vector2Int direction)
    {
        //Debug.Log(EraseScore());
        Vector2Int rotaPos = new Vector2Int();
        //rotaPos = direction * -1;
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
            //Debug.Log("posが0以下やで");
            return rotaPos;
        }
        if (_sphere[pos.y,pos.x] != null)
        {
            rotaPos = -direction;
            return rotaPos;
        }
        //if (_Cube[pos.y, pos.x + 1] != null)
        //{
        //    //Debug.Log("こんにちわ、0より小さいわよ");
        //    return -1;
        //}
        //if (_Cube[pos.y, pos.x - 1] != null)
        //{
        //    //Debug.Log("こんにちわ、最大値より大きいわよ");
        //    return 1;
        //}

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
        _obstacleCount = (_score * _eraseCount) * (_bonus) / 70;
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
    public bool IsChain()
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
        if(_sphere[_borad_Height - 2,3] != null || _sphere[_borad_Height - 2, 4] != null)
        {
            //Debug.Log("Game Over");
            return true;
        }
        return false;
    }
}
