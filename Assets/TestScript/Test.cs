using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum ColorType
//{
//    None,
//    Green,    // 緑.
//    Red,    // 赤.
//    Yellow,    // 黄色.
//    Blue,    // 青.
//    Purple,    // 紫.
//    PuyoMax,       // ぷよ最大値(仮).
//    hindrance,     // おじゃま(仮).
//};

// これはアタッチされていなかったらアタッチするという関数.
//[RequireComponent(typeof(Renderer))]
public class Test : MonoBehaviour
{
    // 実行時に値を取得する読み取り専用の変数を生成
    public static readonly Color[] color_table = new Color[] {
        Color.white,        // 緑.
        Color.green,        // 緑.
        Color.red,          // 赤.
        Color.yellow,       // 黄色.
        Color.blue,         // 青.
        Color.magenta,      // 紫.
        Color.gray,         // 仮でグレーを入れる

        Color.gray,         // おじゃま(グレー)
    };

    [SerializeField] private SpriteRenderer my_renderer = default!;// 自分自身のマテリアルを登録しておく(GetComponentをなくす).
    //[SerializeField] private Renderer my_renderer = default!;// 自分自身のマテリアルを登録しておく(GetComponentをなくす).
    private ColorType _type = ColorType.PuyoMax;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.IsSleeping();
    }
    public void SetColorType(ColorType type)
    {
        _type = type;

        my_renderer.material.color = color_table[(int)_type];
    }
    public ColorType GetColorType()
    {
        return _type;
    }
    // HACK
    public void ChangeColor(float alpha)
    {
        // 内部時刻timeにおけるアルファ値を反映
        var color = my_renderer.material.color;
        //Debug.Log(alpha);
        color.a = alpha;
        my_renderer.material.color = color;
    }

    public void SetPos(Vector3 pos)
    {
        this.transform.localPosition = pos;
    }
    public bool IsMoveSphere()
    {
        // オブジェクトが動いていないかどうか
        if(!_rigidbody.IsSleeping())
        {
            return true;
        }
        return false;
        
    }
}
