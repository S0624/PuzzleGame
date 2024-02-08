using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// これはアタッチされていなかったらアタッチするという関数.
//[RequireComponent(typeof(Renderer))]
public class SphereData : MonoBehaviour
{
    [SerializeField] private Renderer my_renderer = default!;// 自分自身のマテリアルを登録しておく(GetComponentをなくす).
    //[SerializeField] private Renderer my_renderer = default!;// 自分自身のマテリアルを登録しておく(GetComponentをなくす).
    private ColorType _type = ColorType.PuyoMax;
    private Rigidbody _rigidbody;
    private ColorTable _colorTable;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.IsSleeping();
    }
    public void SetColorType(ColorType type)
    {
        _type = type;

        _colorTable = GameObject.Find("ColorManager").GetComponent<ColorTable>();
        my_renderer.material.color = _colorTable.GetColor((int)_type);
    }

    public ColorType GetColorType()
    {
        return _type;
    }
    // 点滅するときに色のアルファ値をいじる
    public void ChangeColor(float alpha)
    {
        // 内部時刻timeにおけるアルファ値を反映
        var color = my_renderer.material.color;
        //Debug.Log(alpha);
        color.a = alpha;
        my_renderer.material.color = color;
    }
    // 点滅するときに色のアルファ値をいじる
    public void ChangeSize(Vector3 size)
    {
        my_renderer.transform.localScale = size;
    }

    public void SetPos(Vector3 pos)
    {
        this.transform.localPosition = pos;
    }
    public bool IsMoveSphere()
    {
        // HACK ?
        _rigidbody = GetComponent<Rigidbody>();
        // オブジェクトが動いていないかどうか
        if (!_rigidbody.IsSleeping())
        {
            return true;
        }
        return false;
        
    }
}
