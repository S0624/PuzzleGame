using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingController : MonoBehaviour
{
	private InputState _inputManager;
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
	// サウンド種類の最大値
	private int _soundLength = 0;
	// 背景の種類の最大値
	private int _backImageMax = 0;
	// カーソルが選んでいる番号を取得する
	private int _cursorNum = 0;
	// ボタンの処理をするための変数.
	private InputManager _input;
	// 押し続けた時の処理
	private int _inputframe = 0;
	// スタート画面を開いているかどうか.
	private bool _isSettingCanvas = false;
	// サウンドのチェックに使用
	private int _soundNumPrev = 0;
	// カーソルの選択肢
	private int _music = 0;
	private int _bgm = 1;
	private int _se = 2;
	private int _background = 3;
	private int _back = 4;
	// 選択した番号
	private int _selectNum = 0;
	private int _soundNum = 0;
	private int _backNum = 0;
	private float _bgmVolume = 0;
	private float _seVolume = 0;
	private int _limitVol = 10;
	//public GameStartController _startCanvas;
	private void Start()
	{
		CursorChoices();
		_inputManager = GameObject.Find("InputManager").GetComponent<InputState>();
		_bgmVolume = _soundManager.BGMVolume() * _limitVol;
		_seVolume = _soundManager.SEVolume() * _limitVol;
        _input = new InputManager();
		_input.Enable();
		_soundLength = _soundManager._soundBGMData.Length - 1;
	}
	private void CursorChoices()
	{
		_music = 0;
		_bgm = 1;
		_se = 2;
		_background = 3;
		_back = 4;
	}	
	private void CursorChoicesSelect()
	{
		_music = -1;
		_bgm = 0;
		_se = 1;
		_background = -1;
		_back = 2;
	}

	// スタート画面を開く処理.
	public void StartSettingOpen()
	{
		// 生成されていなかったら生成する.
		if (_settingUI == null)
		{
			_isSettingCanvas = true;
			_settingUI = GameObject.Instantiate(_settingPrefab) as GameObject;
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
			// 背景の最大の数を取得する
			_backImageMax = _settingManager._backSprite.Length - 1;

			// サウンドのデータ取得
			_soundNum = MoveInput(_soundNum, _soundLength);
			_settingManager.SoundImageUpdate(_soundNum);

			// BGMを変更させる
			_bgmVolume = MoveInput((int)_bgmVolume, _limitVol, true);
			// SEを変更させる
			_seVolume = MoveInput((int)_seVolume, _limitVol, true);
			// 背景のデータ取得
			_backNum = MoveInput(_backNum, _backImageMax);
			_settingManager.ChengeBack(_backNum);
		}
	}
	// 開いているときの処理.
	public void StartSettingOpenUpdate()
	{
		// キーの入力情報取得
		_inputManager.GetInputPlayerPadNum();
		// pauseが開いていなかったら処理を飛ばす.
		if (!_isSettingCanvas) return;
		CursorControllerData();
		SettingData();
		// えらんでいる番号を取得する.
		_cursorNum = _cursor.SelectNum();

		SettingUpdate(_settingManager._isSelectSetting);

		// 設定の更新
		SettingDataUpdate();

	}
	private void SettingUpdate(bool isSelect)
    {
		if(isSelect)
        {
			CursorChoicesSelect();
		}
		else
        {
			CursorChoices();
		}
		// 開いたところのやつをいじるよ
		if (_cursorNum == _music)
		{
			_soundNum = MoveInput(_soundNum, _soundLength);

			SoundCheck();

		}
		// ゲームを中断しselect画面に戻る.
		else if (_cursorNum == _bgm)
		{
			// BGMを変更させる
			_bgmVolume = MoveInput((int)_bgmVolume, _limitVol, true);
		}
		else if (_cursorNum == _se)
		{
			// SEを変更させる
			_seVolume = MoveInput((int)_seVolume, _limitVol, true);
		}
		else if (_cursorNum == _background)
		{
			_backNum = MoveInput(_backNum, _backImageMax);
		}
		// Aボタンを押したときの処理.
		if (_input.UI.Submit.WasPerformedThisFrame())
		{
			// ゲームに戻る.
			if (_cursorNum == _back)
			{
				_soundManager.SEPlay(SoundSEData.Push);
				SettingCanvasClose();
			}
		}
	}
	// 設定の更新
	private void SettingDataUpdate()
    {
		_settingManager.SoundImageUpdate(_soundNum);
		_settingManager.BGMVolumeChenge(_bgmVolume / _limitVol);
		_settingManager.SEVolumeChenge(_seVolume / _limitVol);
		SoundVolumeChenge();
		_settingManager.ChengeBack(_backNum);
	}
	// テスト実装
	private int MoveInput(int num, int max, bool volFlag = false)
	{
		_selectNum = num;
		// 入力情報の取得.
		var isNowAction = _input.UI.CursorMove;
		Vector2 moveInput = _inputManager.GetInputMoveDate();
		if (_inputManager.IsMovePressed())
		{
			_inputframe++;
		}
		// 左右の入力検知.
		if (moveInput.x > 0)
		{
			if (IsPressKey() || isNowAction.WasPressedThisFrame())
			{
				_soundManager.SEPlay(SoundSEData.Select);
				_selectNum++;
				_inputframe = 0;
			}
		}
		else if (moveInput.x < 0)
		{
			if (IsPressKey() || isNowAction.WasPressedThisFrame())
			{
				_soundManager.SEPlay(SoundSEData.Select);
				_selectNum--;
				_inputframe = 0;
			}
		}
		if (volFlag)
		{
			// ボリュームの時だけ戻すのではなく最大値以降大きくならないように
			if (_selectNum < 0)
			{
				_selectNum = 0;
			}
			// ボリュームの時だけ戻すのではなく最小値以降小さくならないように
			else if (_selectNum > max)
			{
				_selectNum = max;
			}
		}
		else
		{
			if (_selectNum < 0)
			{
				_selectNum = max;
			}
			else if (_selectNum > max)
			{
				_selectNum = 0;
			}
		}
		return _selectNum;
	}
	//  キーの入力状態.
	private bool IsPressKey()
	{
		if (_inputframe > 10)
		{
			return true;
		}
		return false;
	}

	// サウンドをいじる(テスト実装)
	private void SoundCheck()
	{
		if (_selectNum != _soundNumPrev)
		{
			_soundManager.BGMChenge(_selectNum);
		}
		_soundNumPrev = _selectNum;
	}
	private void SoundVolumeChenge()
    {
		_soundManager.SoundBGMVolume(_settingManager.BGMVolume());
		_soundManager.SoundSEVolume(_settingManager.SEVolume());
	}
	// スタート画面を閉じる処理.
	public void SettingCanvasClose()
	{
		// フラグの初期化.
		_isSettingCanvas = false;
		// キャンバスを削除.
		Destroy(_settingUI);
	}
	// スタート画面を開いているかどうか.
	public bool IsSettingCanvas()
	{
		return _isSettingCanvas;
	}

}
