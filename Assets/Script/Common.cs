using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    None,
    Green,    // 緑.
    Red,    // 赤.
    Yellow,    // 黄色.
    Blue,    // 青.
    Purple,    // 紫.
    PuyoMax,       // ぷよ最大値(仮).
    hindrance,     // おじゃま(仮).
};
public enum Direction
{
    Down,
    Left,
    Up,
    Right,
    max,
}
public enum RotaState
{
    left,
    right,
    max,
}
public struct ColorArray
{
    public int upColor;
    public int downColor;
}


public class Common : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
