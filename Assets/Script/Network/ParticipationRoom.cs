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
    // 文字の取得のために使用.
    private StringBuilder[] _builder = new StringBuilder[2];
    // テキストの更新のために使用する変数.
    private float _elapsedTime;
    // 名前テキストの取得.
    public TextMeshProUGUI[] _nameText;
    private void Start()
    {
        // 初期化.
        for (int i = 0; i < 2; i++)
        {
            _builder[i] = new StringBuilder();
        }
        _elapsedTime = 0f;
    }

    private void Update()
    {
        UpdateLabel();
    }
    private void UpdateLabel()
    {
        // まだルームに参加していない場合は更新しない
        if (!PhotonNetwork.InRoom) { return; }

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
        for (int i = 0; i < 2; i++)
        {
            // 初期化する.
            _builder[i].Clear();
        }
        // 名前の取得.
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
}
