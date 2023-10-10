using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    Green  = 0,    // 緑.
    Red    = 1,    // 赤.
    Yellow = 2,    // 黄色.
    Blue   = 3,    // 青.
    Purple = 4,    // 紫.

    max    = 5,    // 最大値(仮).
};

// これはアタッチされていなかったらアタッチするという関数.
//[RequireComponent(typeof(Renderer))]
public class Test : MonoBehaviour
{
    static readonly Color[] color_table = new Color[] {
        Color.green,        // 緑.
        Color.red,          // 赤.
        Color.yellow,       // 黄色.
        Color.blue,         // 青.
        Color.magenta,      // 紫.

        Color.gray,
    };

    [SerializeField] Renderer my_renderer = default!;// 自分自身のマテリアルを登録しておく(GetComponentをなくす).
    ColorType _type = ColorType.max;

    public void SetColorType(ColorType type)
    {
        _type = type;

        my_renderer.material.color = color_table[(int)_type];
    }
    public ColorType GetColorType()
    {
        return _type;
    }

    public void SetPos(Vector3 pos)
    {
        this.transform.localPosition = pos;
    }
}
