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
	// ボタンの処理をするための変数.
	private InputManager _input;
	// スタート画面を開いているかどうか.
	private bool _isStartCanvas = false;
	// 決定したかのフラグに使用
	private bool _isInput = false;
	//public GameStartController _startCanvas;
	private void Start()
	{
		_input = new InputManager();
		_input.Enable();
	}

	//// pauseの更新処理.
	//public void StartSettingUpdate()
	//{
	//	StartSettingOpen();
	//	//StartSettingOpenUpdate();
	//}
	// スタート画面を開く処理.
	public void StartSettingOpen()
	{
		// 生成されていなかったら生成する.
		if (_settingUI == null)
		{
			_isStartCanvas = true;
			_settingUI = GameObject.Instantiate(_settingPrefab) as GameObject;
			Time.timeScale = 0f;
		}

	}
	// 開いているときの処理.
	public void StartSettingOpenUpdate()
	{
		// pauseが開いていなかったら処理を飛ばす.
		if (!_isStartCanvas) return;
		// えらんでいる番号を取得する.
		var selectNum = _settingUI.GetComponent<CursorController>().SelectNum();
		_settingUI.GetComponent<CursorController>().Decision(_isInput);
		// Aボタンを押したときの処理.
		if (_input.UI.Submit.WasPerformedThisFrame())
		{
			// ゲームに戻る.
			if (selectNum == 0)
			{
				InputCheck();
				//StartCanvasClose();
			}
			// ゲームを中断しselect画面に戻る.
			else if (selectNum == 1)
			{
				InputCheck();
				//_isSelectScene = true;
				// 閉じる.
				//PauseClose();
				//Debug.Log("しーんいこうするよ");
			}
			else if(selectNum == 2)
            {
				InputCheck();
            }
			else if (selectNum == 3)
			{
				//InputCheck();
				StartCanvasClose();
			}
		}
		else if(_input.UI.Cancel.WasPerformedThisFrame())
        {
			_isInput = false;
			_settingUI.GetComponent<CursorController>().ImageColorChenge(_isInput);
		}
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
		_settingUI.GetComponent<CursorController>().ImageColorChenge(_isInput);
	}
	// スタート画面を閉じる処理.
	private void StartCanvasClose()
	{
		// フラグの初期化.
		_isStartCanvas = false;
		// キャンバスを削除.
		Destroy(_settingUI);
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
