using UnityEngine;
public class PauseController : MonoBehaviour
{
	//　ポーズした時に表示するUIのプレハブ
	public GameObject _pauseUIPrefab;
	//　ポーズUIのインスタンス
	private GameObject _pauseUI;
	// ボタンの処理をするための変数.
	private InputManager _input;
	// ポーズ画面を開いているかどうか.
	private bool _isPause = false;
	// モードセレクトに戻るを押されたかどうか.
	private bool _isSelectScene = false;
	private void Start()
	{
		_input = new InputManager();
		_input.Enable();
	}

	// pauseの更新処理.
	public void PauseUpdate()
    {
		PauseOpen();
		PauseOpenUpdate();
	}
	// ポーズ画面を開く処理.
	private void PauseOpen()
    {
		// 対象のキーを押した時の処理.
		if (_input.UI.Pause.WasPerformedThisFrame() || Input.GetKeyDown("p"))
		{
			// 生成されていなかったら生成する.
			if (_pauseUI == null)
			{
				_isPause = true;
				_pauseUI = GameObject.Instantiate(_pauseUIPrefab) as GameObject;
				Time.timeScale = 0f;
			}
			// もう一回押したら閉じる.
			else
			{
				PauseClose();
			}
		}
	}
	// 開いているときの処理.
	private void PauseOpenUpdate()
	{
		// pauseが開いていなかったら処理を飛ばす.
		if (!_isPause) return;
		// えらんでいる番号を取得する.
		var selectNum = _pauseUI.GetComponent<CursorController>().SelectNum();
		// Aボタンを押したときの処理.
		if (_input.UI.Submit.WasPerformedThisFrame())
		{
			// ゲームに戻る.
			if (selectNum == 0)
			{
				PauseClose();
			}
			// ゲームを中断しselect画面に戻る.
			else
			{
				_isSelectScene = true;
				// 閉じる.
				PauseClose();
				//Debug.Log("しーんいこうするよ");
			}
		}
	}
	// ポーズ画面を閉じる処理.
	private void PauseClose()
    {
		// フラグの初期化.
		_isPause = false;
		// キャンバスを削除.
		Destroy(_pauseUI);
		Time.timeScale = 1f;
	}
	// ポーズ画面を開いているかどうか.
	public bool IsPause()
    {
		return _isPause;
    }
	// モードセレクトに戻るを押されたかどうか.
	public bool IsSelectPush()
    {
		return _isSelectScene;
    }
}
