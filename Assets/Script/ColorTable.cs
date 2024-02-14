using UnityEngine;

public class ColorTable : MonoBehaviour
{
    // 実行時に値を取得する読み取り専用の変数を生成
    public static readonly Color[] color_table = new Color[] {
        Color.white,        // 緑.
        Color.green,        // 緑.
        Color.red,          // 赤.
        Color.yellow,       // 黄色.
        Color.cyan,         // 青.
        Color.magenta,      // 紫.
        Color.gray,         // 仮でグレーを入れる

        Color.gray,         // おじゃま(グレー)
    };

    public Color GetColor(int colorNum)
    {
        return color_table[colorNum];
    }
}
