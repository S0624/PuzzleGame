using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CursorAnim : MonoBehaviour
{
    // 画像の大きさ
    private Vector3 _defaultScale;
    private Vector3 _minScale = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        //_defaultScale = this.transform.localScale;
        //Debug.Log(_defaultScale);
        //transform.DOScale(new Vector3(5, 1, 0), 1.0f).SetEase(Ease.OutCirc);
    }

    // Update is called once per frame
    void Update()
    {
        CursorAnimUpdate();
    }
    // カーソルのアニメーションもどき
    private void CursorAnimUpdate()
    {
        //if (this.transform.localScale.y == _defaultScale.y)
        //{
        //    Debug.Log("くるくる");
        //    //transform.localScale = new Vector3(1.0f, 10.0f, 0.0f);
        //    //GetComponent<RectTransform>().transform.DOScaleY(2.0f, 0.5f).SetEase(Ease.OutBack);
        //    var scale = transform.DOScale(_defaultScale, 1.0f).SetEase(Ease.OutCirc);
        //    //this.transform.DOScaleY(2.0f, 0.5f).SetEase(Ease.OutBack);
        //}
        //if (this.transform.localScale.y == _minScale.y)
        //{
        //    Debug.Log("くるくるそのに");
        //    this.transform.DOScale(_defaultScale, 0.5f).SetEase(Ease.OutBack);
        //}
        //Debug.Log(transform.localScale);
    }
}
