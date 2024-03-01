using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

// ネットワーク用のシーン移行処理
public class NetworkTransition : MonoBehaviourPunCallbacks
{
    public string _nextScene;
    public string _prevScene;
    public PhotonView _view;
    public FadeManager _fadeManager;
    public Fade _fade;
    private 
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        // デバッグ用
        // リストの情報を取得
        var players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            Debug.Log($"{player.NickName}({player.ActorNumber}) - {player.GetButtonState()}");
        }
    }

    // Update is called once per frame
    void Update()
    {


    }
    public void PhotonEventOn()
    {
        _view.RPC(nameof(SceneTransition), RpcTarget.All);
    }
    /// <summary>
    /// シーン移行します。
    /// </summary>
    [PunRPC]
    private void SceneTransition()
    {
        _fadeManager._isFade = true;
        if (_fade.cutoutRange != 1.0f) return;

        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadSceneAsync(_nextScene, LoadSceneMode.Single);
    }
    /// <summary>
    /// 前のシーンに戻る
    /// </summary>
    public void PrevSceneTransition()
    {
        if (_prevScene != null)
        {
            _fadeManager._isFade = true;
            if (_fade.cutoutRange != 1.0f) return;
            // サーバーから切断する
            PhotonNetwork.Disconnect();
            // シーンを移行する
            SceneManager.LoadSceneAsync(_prevScene, LoadSceneMode.Single);
        }
    }
}
