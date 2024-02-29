using System.Text;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 参加した部屋のPlayerのディスプレイの更新処理
public class ParticipationRoom : MonoBehaviourPunCallbacks
{
    // 部屋の最大人数.
    private int _roomMaxPlayer = 2;
    // 名前が被った時にどちらがどちらかわかるために数字を割りあてる
    private int _nameCoveringNum = 0;
    // 文字の取得のために使用.
    private StringBuilder[] _builder;
    // テキストの更新のために使用する変数.
    private float _elapsedTime;
    // 名前テキストの取得.
    public TextMeshProUGUI[] _nameText;
    private void Start()
    {
        _builder = new StringBuilder[_roomMaxPlayer];
        // 初期化.
        for (int i = 0; i < _roomMaxPlayer; i++)
        {
            _builder[i] = new StringBuilder();
        }
        _elapsedTime = 0f;
    }

    public void UpdateLabel()
    {
        // 0.1秒毎にテキストを更新する
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > 0.1f)
        {
            _elapsedTime = 0f;
            GetNameUpdate();
        }
    }
    // プレイヤーネームを取得する処理
    private void GetNameUpdate()
    {
        var players = PhotonNetwork.PlayerList;
        for (int i = 0; i < _roomMaxPlayer; i++)
        {
            // 初期化する.
            _builder[i].Clear();
        }
        // 名前の取得.
        NameCheck(players);
        for (int i = 0; i < players.Length; i++)
        {
            _builder[i].Append($"{players[i].NickName}");
        }
        // テキストの更新.
        for (int i = 0; i < _nameText.Length; i++)
        {
            _nameText[i].text = _builder[i].ToString();
        }
    }
    // 名前が被った時に自動で後に入ったほうに数字を付ける
    private void NameCheck(Photon.Realtime.Player[] player)
    {
        if (player.Length >= _roomMaxPlayer)
        {
            if (player[0].NickName == player[1].NickName)
            {
                _nameCoveringNum++;
                player[1].NickName = player[1].NickName + _nameCoveringNum.ToString();
            }
        }
    }
}
