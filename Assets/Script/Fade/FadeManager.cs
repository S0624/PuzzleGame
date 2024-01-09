using UnityEngine;

public class FadeManager : MonoBehaviour
{
    // フェードキャンバスの取得.
    [SerializeField] private Fade _fade;
    // フェードのフラグ.
    public bool _isFade = false;

    private void FixedUpdate()
    {
        // 押されていたらフェードイン.
        if (_isFade)
        {
            _fade.FadeIn(1.0f);
        }
        // 押されていなかったらフェードアウト.
        else
        {
            _fade.FadeOut(1.0f);
        }
    }
}
