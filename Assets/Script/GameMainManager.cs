﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainManager : MonoBehaviour
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
    public TestController _testController;
    public TestMove _move;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // キューブの回転処理.
        _move.RotaUpdate();
        // テスト用 ゲームオーバーになったら画像を表示
        GenereteGameOver();
        GenereteAllClear();
    }
    void FixedUpdate()
    {
        // キューブの移動処理.
        _move.MoveUpdate();
    }
    // テスト用 ゲームオーバーになったら画像を表示
    private void GenereteGameOver()
    {
        if (_gameOverTex == null)
        {
            if (_testController.IsGameOver())
            {
                _gameOverTex = Instantiate(GameOverImg);
                _gameOverTex.transform.SetParent(Canvas.transform, false);
            }
        }
    }
    // テスト用 全消しになったら画像を表示
    private void GenereteAllClear()
    {
        if (_allClearTex == null)
        {
            if (_testController.FieldAllClear())
            {
                _allClearTex = Instantiate(AllClearImg);
                _allClearTex.transform.SetParent(Canvas.transform, false);
            }
        }
        else
        {
            if (!_testController.FieldAllClear())
            {
                Destroy(_allClearTex);
            }
        }
    }
}
