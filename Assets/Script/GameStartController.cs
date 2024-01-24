using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartController : MonoBehaviour
{
	//　スタート時に表示するUIのプレハブ
	public GameObject _startPrefab;
	//　スタート画面UIのインスタンス
	private GameObject _startUI;
	// ボタンの処理をするための変数.
	private InputManager _input;
	// スタート画面を開いているかどうか.
	private bool _isStartCanvas = false;
	// 設定画面を押したかどうか.
	private bool _isSetting = false;
	// 音を鳴らすためのサウンドの取得
	private SoundManager _soundManager;
	public SettingController _settingCanvas;
	private void Start()
	{
		_input = new InputManager();
		_input.Enable();
		StartSettingOpen();
		_soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
	}

	// pauseの更新処理.
	public void StartSettingUpdate()
	{
		//StartSettingOpen();
		StartSettingOpenUpdate();
	}
	// スタート画面を開く処理.
	private void StartSettingOpen()
	{
		// 生成されていなかったら生成する.
		if (_startUI == null)
		{
			_isStartCanvas = true;
			_startUI = GameObject.Instantiate(_startPrefab) as GameObject;
			Time.timeScale = 0f;
		}

	}
	// 開いているときの処理.
	private void StartSettingOpenUpdate()
	{
		_startUI.GetComponent<CursorController>().Decision(_isSetting);
		_settingCanvas.StartSettingOpenUpdate();
		if (_settingCanvas.IsSettingCanvas()) return;

		// pauseが開いていなかったら処理を飛ばす.
		if (!_isStartCanvas) return;
		// えらんでいる番号を取得する.
		var selectNum = _startUI.GetComponent<CursorController>().SelectNum();
		// Aボタンを押したときの処理.
		if (_input.UI.Submit.WasPerformedThisFrame())
		{
			_soundManager.SEPlay(0);
			// ゲームに戻る.
			if (selectNum == 0)
			{
				StartCanvasClose();
			}
			// ゲームを中断しselect画面に戻る.
			else if (selectNum == 1)
			{
				if (!_isSetting)
				{
					_settingCanvas.StartSettingOpen();
					_isSetting = true;
				}
			}
		}
		_isSetting = _settingCanvas.IsSettingCanvas();
	}
	// スタート画面を閉じる処理.
	private void StartCanvasClose()
	{
		// フラグの初期化.
		_isStartCanvas = false;
		// キャンバスを削除.
		Destroy(_startUI);
		Time.timeScale = 1f;
	}
	// スタート画面を開いているかどうか.
	public bool IsStartCanvas()
	{
		return _isStartCanvas;
	}
	//// モードセレクトに戻るを押されたかどうか.
	//public bool IsSelectPush()
	//{
	//	return _isSelectScene;
	//}
}
