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
    public ColorSeedCreate _seed;
    public TestColorManager _colormanager;
    public FieldData _field;
    public SphereMove _move;
    public PauseController _pause;
    public LoadSceneManager _scene;
    // ゲームオーバーかどうかのフラグを取得する.
    private bool _isGameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        _seed.InitColor();
        _colormanager.SetColorSeed(_seed);
        _colormanager.InitObjectName();
        _colormanager.ColorRandam();
    }

    // Update is called once per frame
    private void Update()
    {
        // ポーズ画面を開いていたら処理を止める.
        //if (_pause.IsPause()) return;
        if (!_pause.IsPause())
        {
            // テスト用 ゲームオーバーになったら画像を表示
            GenereteGameOver();
            GenereteAllClear();
            // ゲームオーバーになったら処理を止めるよ.
            if (_isGameOver) return;
            // キューブの回転処理.
            _move.SphereUpdate();
            _move.InstallationProcess(_field.IsSetSphere(), _field);
            // フィールドの更新処理.
            _field.FieldUpdate();
        }
        // pauseの更新処理.
        _pause.PauseUpdate();
        if(_pause.IsSelectPush())
        {
            _scene.PauseTransitionScene();
        }
    }
    void FixedUpdate()
    {
        // ポーズ画面を開いていたら処理を止める.
        if (_pause.IsPause()) return;
        // ゲームオーバーになったら処理を止めるよ.
        if (_isGameOver) return;
        // キューブの移動処理.
        _move.FreeFallUpdate();
    }
    // テスト用 ゲームオーバーになったら画像を表示
    private void GenereteGameOver()
    {
        if (_gameOverTex == null)
        {
            if (_field.IsGameOver())
            {
                _isGameOver = true;
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
            if (_field.FieldAllClear())
            {
                _allClearTex = Instantiate(AllClearImg);
                _allClearTex.transform.SetParent(Canvas.transform, false);
            }
        }
        else
        {
            if (!_field.FieldAllClear())
            {
                Destroy(_allClearTex);
            }
        }
    }
}
