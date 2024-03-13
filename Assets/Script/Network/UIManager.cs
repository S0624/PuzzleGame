using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UIManager : MonoBehaviourPunCallbacks//, IPunObservable
{
    public Image _buttonImg = null;
    // ボタンの処理をするための変数.
    private InputManager _input;
    // 準備OKかどうかのフラグ.
    private bool _isDecisionButtonPush = false;
    // 吹き出しの取得.
    public Image[] _speechDubble;
    // 画像の差し替えに使用.
    public Sprite[] _changeSprite;
    // テキストの取得.
    public ParticipationRoom _participationRoom;

    public PhotonView _view;
    // Start is called before the first frame update
    void Start()
    {
        _input = new InputManager();
        _input.Enable();
    }

    // Update is called once per frame
    private void Update()
    {
        // 対象のキーを押した時の処理.
        if (_input.UI.Submit.WasPerformedThisFrame() || Input.GetKeyDown("z"))
        {
            // ボタンのフラグを変える処理.
            IsButtonFlagUpdate();
            // 色を変える
            if (_isDecisionButtonPush)
            {
                _buttonImg.color = Color.red;
            }
            else
            {
                _buttonImg.color = Color.white;
            }
        }
        PhotonNetwork.LocalPlayer.ButtonDown(_isDecisionButtonPush);
        PhotonNetwork.LocalPlayer.CustomUpdate();
        if (photonView.IsMine)
        {
            PhotonEventOn();
        }

    }
    // ボタンのフラグを変える処理.
    private void IsButtonFlagUpdate()
    {
        if (_isDecisionButtonPush == true)
        {
            _isDecisionButtonPush = false;
        }
        else
        {
            _isDecisionButtonPush = true;
        }
    }
    // テスト実装
    private void PhotonEventOn()
    {
        _view.RPC(nameof(SpeechDubbleUpdate), RpcTarget.All);
    }

    // 吹き出しの画像のアップデート処理.
    [PunRPC]
    private void SpeechDubbleUpdate()
    {
        for (int i = 0; i < _speechDubble.Length; i++)
        {
            // プレイヤーが入室していなかったら
            if (_participationRoom._nameText[i].text == "")
            {
                _speechDubble[i].sprite = _changeSprite[(int)NetworkStateImage.Sleep];
            }
            else
            {
                ChengeImg(i);
            }
        }
        //Debug.Log(_participationRoom.photonView.Owner.NickName);
    }
    // 画像を変更する処理.
    private void ChengeImg(int i)
    {
        // リストの情報を取得
        var players = PhotonNetwork.PlayerList;
        // 部屋の人数に合わせて処理を飛ばす
        if(players.Length < i)
        {
            return;
        }
        // 吹き出しの画像の更新処理
        if (players[i].GetButtonState())
        {
            Debug.Log("とおってる？");
            _speechDubble[i].sprite = _changeSprite[(int)NetworkStateImage.PreparationOK];
        }
        else
        {
            Debug.Log("わかりません");
            _speechDubble[i].sprite = _changeSprite[(int)NetworkStateImage.PreparationNow];
        }

        foreach (var player in players)
        {
            Debug.Log($"{player.NickName}({player.ActorNumber}) - {player.GetButtonState()}");
        }
    }

    //void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        // 自身のアバターのスタミナを送信する
    //        stream.SendNext(_isDecisionButtonPush);
    //    }
    //    else
    //    {
    //        // 他プレイヤーのアバターのスタミナを受信する
    //        _isDecisionButtonPush = (bool)stream.ReceiveNext();
    //    }
    //}
}