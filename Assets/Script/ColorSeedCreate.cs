using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// カラーの種生成.
public class ColorSeedCreate : MonoBehaviour
{
    private int _seedMax = 50;
    // カラー種.
    public ColorArray[] _colorSeed = new ColorArray[50];
    // 色の番号
    private int _colorNum;
    private int _colorNum2;
    static public int _colorDifficulty = 1;
    public int[] _upSeed = new int[50];
    public int[] _downSeed = new int[50];

    // カラーの種生成.
    public void InitColor()
    {
        SeedInit();
        ColorRandom();
    }
    // カラーの種生成.
    public void InitNetworkColor()
    {
        ColorRandom();
    }
    private void SeedInit()
    {
        _colorSeed = new ColorArray[_seedMax];
        _upSeed = new int[50];
        _downSeed = new int[_seedMax];
    }
    private void ColorRandom()
    {
        for (int i = 0; i < _colorSeed.Length; i++)
        {
#if true
            _colorNum = Random.Range((int)ColorType.Green, (int)ColorType.PuyoMax - _colorDifficulty);
            _colorSeed[i].upColor = _colorNum;
            _upSeed[i] = _colorNum;
            _colorNum2 = Random.Range((int)ColorType.Green, (int)ColorType.PuyoMax - _colorDifficulty);
            _colorSeed[i].downColor = _colorNum2;
            _downSeed[i] = _colorNum2;
#endif
        }
    }


    public void ColorPreparation(int color)
    {
        _colorDifficulty = color;
    }

    public bool NetworkInitColor(int[]upcolor, int[] downcolor)
    {
        //　nullだったら処理しない（エラーになるのを防ぐため）
        if (upcolor == null || downcolor == null)
        {
            return false;
        }
        Debug.Log(downcolor[0]);
        for (int i = 0; i < _colorSeed.Length; i++)
        {
            _colorSeed[i].upColor = upcolor[i];
            _upSeed[i] = upcolor[i];
            _colorSeed[i].downColor = downcolor[i];
            _downSeed[i] = downcolor[i];
        }
        return true;
    }
    // 番号に沿った色を返す.
    public int SetColorNum(int num, Direction dir)
    {
        if (dir == Direction.Up)
        {
            return _upSeed[num];
        }
        if (dir == Direction.Down)
        {
            return _downSeed[num];
        }
        // ここまで来たらエラーになる.
        Debug.Assert(true);
        return 0;
    }
    public int SeedLength()
    {
        return _colorSeed.Length;
    }
}
