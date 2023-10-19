using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// コメントがないよ？
struct CubeCheckPos
{
    public Vector2Int[] direction;
}

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

    // ボードの横の最大値(6 * 14).
    private const int _borad_Width = 6;
    // ボードの縦の最大値.
    private const int _borad_Height = 15;

    [SerializeField] GameObject _prefabCube = default;

    int[,] _board = new int[_borad_Height, _borad_Width];
    GameObject[,] _Cube = new GameObject[_borad_Height, _borad_Width];

    int[,] _tempBoard = new int[_borad_Height, _borad_Width];
    CubeCheckPos _cubeDirection;
    Vector2Int[] _cubeD = new Vector2Int[(int)Direction.max];

    // ボードの中を全消しする(クリアする).
    public void ClearAll()
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
        //Generate();

        // HACK テストテストテストテスト
        // タテヨコ方向のチェックをforで回せないか実装してみるためのテスト実装(四つ分配列でもつ)
        CubeCheckPos _cubeDirection = new CubeCheckPos();
        _cubeD[(int)Direction.Right] = new Vector2Int(1, 0);
        _cubeD[(int)Direction.Left] = new Vector2Int(-1, 0);
        _cubeD[(int)Direction.Up] = new Vector2Int(0, 1);
        _cubeD[(int)Direction.Down] = new Vector2Int(0, -1);
        //_cubeDirection.direction[(int)Direction.Right] = new Vector2Int(1, 0);
        //_cubeDirection.direction[(int)Direction.Left] = new Vector2Int(-1, 0);
        //_cubeDirection.direction[(int)Direction.Up] = new Vector2Int(0, 1);
        //_cubeDirection.direction[(int)Direction.Down] = new Vector2Int(0, -1);
    }
    // 生成する.
    public void Generate()
    {
        // 生成する前に消す.
        ClearAll();

        // ランダムにフィールド上すべてに生成する
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                IsSetCube(new Vector2Int(x, y), Random.Range(0, (int)ColorType.PuyoMax));
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
    public bool IsNextCube(Vector2Int pos)
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

    void TempBoardClearAll()
    {
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                _tempBoard[y , x] = 0;
            }
        }
    }
    //public bool IsCheckField(Vector2Int pos, int val)
    public bool IsCheckField()
    {
        int testCount = 0;

        ColorType testcolor;
        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                if(_Cube[y, x] == null)
                {
                    // 何も入っていない
                    testcolor = ColorType.None;
                    continue;
                }
                testcolor = _Cube[y, x].GetComponent<Test>().GetColorType();
                testCheckField(x, y, testcolor);
                //塗りつぶせる個数をチェックする
                testCount = countTempField(_tempBoard);
                //if (testcolor == ColorType.Green)
                //{
                //    testCount++;
                //}
            }
        }

        if (testCount >= 4)
        {
            Debug.Log("けすよ");
            return true;
        }
        return false;
    }
    bool isSameColor(ColorType colorType, int x, int y)
    {
        if (x < 0) return false;
        if (x > _borad_Width - 1) return false;
        if (y < 0) return false;
        if (y > _borad_Height - 1) return false;

        //指定した位置に指定された色が置かれているかチェックをする
        if (_Cube[y, x].GetComponent<Test>().GetColorType() == colorType) return true;

        return false;
    }

    bool testCheckField(int x, int y, ColorType color)
    {
        //違う色なので終了
        if (!isSameColor(color, x, y))
        {
            return false;
        }
        //同じ色の場合
        _tempBoard[y,x] = 1;        //同じ色がつながっている

        //for (var  dir : _cubeDirection)
        for (int dir = 0; dir < (int)Direction.max; dir++)
        {
            int indexX = x + _cubeD[dir].x;
            int indexY = y + _cubeD[dir].y;

            //範囲外はチェックしない
            if (indexX < 0) continue;
            if (indexX > _borad_Width - 1) continue;
            if (indexY < 0) continue;
            if (indexY > _borad_Height - 1) continue;

            if (_tempBoard[indexY,indexX] == 0)//すでにつながっている判定されている部分はチェックしない
            {
                testCheckField(indexX, indexY,color);
            }
        }

        Debug.Log("はい");
        return false;
    }

    int countTempField(int[,] tempField)
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
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
