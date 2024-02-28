using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public Sprite[] _numImage;
    public Image[] _scoreImage;
    public Image[] _chainImage;
    // 仮実装用
    public FieldData _fieldController;
    public SphereMove _sphereMove;
    // スコアを入れる用の変数.
    private int _score = 0;
    // スコアを入れる用の変数.
    private int _chain = 0;
    private int _timer = 0;
    private int _prevChain = 0;
    [SerializeField] private GameObject _chainTextUI = default;
    private GameObject _chainText;
    private bool _isChainText = false;
    private int _chainTimer = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ScoreUpdate();
        ChainUpdate();
    }
    private void FixedUpdate()
    {
        _timer++;
    }

    private void ScoreUpdate()
    {
        _score += _fieldController.EraseScore();
        _score += _sphereMove.MoveScore();
        // 本来ここではやらないほうがいい処理、テスト用に作っているので仮実装
        _fieldController.SetScore();
        _sphereMove.SetScore();
        // スコアの更新.
        for (int i = 0; i < _scoreImage.Length; i++)
        {
            ScoreTextUpdate(i, GetDigit(_score,i + 1));
        }
    }
    private void ScoreTextUpdate(int num,int scoreNum)
    {
        _scoreImage[num].sprite = _numImage[scoreNum];
    }
    private void ChainUpdate()
    {
        _chain = _fieldController.SetChain();
        // 本来ここではやらないほうがいい処理、テスト用に作っているので仮実装
        // 連鎖数の更新.
        // 基本的に前保存した数より大きかったら処理をするが、
        // ゼロに戻るとき一瞬で元に戻ってしまうため
        // 連鎖数が前保存したものより小さければ時間を図って処理をさせている.
        if (_timer > 60 && _prevChain < _chain)
        {
            _isChainText = true;
            //_fieldController.ErasePos();

            _timer = 0;
        }
        _prevChain = _chain;

        ChainDisplay();
        // スコアの更新.
        for (int i = 0; i < _chainImage.Length; i++)
        {
            ChainImageUpdate(i, GetDigit(_chain, i + 1));
        }
    }
    // 連鎖数を表示する
    private void ChainDisplay()
    {
        if (_isChainText)
        {
            if (_chainText == null)
            {
                _chainText = Instantiate(_chainTextUI, new Vector3(_fieldController.ErasePos().x, _fieldController.ErasePos().y, _chainTextUI.transform.position.z), _chainTextUI.transform.rotation);
                for (int i = 0; i < _chainImage.Length; i++)
                {
                    _chainText.GetComponent<ImageTextUpdate>().ChainImageUpdate(i, _numImage[GetDigit(_chain, i + 1)]);
                }
            }
            _chainTimer++;
        }
        if(_chainTimer > 30)
        {
            _chainTimer = 0;
            _isChainText = false;
            if (_chainText != null)
            {
                Destroy(_chainText);
            }
        }
    }
    private void ChainImageUpdate(int num,int chainNum)
    {
        _chainImage[num].sprite = _numImage[chainNum];
    }
    // 桁を取得する.
    public int GetDigit(int num,int digit)
    {
        return (int)( num / Mathf.Pow(10, digit - 1)) % 10;
    }
}
