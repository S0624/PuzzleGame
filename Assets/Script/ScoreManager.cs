using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    // テキストではなくテキストメッシュプロを使用している.
    private TextMeshProUGUI _scoreText;
    // スコアを入れる用の変数.
    private int _score;
    // Start is called before the first frame update
    void Start()
    {
        // 初期化処理.
        _scoreText = GetComponent<TextMeshProUGUI>();
        // HACK 仮で適当な値を入れている
        _score = 99999999;
    }

    // Update is called once per frame
    void Update()
    {
        // スコアの更新.
        _scoreText.text = _score.ToString();
    }
}
