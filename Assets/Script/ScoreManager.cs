using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    // 仮実装用
    public TestController _testController;
    public TestMove _testMove;
    // テキストではなくテキストメッシュプロを使用している.
    private TextMeshProUGUI _scoreText;
    // スコアを入れる用の変数.
    private int _score;
    // Start is called before the first frame update
    void Start()
    {
        //_testController = GameObject.Find("Field").GetComponent<TestController>();
        //_testMove = GameObject.Find("Puyo").GetComponent<TestMove>();
        // 初期化処理.
        _scoreText = GetComponent<TextMeshProUGUI>();
        // HACK 仮で適当な値を入れている
        _score = 00000000;
    }

    // Update is called once per frame
    void Update()
    {
        _score += _testController.EraseScore();
        _score += _testMove.MoveScore(); 
        // 本来ここではやらないほうがいい処理、テスト用に作っているので仮実装
        _testController.SetScore();
        _testMove.SetScore(); 
        // スコアの更新.
        _scoreText.text = _score.ToString();
    }
}
