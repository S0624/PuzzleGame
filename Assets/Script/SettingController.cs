using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingController : MonoBehaviour
{
	//　スタート時に表示するUIのプレハブ
	public GameObject _settingPrefab;
	//　スタート画面UIのインスタンス
	private GameObject _settingUI;
	//CursorControllerの取得
	private CursorController _cursor;
	// SettingManagerの取得
	private SettingManager _settingManager;
	// サウンドマネージャーの取得
	public SoundManager _soundManager;
	private int _soundLength = 0;
	// カーソルが選んでいる番号を取得する
	private int _cursorNum = 0;
	// ボタンの処理をするための変数.
	private InputManager _input;
	// スタート画面を開いているかどうか.
	private bool _isSettingCanvas = false;
	// 決定したかのフラグに使用
	private bool _isInput = false;
	// 選択した番号
	private int _selectNum = 0;
	//public GameStartController _startCanvas;
	private void Start()
	{
		
		_input = new InputManager();
		_input.Enable();
		_soundLength = _soundManager._soundData.Length - 1;
	}

	// スタート画面を開く処理.
	public void StartSettingOpen()
	{
		// 生成されていなかったら生成する.
		if (_settingUI == null)
		{
			_isSettingCanvas = true;
			_settingUI = GameObject.Instantiate(_settingPrefab) as GameObject;
			Time.timeScale = 0f;
		}

	}
	// データの取得.
	private void CursorControllerData()
    {
		if (_cursor == null)
		{
			_cursor = _settingUI.GetComponent<CursorController>();
		}
	}
	// データの取得.
	private void SettingData()
	{
		if (_settingManager == null)
		{
			_settingManager = _settingUI.GetComponent<SettingManager>();
		}
	}
	// 開いているときの処理.
	public void StartSettingOpenUpdate()
	{
		// pauseが開いていなかったら処理を飛ばす.
		if (!_isSettingCanvas) return;
		CursorControllerData();
		SettingData();
		// えらんでいる番号を取得する.
		_cursorNum = _cursor.SelectNum();
		// カーソルを押したかどうかのフラグを返す
		_cursor.Decision(_isInput);
		
		// 開いたところのやつをいじるよ
		if (_isInput)
		{
			if (_cursorNum == 0)
			{
				MoveInput();
				if (_input.UI.Submit.WasPerformedThisFrame())
				{
					SoundCheck();
				}
			}
			// ゲームを中断しselect画面に戻る.
			else if (_cursorNum == 1)
			{
				BGMVolumeChenge();
			}
			else if (_cursorNum == 2)
			{

			}
		}

		// Aボタンを押したときの処理.
		if (_input.UI.Submit.WasPerformedThisFrame())
		{
			// ゲームに戻る.
			if (_cursorNum == 3)
			{
				SettingCanvasClose();
			}
			else
            {
				InputCheck();
			}

		}
		else if(_input.UI.Cancel.WasPerformedThisFrame())
        {
			_isInput = false;
			_settingManager.ImageColorChenge(_isInput);
		}

		
	}
	// テスト実装
	private void MoveInput()
	{
		var _isNowAction = _input.UI.CursorMove;
		Vector2 moveInput = _input.UI.CursorMove.ReadValue<Vector2>();

		// 左右の入力検知.
		if (moveInput.x > 0 && _isNowAction.WasPressedThisFrame())
		{
			_selectNum++;
		}
		else if (moveInput.x < 0 && _isNowAction.WasPressedThisFrame())
		{
			_selectNum--;
		}
		if (_selectNum < 0)
		{
			_selectNum = _soundLength;
		}
		else if (_selectNum > _soundLength)
		{
			_selectNum = 0;
		}
		_settingManager.SoundTextUpdate(_selectNum);
	}
	// 推したかどうかのチェックをする処理.
	private void InputCheck()
    {
		//if (_isInput)
		//{
		//	_isInput = false;
		//}
		//else
        {
			_isInput = true;
        }
		_settingManager.ImageColorChenge(_isInput);
	}
	// サウンドをいじる(テスト実装)
	private void SoundCheck()
    {
		_soundManager.SoundChenge(_selectNum);
	}
	private void BGMVolumeChenge()
    {
		_soundManager.SoundBGMVolume(_settingManager.BGMVolume());
	}
	// スタート画面を閉じる処理.
	private void SettingCanvasClose()
	{
		// フラグの初期化.
		_isSettingCanvas = false;
		// キャンバスを削除.
		Destroy(_settingUI);
		Time.timeScale = 1f;
	}
	// スタート画面を開いているかどうか.
	public bool IsSettingCanvas()
	{
		return _isSettingCanvas;
	}

}
