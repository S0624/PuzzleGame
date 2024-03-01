using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 色のタイプ
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
// 方向
public enum Direction
{
    Down,
    Left,
    Up,
    Right,
    max,
}
// 回転の状態
public enum RotaState
{
    left,
    right,
    max,
}
// 色の取得
public struct ColorArray
{
    public int upColor; // 上の色
    public int downColor; // 下の色
}

// 音データ(BGM)
public enum SoundBGMData
{
    Tan,
    Grassland,
    Pop,
    Gothic,
    Circus,
    Matching,
    Title,
    Select,
}
// 音データ(SE)
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

// フィールドの中身のデータ
public enum FieldContentsData
{
    None,       // 空
    Octopus,    // たこ.
    Obstacle,    // おじゃま.
}

// ネットワークの状態の画像
public enum NetworkStateImage
{
    Sleep,
    PreparationNow,
    PreparationOK,
}

// プレイヤーの番号
public enum PlayerNumber
{
    // 左のプレイヤー
    LeftPlayer,
    // 右のプレイヤー
    RightPlayer,
}
