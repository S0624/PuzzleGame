using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

// ネットワーク用のシーン移行処理
public class NetworkTransition : MonoBehaviourPunCallbacks
{
    public string _nextScene;
    public PhotonView _view;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            PhotonEventOn();
        }
    }
    private void PhotonEventOn()
    {
        //_view.RPC(nameof(SceneTransition), RpcTarget.All);
    }
    [PunRPC]
    private void SceneTransition()
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        Debug.Log("いこうしたい。");
        //SceneManager.LoadSceneAsync(_nextScene, LoadSceneMode.Single);
    }
}
