using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColorManager : MonoBehaviour
{

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
    // HACK マジックナンバー推しなんかってレベルのマジックナンバーがいるんですけど・・・
    [SerializeField] private GameObject _nowCube;
    [SerializeField] private GameObject _nextCube;
    [SerializeField] private GameObject _nextnextCube;
    // Test たすけて
    private GameObject[] _cubeColor = new GameObject[3];

    private int[] _colorData = new int[6];
    private string[] _colorNameData = new string[6];

    // テスト用なのでけす
    int _testCount = 0;
    // 色の番号
    private int _colorNum;

    // Start is called before the first frame update
    void Start()
    {
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
                _colorData[_test] = ColoCheck(child.GetComponent<Renderer>().material.color);
                _colorNameData[_test] = child.name;
                _test++;
            }
        }
    }


    private void ColorRandam()
    {
        //　雑に色付けようとしたけどバグってる
        // 子オブジェクトを全て取得する
        foreach (var cube in _cubeColor)
        {
            foreach (Transform child in cube.transform)
            {
                _colorNum = Random.Range((int)ColorType.Green, (int)ColorType.Yellow);
                child.GetComponent<Renderer>().material.color = color_table[_colorNum];
            }
        }
    }

    private int ColoCheck(Color color)
    {
        for (int i = 0; i < color_table.Length; i++)
        {
            if (color_table[i] == color)
            {
                return i;
            }
        }
        // 同じ色がなかった場合エラーなので-1を入れる
        return -1;
    }

    public void ColorChenge()
    {
        // HACK なんか・・・なんかなぁ 色変えの処理のとこ
        int test = 2;
        for (int i = 0; i < _cubeColor.Length - 1; i++)
        {
            foreach (Transform child in _cubeColor[i].transform)
            {
                child.GetComponent<Renderer>().material.color = color_table[_colorData[i + test]];
                test++;
            }
            test = 3;
        }

        foreach (Transform child in _cubeColor[2].transform)
        {
#if true
            _colorNum = Random.Range((int)ColorType.Green, (int)ColorType.PuyoMax - 3);
#endif
            child.GetComponent<Renderer>().material.color = color_table[_colorNum];
        }

        int _test = 0;
        foreach (var cube in _cubeColor)
        {
            foreach (Transform child in cube.transform)
            {
                _colorData[_test] = ColoCheck(child.GetComponent<Renderer>().material.color);
                _colorNameData[_test] = child.name;
                _test++;
            }
        }
    }

    public int GetColorNumber(string name)
    {
        for (int i = 0; i < _cubeColor.Length; i++)
        {
            if (_colorNameData[i] == name)
            {
                return _colorData[i];
            }
        }
        return 0;
    }
}
