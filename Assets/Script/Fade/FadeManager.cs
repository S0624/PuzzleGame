using UnityEngine;
using TMPro;

public class FadeManager : MonoBehaviour
{
    // フェードキャンバスの取得.
    [SerializeField] private Fade _fade;
    // フェードのフラグ.
    public bool _isFade = false;
    private void Update()
    {
        if (_isFade)
        {
            _fade.FadeIn();
        }
        // 押されていなかったらフェードアウト.
        else
        {
            _fade.FadeOut();
        }
    }
}
