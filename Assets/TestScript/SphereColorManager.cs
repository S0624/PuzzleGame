using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SphereColorManager : MonoBehaviour
{
    private ColorSeedCreate _seed;
    //// HACK なんかいい処理ないんですか別ファイルで全く同じの使ってる
    //// 実行時に値を取得する読み取り専用の変数を生成
    // static readonly Color[] color_table = new Color[] {
    //    Color.white,        // 白.
    //    Color.green,        // 緑.
    //    Color.red,          // 赤.
    //    Color.yellow,       // 黄色.
    //    Color.blue,         // 青.
    //    Color.magenta,      // 紫.
    //    Color.gray,         // 仮でグレーを入れる

    //    Color.gray,         // おじゃま(グレー)
    //};

    // HACK マジックナンバー推しなんかってレベルのマジックナンバーがいるんですけど・・・
    [SerializeField] private GameObject _nowSphere;
    [SerializeField] private GameObject _nextSphere;
    [SerializeField] private GameObject _nextSecondSphere;
    // Test たすけて
    private GameObject[] _sphereColor = new GameObject[3];

    private string[] _colorNameData = new string[6];

    // カラーの取得.
    public ColorTable _colorTable;
    // テスト用なのでけす
    int _seedCount = 0;
    // 色の番号
    private int _colorNum;

   
    public void SetColorSeed(ColorSeedCreate seed)
    {
        _seed = seed;
    }
    // 子オブジェクトの名前の取得.
    public void InitObjectName()
    {
        // HACK ヒィッ とんでもないコードしてる
        _sphereColor[0] = _nowSphere;
        _sphereColor[1] = _nextSphere;
        _sphereColor[2] = _nextSecondSphere;
        int colorNum = 0;
        foreach (var sphere in _sphereColor)
        {
            foreach (Transform child in sphere.transform)
            {
                _colorNameData[colorNum] = child.name;
                colorNum++;
            }
        }
    }
    public void ColorRandam()
    {
        //　雑に色付けようとしたけどバグってる
        // 子オブジェクトを全て取得する
        int i = 0;
        foreach (var sphere in _sphereColor)
        {           
            sphere.transform.GetChild(0).GetComponent<Renderer>().material.color = _colorTable.GetColor(_seed.SetColorNum(i, Direction.Up));
            sphere.transform.GetChild(1).GetComponent<Renderer>().material.color = _colorTable.GetColor(_seed.SetColorNum(i, Direction.Down));
            i++;
        }
    }

    // 色の変更処理.
    public void ColorChenge(string name)
    {

        if (name == _sphereColor[0].name)
        {
            _seedCount++;
            // 入れ替えの処理.
            for (int i = 0; i < _sphereColor.Length; i++)
            {
                _sphereColor[i].transform.GetChild(0).GetComponent<Renderer>().material.color = _colorTable.GetColor(_seed.SetColorNum(ArrayNum(i), Direction.Up));
                _sphereColor[i].transform.GetChild(1).GetComponent<Renderer>().material.color = _colorTable.GetColor(_seed.SetColorNum(ArrayNum(i), Direction.Down));
            }
        }
    }
    private int ArrayNum(int count)
    {
        if (_seedCount >= _seed.SeedLength())
        {
            _seedCount = 0;
        }
        if (_seedCount + count >= _seed.SeedLength())
        {
            var num = (_seedCount + count) - (_seed.SeedLength());
            return num;
        }
        return _seedCount + count;
    }

    public int GetColorNumber(string name, int childNum)
    {
        for (int i = 0; i < _colorNameData.Length; i++)
        {
            if (_colorNameData[i] == name)
            {
                if (childNum == 0)
                {
                    return _seed.SetColorNum(_seedCount, Direction.Up);
                }
                else
                {
                    return _seed.SetColorNum(_seedCount, Direction.Down);
                }
            }
        }
        return 0;
    }
    public Color GetColor(string name, int childNum,int color)
    {
        for (int i = 0; i < _colorNameData.Length; i++)
        {
            if (_colorNameData[i] == name)
            {
                if (childNum == 0)
                {
                    return _colorTable.GetColor(color);
                }
                else
                {
                    return _colorTable.GetColor(color);
                }
            }
        }
        return Color.white;
    }
}
