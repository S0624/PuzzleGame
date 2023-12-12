using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColorManager : MonoBehaviour
{
   struct colorTestArray{
    public int upColor;
    public int downColor;
    }

// HACK なんかいい処理ないんですか別ファイルで全く同じの使ってる
// 実行時に値を取得する読み取り専用の変数を生成
static readonly Color[] color_table = new Color[] {
        Color.white,        // 白.
        Color.green,        // 緑.
        Color.red,          // 赤.
        Color.yellow,       // 黄色.
        Color.blue,         // 青.
        Color.magenta,      // 紫.
        Color.gray,         // 仮でグレーを入れる

        Color.gray,         // おじゃま(グレー)
    };
    private colorTestArray[] _testColor = new colorTestArray[10];
    // HACK マジックナンバー推しなんかってレベルのマジックナンバーがいるんですけど・・・
    [SerializeField] private GameObject _nowCube;
    [SerializeField] private GameObject _nextCube;
    [SerializeField] private GameObject _nextnextCube;
    // Test たすけて
    private GameObject[] _cubeColor = new GameObject[3];

    //private int[] _colorData = new int[6];
    private string[] _colorNameData = new string[6];

    // テスト用なのでけす
    int _testCount = 0;
    // 色の番号
    private int _colorNum;

    // Start is called before the first frame update
    void Start()
    {
        InitColor();
        //_testColor[0] = ;
        // HACK ヒィッ とんでもないコードしてる
        _cubeColor[0] = _nowCube;
        _cubeColor[1] = _nextCube;
        _cubeColor[2] = _nextnextCube;
        ColorRandam();
        int _test = 0;
        foreach (var cube in _cubeColor)
        {
            foreach (Transform child in cube.transform)
            {
                //_colorData[_test] = ColoCheck(child.GetComponent<Renderer>().material.color);
                _colorNameData[_test] = child.name;
                _test++;
            }
        }
    }
    // カラーの種生成.
    private void InitColor()
    {
        for (int i = 0; i < _testColor.Length; i++)
        {
#if true
            _colorNum = Random.Range((int)ColorType.Green, (int)ColorType.PuyoMax - 2);
            _testColor[i].upColor = _colorNum;
            _colorNum = Random.Range((int)ColorType.Green, (int)ColorType.PuyoMax - 2);
            _testColor[i].downColor = _colorNum;
            //_colorNum = Random.Range((int)ColorType.Green, (int)ColorType.PuyoMax);
            //_testColor[i].upColor = _colorNum;
            //_colorNum = Random.Range((int)ColorType.Green, (int)ColorType.PuyoMax);
            //_testColor[i].downColor = _colorNum;
#endif
        }
    }
    private void ColorRandam()
    {
        //　雑に色付けようとしたけどバグってる
        // 子オブジェクトを全て取得する
        int i = 0;
        foreach (var cube in _cubeColor)
        {
            //foreach (Transform child in cube.transform)
            {
                cube.transform.GetChild(0).GetComponent<Renderer>().material.color = color_table[_testColor[i].upColor];
                cube.transform.GetChild(1).GetComponent<Renderer>().material.color = color_table[_testColor[i].downColor];
                i++;
            }
        }
    }

    //private int ColoCheck(Color color)
    //{
    //    for (int i = 0; i < color_table.Length; i++)
    //    {
    //        if (color_table[i] == color)
    //        {
    //            return i;
    //        }
    //    }
    //    // 同じ色がなかった場合エラーなので-1を入れる
    //    return -1;
    //}

    public void ColorChenge()
    {
        _testCount++;
        // 入れ替えの処理.
        for(int i = 0; i < _cubeColor.Length; i++)
        {
            _cubeColor[i].transform.GetChild(0).GetComponent<Renderer>().material.color = color_table[_testColor[ArrayNum(i)].upColor];
            _cubeColor[i].transform.GetChild(1).GetComponent<Renderer>().material.color = color_table[_testColor[ArrayNum(i)].downColor];
        }
    }
    private int ArrayNum(int count)
    {
        if(_testCount >= _testColor.Length)
        {
            _testCount = 0;
        }
        if (_testCount + count >= _testColor.Length)
        {
            var num = (_testCount + count) - (_testColor.Length);
            return num;
        }
        return _testCount + count;
    }

    public int GetColorNumber(string name,int childNum)
    {
        for (int i = 0; i < _colorNameData.Length; i++)
        {
            if (_colorNameData[i] == name)
            {
                if (childNum == 0)
                {
                    //Debug.Log(_testCount);
                    return _testColor[_testCount].upColor;
                }
                else
                {
                    return _testColor[_testCount].downColor;
                }
            }
        }
        return 0;
    }
}
