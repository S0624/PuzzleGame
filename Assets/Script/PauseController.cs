using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PauseController : MonoBehaviour
{
	//　ポーズした時に表示するUIのプレハブ
	public GameObject _pauseUIPrefab;
	//　ポーズUIのインスタンス
	private GameObject _pauseUI;

	// Update is called once per frame
	private void Update()
	{
		if (Input.GetKeyDown("q"))
		{
			// 生成されていなかったら生成する.
			if (_pauseUI == null)
			{
				_pauseUI = GameObject.Instantiate(_pauseUIPrefab) as GameObject;
				Time.timeScale = 0f;
			}
			// もう一回押したら閉じる.
			else
			{
				Destroy(_pauseUI);
				Time.timeScale = 1f;
			}
		}
	}
}
