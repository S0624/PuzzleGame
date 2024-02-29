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

public enum SoundBGMData
{
    BGM,
    BGM1,
    BGM2,
    TitleBGM,
    SelectBGM,
}
public enum SoundSEData
{
    Push,
    Rota,
    Select,
    TitlePushSE,
    MoveSE,
    CancelSE,
    EraseSE,
    Ready,
    Go,
    AllClear,
    GameOver,
}

public enum FieldContentsData
{
    None,       // 空
    Octopus,    // たこ.
    Obstacle,    // おじゃま.
}

public enum PlayerNumber
{
    // 左のプレイヤー
    LeftPlayer,
    // 右のプレイヤー
    RightPlayer,
}
