using UnityEngine;
using TMPro;
// テスト用のscript
// テキストを表示させる(連鎖数)
public class TestChain : MonoBehaviour
{
    // 仮実装用
    public FieldData _testController;
    // テキストではなくテキストメッシュプロを使用している.
    private TextMeshProUGUI _chainText;
    // スコアを入れる用の変数.
    private int _chain;
    private int _timer = 0;
    private int _prevChain = 0;
    // Start is called before the first frame update
    void Start()
    {
        //_testController = GameObject.Find("Field").GetComponent<TestController>();
        // 初期化処理.
        _chainText = GetComponent<TextMeshProUGUI>();
        // HACK 仮で適当な値を入れている
        _chain = 00000000;
    }

    // Update is called once per frame
    void Update()
    {
        _chain = _testController.SetChain();
        // 本来ここではやらないほうがいい処理、テスト用に作っているので仮実装
        // 連鎖数の更新.
        // 基本的に前保存した数より大きかったら処理をするが、
        // ゼロに戻るとき一瞬で元に戻ってしまうため
        // 連鎖数が前保存したものより小さければ時間を図って処理をさせている.
        if (_timer > 60 || _prevChain < _chain)
        {
            _prevChain = _chain;
            _chainText.text = _chain.ToString();
            _timer = 0;
        }
    }
    private void FixedUpdate()
    {
        _timer++;
    }
}
