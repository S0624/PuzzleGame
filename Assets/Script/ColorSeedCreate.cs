using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// カラーの種生成.
public class ColorSeedCreate : MonoBehaviour
{
    // カラー種.
    private ColorArray[] _colorSeed = new ColorArray[50];
    // 色の番号
    private int _colorNum;
    private int _colorNum2;

    // カラーの種生成.
    public void InitColor()
    {
        for (int i = 0; i < _colorSeed.Length; i++)
        {
#if true
            _colorNum = Random.Range((int)ColorType.Green, (int)ColorType.PuyoMax - 2);
            _colorSeed[i].upColor = _colorNum;
            _colorNum2 = Random.Range((int)ColorType.Green, (int)ColorType.PuyoMax - 2);
            _colorSeed[i].downColor = _colorNum2;
#endif
        }
    }
    // 番号に沿った色を返す.
    public int SetColorNum(int num, Direction dir)
    {
        if (dir == Direction.Up)
        {
            return _colorSeed[num].upColor;
        }
        if (dir == Direction.Down)
        {
            return _colorSeed[num].downColor;
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
