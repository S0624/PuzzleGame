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
    // オブジェクトの取得.
    public ColorSeedCreate _seed;
    public SphereColorManager[] _colorManager;
    // Start is called before the first frame update
    void Start()
    {
        // リストの情報を取得
        var players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            if (i == 0)
            {
                _seed.InitNetworkColor();
                ColorSeedInin(_seed);
                PhotonNetwork.LocalPlayer.SetCreateSeed(_seed._upSeed, _seed._downSeed);
            }
            else
            {
                Debug.Log("通ってる");
                _seed.NetworkInitColor(players[i].GetUpDColorSeed(), players[i].GetDownDColorSeed());
                ColorSeedInin(_seed);
            }
        }

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
    /// <summary>
    /// 色の種の初期化処理
    /// </summary>
    /// <param name="seed"></param>
    private void ColorSeedInin(ColorSeedCreate seed)
    {
        foreach (var col in _colorManager)
        {
            col.SetColorSeed(seed);
            col.InitObjectName();
            col.ColorRandam();
        }
    }
}
