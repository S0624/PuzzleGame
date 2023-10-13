using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// コメントがないよ？

public class TestController : MonoBehaviour
{
    // ボードの横の最大値(6 * 14).
    private const int _borad_Width = 6;
    // ボードの縦の最大値.
    private const int _borad_Height = 14;

    [SerializeField] GameObject _prefabCube = default!;

    int[,] _board = new int[_borad_Height, _borad_Width];
    GameObject[,] _Cube = new GameObject[_borad_Height, _borad_Width];

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
    }
    // 生成する.
    public void Generate()
    {
        // 生成する前に消す.
        ClearAll();

        for (int y = 0; y < _borad_Height; y++)
        {
            for (int x = 0; x < _borad_Width; x++)
            {
                IsSetCube(new Vector2Int(x, y), Random.Range(0, (int)ColorType.max));
            }
        }
    }
    // 範囲外じゃないかの判定
    public bool IsValidated(Vector2Int pos)
    {
        if (0 <= pos.x && pos.x < _borad_Width
        && 0 <= pos.y && pos.y < _borad_Height)
        {
            return true;
        }
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
        Debug.Assert(_Cube[pos.y, pos.x] == null);
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

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
