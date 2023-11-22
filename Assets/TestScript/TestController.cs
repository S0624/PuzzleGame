using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// コメントがないよ？
public enum Direction
{
    Down,
    Left,
    Up,
    Right,
    max,
}
public class TestController : MonoBehaviour
{
    // ボードの横の最大値(6 * 13).
    private const int _borad_Width = 6;
    // ボードの縦の最大値.
    private const int _borad_Height = 13;

    [SerializeField] private GameObject _prefabCube = default;

    private int[,] _board = new int[_borad_Height, _borad_Width];
    private GameObject[,] _Cube = new GameObject[_borad_Height, _borad_Width];

    private int[,] _eraseBoard = new int[_borad_Height, _borad_Width];
    private Vector2Int[] _cubeDirection = new Vector2Int[(int)Direction.max];
    private bool _isTestEraseFlag = false;

    // 点滅周期[s]
    //[SerializeField] private float _cycle = 1;

    private double _time;

    private int flashcount = 0;

    // 点滅中かどうか
    private bool _isFlashAnimation;

    float test = 0.1f;

    // スコアのカウント用
    private int _score = 10;
    private int _eraseCount = 0;

    // 連鎖カウント
    private int _chainCount = 0;
    // 設置したかのフラグ.
    private bool _isInstallaion = false;
    // HACK テスト用のフラグ(処理が終わったよ、のフラグ)
    private bool _isProcess = false;

    // 全消しフラグ
    private bool _isClearAll = false;
    // ボードの中を全消しする(クリアする).
    private void ClearAll()
    {
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                _board[y, x] = 0;

                if (_Cube[y, x] != null) Destroy(_Cube[y, x]);
                _Cube[y, x] = null;
            }
        }
    }

    // 初期化処理.
    public void Start()
    {
        // HACK テストテストテストテスト
        // タテヨコ方向のチェックをforで回せないか実装してみるためのテスト実装(四つ分配列でもつ)
        _cubeDirection[(int)Direction.Right] = new Vector2Int(1, 0);
        _cubeDirection[(int)Direction.Left] = new Vector2Int(-1, 0);
        _cubeDirection[(int)Direction.Up] = new Vector2Int(0, 1);
        _cubeDirection[(int)Direction.Down] = new Vector2Int(0, -1);
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
                IsSetCube(new Vector2Int(x, y), Random.Range(1, (int)ColorType.PuyoMax));
            }
        }
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
    public bool IsCanSetCube(Vector2Int pos)
    {
        if (!IsValidated(pos)) return false;
        //Debug.Log(pos);

        //Debug.Log(_board[pos.y, pos.x]);
        return 0 == _board[pos.y, pos.x];
    }
    // フィールド内にセットする
    public bool IsSetCube(Vector2Int pos, int val)
    {

        if (!IsCanSetCube(pos)) return false;

        // 色番号をセット.
        _board[pos.y, pos.x] = val;
        // もし中身が入っていたらエラー表記を出
        //Debug.Assert(_Cube[pos.y, pos.x] == null);
        //Debug.Log(pos);

        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0.0f);
        _Cube[pos.y, pos.x] = Instantiate(_prefabCube, world_position, Quaternion.identity, transform);
        _Cube[pos.y, pos.x].GetComponent<Test>().SetColorType((ColorType)val);

        // 設置したよ
        _isInstallaion = true;
        return true;
    }
    // Y軸(下方向)におけるかどうかのチェック
    public bool IsNextCubeY(Vector2Int pos)
    {
        // ゼロだったらおけるようにする
        if (pos.y == 0)
        {
            return true;
        }
        if (_Cube[pos.y - 1, pos.x] != null)
        {
            return true;
        }
        return false;
    }
    // X軸(左右方向)におけるかどうかのチェック
    public bool IsNextCubeX(Vector2Int pos, int add)
    {
        // 範囲外じゃないかどうか
        if (pos.x + add < 0 || pos.x + add > _borad_Width - 1)
        {
            return true;
        }
        if (_Cube[pos.y, pos.x + add] != null)
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
                if (_Cube[y, x] != null)
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
        ColorType cubeColor;
        int[,] tempBorad = new int[_borad_Height, _borad_Width];
        // 仮で保存する変数を初期化する
        EraseBoardClearAll();
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                ClearTempBorad(tempBorad);

                if (IsCubeFallDown(x, y)) return true;

                if (_Cube[y, x] == null)
                {
                    // 何も入っていないので処理をとばす
                    cubeColor = ColorType.None;
                    continue;
                }
                // カラーの番号を取得
                cubeColor = _Cube[y, x].GetComponent<Test>().GetColorType();
                // 消えるかどうかの判定
                IsRecursionCheckField(tempBorad, x, y, cubeColor);
                // 消せる個数をチェックする
                eraseCount = CountTempField(tempBorad);
                // 指定された数よりも消せる数が多かったら
                if (eraseCount >= 4)
                {
                    _isProcess = true;
                    TempEraseField(tempBorad);
                    //return true;
                }
            }
        }
        //if (_isInstallaion)
        //{
            FrashField(_eraseBoard);
        //}

        if (_isFlashAnimation)
        {
            return true;
        }
        _isProcess = false;
        return false;
    }
    //public void IsFieldUpdate()
    //{
    //    int eraseCount = 0;
    //    ColorType cubeColor;
    //    int[,] tempBorad = new int[_borad_Height, _borad_Width];
    //    //// 仮で保存する変数を初期化する
    //    //EraseBoardClearAll();
    //    for (int y = 0; y < _borad_Height; y++)
    //    {
    //        for (int x = 0; x < _borad_Width; x++)
    //        {
    //            ClearTempBorad(tempBorad);

    //            //if (IsCubeFallDown(x, y)) return true;

    //            if (_Cube[y, x] == null)
    //            {
    //                // 何も入っていないので処理をとばす
    //                cubeColor = ColorType.None;
    //                continue;
    //            }
    //            // カラーの番号を取得
    //            cubeColor = _Cube[y, x].GetComponent<Test>().GetColorType();
    //            // 消えるかどうかの判定
    //            IsRecursionCheckField(tempBorad, x, y, cubeColor);
    //            // 消せる個数をチェックする
    //            eraseCount = CountTempField(tempBorad);
    //            // 指定された数よりも消せる数が多かったら
    //            if (eraseCount >= 4)
    //            {
    //                TempEraseField(tempBorad);
    //                //return true;
    //            }
    //        }
    //    }

    //    FrashField(_eraseBoard);
    //    //if (_isFlashAnimation)
    //    //{
    //    //    return true;
    //    //}
    //}

    //public void GetInstallation(bool flag)
    //{
    //    _isInstallaion = flag;
    //}
    //public bool SetInstallation()
    //{
    //    Debug.Log("_isProcess" + _isProcess);
    //    return _isProcess;
    //}
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


        if(_Cube[y, x] == null) return false;
        //指定した位置に指定された色が置かれているかチェックをする
        if (_Cube[y, x].GetComponent<Test>().GetColorType() == colorType) return true;

        return false;
    }

    private bool IsRecursionCheckField(int[,] tempField, int x, int y, ColorType color)
    {
        //違う色なので終了
        if (!IsSameColor(color, x, y))
        {
            return false;
        }
        //同じ色の場合
        tempField[y,x] = 1;        //同じ色がつながっている

        for (int dir = 0; dir < (int)Direction.max; dir++)
        {
            int indexX = x + _cubeDirection[dir].x;
            int indexY = y + _cubeDirection[dir].y;

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
            }
        }
    }
    // HACK 光らせたいからそのテスト
    private void FrashField(int[,] tempField)
    {
        _isTestEraseFlag = true;

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

        // HACK テスト実装
        // 内部時刻を経過させる
        _time += test ;
        
        var alpha = _time;

        for (int x = 0; x < _borad_Width; x++)
        {
            for (int y = 0; y < _borad_Height; y++)
            {
                if (tempField[y, x] == 1)
                {
                    // 点滅？処理.
                    _isFlashAnimation = true;
                    _Cube[y, x].GetComponent<Test>().ChangeColor((float)alpha);
                    //if (_Cube[y, x] != null) Destroy(_Cube[y, x]);
                }
            }
        }
        if (alpha < 0 || alpha > 1)
        {
            test *= -1;
        }
        // HACK なんか気持ち悪い処理になってる なんか、なんかちがう
        if (alpha >= 1.0f)
        {
            flashcount++;
        }
        // 三回点滅したら.
        if (flashcount >= 3)
        {
            flashcount = 0;
            _isFlashAnimation = false;
            _time = 1.0f;
            // 連鎖カウント.
            //_chainCount++;
            EraseField(tempField);
        }
    }
    // キューブを消す処理
    private void EraseField(int[,] tempField)
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
                    if (_Cube[y, x] != null) Destroy(_Cube[y, x]);
                    _Cube[y, x] = null;
                    _board[y, x] = 0;
                    tempField[y, x] = 0;
                    fallDown++;
                    _eraseCount++;
                }
            }
            FallDownField(x, fallDown);
        }
        _isTestEraseFlag = false;
    }
    private void FallDownField(int x ,int falldown)
    {
        // 落す処理(配列の情報をずらす(現在のフィールドにあるキューブを落す))
        for (int y = 0; y < _borad_Height - falldown; y++)
        {
            if (_board[y, x] == 0)
            {
                _Cube[y, x] = _Cube[y + falldown, x];
                _Cube[y + falldown, x] = null;
                //Debug.Log(_board[y + drapFall, x]);
                _board[y, x] = _board[y + falldown, x];
                _board[y + falldown, x] = 0;
                //_Cube[y, x] = _Cube[0,0];
            }
        }
    }
    // キューブが落下中かどうか
    // HACK 落下中だと移動できなくしたいんだけどこまったことになってる
    private bool IsCubeFallDown(int x, int y)
    {
        if (_isTestEraseFlag)
        {
            // 数値が何も入っていない場合チェックしなくてよい
            //if (_Cube[y, x] == null)
            //{
            //    return false;
            //}
            if (_Cube[y, x] != null && _Cube[y, x].GetComponent<Test>().IsMoveCube())
            {
                //Debug.Log("とおったよHey");
                //_isTestEraseFlag = false;
                return true;
            }
        }
        //_isTestEraseFlag = false;
        return false;
    }

#if DEBUG
    public Vector2Int SteepDescent(Vector2Int pos, int dir)
    {
        Vector2Int result = new Vector2Int();
        // 下まで急降下させる
        for (int y = _borad_Height - 1; y >= 0; y--)
        {
            if (_Cube[y, pos.x] == null)
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
            if (_Cube[result.y - 2, result.x] == null)
            {
                result.y = result.y - 2;
            }
        }
        EraseScore();
        return result;
    }

#endif
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
            Debug.Log("posが0以下やで");
            return rotaPos;
        }
        if (_Cube[pos.y,pos.x] != null)
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
        //Debug.Log(_eraseCount * 10);
        return _score * _eraseCount;
    }
    public void SetScore()
    {
        _eraseCount = 0;
    }
    // ゲームオーバーフラグ
    public bool IsGameOver()
    {
        // HACK とりあえず雑にテストでゲームオーバー処理をしようとしてる
        // 2をマイナスしている理由は12のところがばってんで13は使用しないので12をみたいため
        if(_Cube[_borad_Height - 2,3] != null || _Cube[_borad_Height - 2, 4] != null)
        {
            //Debug.Log("Game Over");
            return true;
        }
        return false;

    }

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
