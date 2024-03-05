using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetMatchGameManager : MonoBehaviourPunCallbacks
{
    private bool _isDecisionButtonPush = false;
    public TextMeshProUGUI[] _text;
    // Start is called before the first frame update
    void Start()
    {
        //// リストの情報を取得
        //var players = PhotonNetwork.PlayerList;
        //if (photonView.IsMine)
        //{
        //    if(Input.GetKeyDown("z"))
        //    {
        //        foreach (var player in players)
        //        {
        //            Debug.Log($"{player.NickName}({player.ActorNumber}) - {player.GetButtonState()}");
        //        }
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        // リストの情報を取得
        var players = PhotonNetwork.PlayerList;
        if (Input.GetKeyDown("z"))
        {
            if (_isDecisionButtonPush)
            {
                _isDecisionButtonPush = false;
            }
            else
            {
                _isDecisionButtonPush = true;
            }
            Debug.Log("とおってる");
        }
        PhotonNetwork.LocalPlayer.ButtonDown(_isDecisionButtonPush);
        if (photonView.IsMine)
        {
            foreach (var player in players)
            {
                Debug.Log($"{player.NickName}({player.ActorNumber}) - {player.GetButtonState()}");
            }
        }
        // テスト用
        for (int i = 0; i < players.Length; i++)
        {
            _text[i].text = players[i].GetButtonState().ToString();
        }

    }
}
