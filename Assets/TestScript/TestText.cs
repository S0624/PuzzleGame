using UnityEngine;
using TMPro;
// テスト用のscript
// テキストを表示させる(連鎖数)
public class TestText : MonoBehaviour
{
    // 仮実装用
    //public TestController _testController;
    // テキストではなくテキストメッシュプロを使用している.
    private TextMeshProUGUI _obstacleText;
    // スコアを入れる用の変数.
    private int _obs;
    //private int _timer = 0;
    //private int _prevChain = 0;
    // Start is called before the first frame update
    void Start()
    {
        //_testController = GameObject.Find("Field").GetComponent<TestController>();
        // 初期化処理.
        _obstacleText = GetComponent<TextMeshProUGUI>();
        // HACK 仮で適当な値を入れている
        _obs = 00000000;
    }

    // Update is called once per frame
    void Update()
    {
        //_obs = _testController.SetChain();
        // 本来ここではやらないほうがいい処理、テスト用に作っているので仮実装
        // 連鎖数の更新.
        // 基本的に前保存した数より大きかったら処理をするが、
        // ゼロに戻るとき一瞬で元に戻ってしまうため
        // 連鎖数が前保存したものより小さければ時間を図って処理をさせている.
        //if (_timer > 60 || _prevChain < _obs)
        {
            //_prevChain = _obs;
            _obstacleText.text = _obs.ToString();
            //_timer = 0;
        }
    }
    public void SetObstacleCount(int obs)
    {
        _obs = obs;
    }
}
