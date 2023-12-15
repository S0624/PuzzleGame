using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainMatchManager : MonoBehaviour
{
    // 画像をいれる
    [Header("表示させる画像たち")]
    [SerializeField] private GameObject GameOverImg;
    [SerializeField] private GameObject AllClearImg;
    // Canvasを入れるよう
    [SerializeField] private GameObject Canvas;
    private GameObject _gameOverTex = null;
    private GameObject _allClearTex = null;
    // オブジェクトの取得.
    public ColorSeedCreate _seed;
    public TestColorManager[] _colormanager;
    public TestController[] _testController;
    public TestMove[] _moveSphere;
    int[] _obstacle = new int[2];

    //public TestMove _rightMove;
    // Start is called before the first frame update
    void Start()
    {
        _seed.InitColor();
        foreach (var col in _colormanager)
        {
            col.SetColorSeed(_seed);
            col.InitObjectName();
            col.ColorRandam();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // スフィアの回転処理.
        foreach (var move in _moveSphere)
        {
            move.SphereUpdate();
        }
        for (int i = 0; i < _moveSphere.Length; i++)
        {
            _moveSphere[i].SphereReGenerete(_testController[i].IsChain(), _testController[i]);
        }
        // テスト用 ゲームオーバーになったら画像を表示
        GenereteGameOver();
        GenereteAllClear();
        // お邪魔計算.
        ObstacleCalculation();
    }
    void FixedUpdate()
    {
        // キューブの移動処理.
        foreach (var move in _moveSphere)
        {
            move.FreeFallUpdate();
        }
    }
    // テスト用 ゲームオーバーになったら画像を表示
    private void GenereteGameOver()
    {
        if (_gameOverTex == null)
        {
            foreach (var controller in _testController)
            {
                if (controller.IsGameOver())
                {
                    _gameOverTex = Instantiate(GameOverImg);
                    _gameOverTex.transform.SetParent(Canvas.transform, false);
                }
            }
        }
    }
    // テスト用 全消しになったら画像を表示
    private void GenereteAllClear()
    {
        if (_allClearTex == null)
        {
            foreach (var controller in _testController)
            {
                if (controller.FieldAllClear())
                {
                    _allClearTex = Instantiate(AllClearImg);
                    _allClearTex.transform.SetParent(Canvas.transform, false);
                }
            }
        }
        else
        {
            foreach (var controller in _testController)
            {
                if (!controller.FieldAllClear())
                {
                    Destroy(_allClearTex);
                }
            }
        }
    }
    // 邪魔スフィアの計算(相殺処理)
    private void ObstacleCalculation()
    {
        int[] add = new int[2];
        int total = 0;
        for (int i = 0; i < _testController.Length; i++)
        {
            // とりあえず値を取得するよ.
            add[i] = _testController[i].GetObstacle();
            // 今持っている数値が大きかったら代入するよ
            if(_obstacle[i] < add[i])
            {
                _obstacle[i] = add[i];
            }
            _testController[i].IsFieldUpdate();
        }
        // 計算するよ
        /// 減らしていく処理はまだ、つまりまだこれは計算だけなの
        total = _obstacle[0] - _obstacle[1];
        if(total == 0)
        {
            Debug.Log("均衡中...");
        }
        else if (total > 0)
        {
            Debug.Log("右に" + total);
        }
        else
        {
            Debug.Log("左に" + total * -1);
        }
    }
}
