using UnityEngine;
using TMPro;

public class FadeManager : MonoBehaviour
{
    // フェードキャンバスの取得.
    [SerializeField] private Fade _fade;
    // フェードのフラグ.
    public bool _isFade = false;
    public bool _isTest = false;
    public TextMeshProUGUI _test;
    private void Update()
    {
        if (_test != null)
        {
            _test.text = _fade.cutoutRange.ToString() + "\n" + _isFade;
        }
        //Debug.Log(_fade.cutoutRange);
        //Debug.Log(_isFade + "ぱぱぱぱぱ" + _fade.cutoutRange);
        // 押されていたらフェードイン.
        if (_isFade)
        {
            Debug.Log("in");
            _fade.FadeIn();
        }
        // 押されていなかったらフェードアウト.
        else
        {
            Debug.Log("out");
            _fade.FadeOut();
        }
    }
}
