using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
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
    public Player[] _playerData = new Player[2];
    static public bool _isSceneTrans = false;
    
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        //// デバッグ用
        //// リストの情報を取得
        //var players = PhotonNetwork.PlayerList;
        //foreach (var player in players)
        //{
        //    Debug.Log($"{player.NickName}({player.ActorNumber}) - {player.GetButtonState()}");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if(_fade.cutoutRange == 0.0f && _isSceneTrans)
        {
            _isSceneTrans = false;
        }

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
        // シーンを移行してもよいかどうかをチェックする
        if (IsLoadSceneChenge())
        {
            _isSceneTrans = true;
            PhotonNetwork.IsMessageQueueRunning = false;
            SceneManager.LoadSceneAsync(_nextScene, LoadSceneMode.Single);
        }
    }
    /// <summary>
    /// 前のシーンに戻る
    /// </summary>
    public void PrevSceneTransition()
    {
        if (_prevScene != null)
        {
            _fadeManager._isFade = true;
            // シーンを移行してもよいかどうかをチェックする
            if (IsLoadSceneChenge())
            {
                _isSceneTrans = true;
                // サーバーから切断する
                PhotonNetwork.Disconnect();
                // シーンを移行する
                SceneManager.LoadSceneAsync(_prevScene, LoadSceneMode.Single);
            }
        }
    }
    public string PlayerData(int playerNum)
    {
        var players = PhotonNetwork.PlayerList;
        for (int i = 0;i < players.Length;i++)
        {
            _playerData[i] = players[i];
            //Debug.Log($"{players[i].NickName}({players[i].ActorNumber}) - {players[i].GetButtonState()}");
        }
        return _playerData[playerNum].NickName;
    }
    /// <summary>
    /// シーンをロードしてもいいかの検知
    /// </summary>
    /// <returns>シーンをロード可能</returns>
    private bool IsLoadSceneChenge()
    {
        // チェンジ可能かどうかのフラグ
        bool isChenge = true;
        // もし条件にあってなかったらfalseを返す
        // フェードが終わりきっているかどうか
        if (_fade.cutoutRange != 1.0f) return false;
        // フェードのフラグがたっているかどうか
        if (!_fadeManager._isFade) return false;
        // シーンを移行しても可能かどうかのフラグ
        if (_isSceneTrans) return false;
        // ここまで来れたらすべての条件をクリアしている
        return isChenge;
    }
}
