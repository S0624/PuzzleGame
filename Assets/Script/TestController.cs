using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// コメントがないよ？
public class TestController : MonoBehaviour
{
    // このクラスでしか使わない想定なのでプライベートにしている
    private enum Direction
    {
        Right,
        Left,
        Up,
        Down,
        max,
    }

    // ボードの横の最大値(6 * 15).
    private const int _borad_Width = 6;
    // ボードの縦の最大値.
    private const int _borad_Height = 15;

    [SerializeField] private GameObject _prefabCube = default;

    private int[,] _board = new int[_borad_Height, _borad_Width];
    private GameObject[,] _Cube = new GameObject[_borad_Height, _borad_Width];

    private int[,] _tempBoard = new int[_borad_Height, _borad_Width];
    private Vector2Int[] _cubeDirection = new Vector2Int[(int)Direction.max];

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

        return 0 == _board[pos.y, pos.x];
    }
    // フィールド内にセットする
    public bool IsSetCube(Vector2Int pos, int val)
    {
        if (!IsCanSetCube(pos)) return false;

        _board[pos.y, pos.x] = val;
        // もし中身が入っていたらエラー表記を出す
        //Debug.Assert(_Cube[pos.y, pos.x] == null);

        Vector3 world_position = transform.position + new Vector3(pos.x, pos.y, 0.0f);
        _Cube[pos.y, pos.x] = Instantiate(_prefabCube, world_position, Quaternion.identity, transform);
        _Cube[pos.y, pos.x].GetComponent<Test>().SetColorType((ColorType)val);

        return true;
    }
    // Y軸(下方向)におけるかどうかのチェック
    public bool IsNextCubeY(Vector2Int pos)
    {
        // ゼロだったらおけるようにする
        if(pos.y == 0)
        {
            return true;
        }
        if(_Cube[pos.y - 1, pos.x] != null)
        {
            return true;
        }
        return false;
    }
    // X軸(左右方向)におけるかどうかのチェック
    public bool IsNextCubeX(Vector2Int pos,int add)
    {
        //// ゼロだったらおけるようにする
        if (pos.x + add < 0 || pos.x + add > 5)
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
    private  void TempBoardClearAll()
    {
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                _tempBoard[y , x] = 0;
            }
        }
    }
    public bool IsCheckField()
    {
        int eraseCount = 0;
        ColorType cubeColor;
        
        //// これはテスト
        //_Cube[0, 5] = _Cube[0, 0];
        //Debug.Log(_Cube[0, 5]);

        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                // 仮で保存する変数を初期化する
                TempBoardClearAll();
                if(_Cube[y, x] == null)
                {
                    // 何も入っていないので処理をとばす
                    cubeColor = ColorType.None;
                    continue;
                }
                cubeColor = _Cube[y, x].GetComponent<Test>().GetColorType();
                IsRecursionCheckField(x, y, cubeColor);
                //塗りつぶせる個数をチェックする
                eraseCount = CountTempField(_tempBoard);

                if (eraseCount >= 4)
                {
                    EraseField(_tempBoard);
                    Debug.Log("けすよ");
                    return true;
                }
            }
        }


        return false;
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

    private bool IsRecursionCheckField(int x, int y, ColorType color)
    {
        //違う色なので終了
        if (!IsSameColor(color, x, y))
        {
            return false;
        }
        //同じ色の場合
        _tempBoard[y,x] = 1;        //同じ色がつながっている

        for (int dir = 0; dir < (int)Direction.max; dir++)
        {
            int indexX = x + _cubeDirection[dir].x;
            int indexY = y + _cubeDirection[dir].y;

            //範囲外はチェックしない
            if (indexX < 0) continue;
            if (indexX > _borad_Width - 1) continue;
            if (indexY < 0) continue;
            if (indexY > _borad_Height - 1) continue;

            if (_tempBoard[indexY,indexX] == 0)//すでにつながっている判定されている部分はチェックしない
            {
                // 位置をずらしてチェックする
                IsRecursionCheckField(indexX, indexY,color);
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
    // キューブを消す処理
    private void EraseField(int[,] tempField)
    {
        for (int x = 0; x < _borad_Width; x++)
        {
            int drapFall = 0;
            for (int y = 0; y < _borad_Height; y++)
            {
                if (tempField[y, x] == 1)
                {
                    // こわす.
                    if (_Cube[y, x] != null) Destroy(_Cube[y, x]);
                    _Cube[y, x] = null;
                    tempField[y, x] = 0;
                    drapFall++;
                }
                if ( y < _borad_Height - drapFall)
                {

                    Debug.Log("とおってる");
                    _Cube[y, x] = _Cube[y + drapFall, x];
                    
                    //_Cube[y, x] = _Cube[0,0];
                }
            }
        }
        //Debug.Log(_Cube[0, 0]);
        //Debug.Log(_Cube[1, 0]);

    }

#if DEBUG
    public Vector2Int SteepDescent(Vector2Int pos)
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
        return result;
    }
#endif
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
