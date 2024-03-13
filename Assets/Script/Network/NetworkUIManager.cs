using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Unity.VisualScripting;

public class NetworkUIManager : MonoBehaviourPunCallbacks
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
    // PhotonViewの取得
    public PhotonView _view;
    // NetworkTransitionの取得
    public NetworkTransition _transition;
    // Gageの取得
    public Image _gageImage;
    // Gageに加算する数値
    private float _gageAdd = 0.02f;
    // Gage最大値
    private int _gageMax = 1;
    // Gageがマックスになったフラグ
    private bool _isGagemax = false;
    // Start is called before the first frame update
    void Start()
    {
        _input = new InputManager();
        _input.Enable();
        // Gage初期化
        _gageImage.fillAmount = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        IsPrevButtonUpdate();
        // まだルームに参加していない場合は更新しない
        if (!PhotonNetwork.InRoom) { return; }
        _participationRoom.UpdateLabel();
        // 準備が完了していたらボタンを押せないようにする
        // 対象のキーを押した時の処理.
        if (IsButtonInput())
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
        if (IsReady())
        {
            _transition.PhotonEventOn();
        }

    }
    /// <summary>
    /// ボタンを押せるかどうか
    /// </summary>
    /// <returns>ボタンを押したかのフラグ</returns>
    private bool IsButtonInput()
    {
        // どちらも押し終わっていたら押せないようにする
        if (IsReady()) return false;
        // ボタンを押しました
        if (_input.UI.Submit.WasPerformedThisFrame() || Input.GetKeyDown("z")) return true;
        // 何もしてないのでfalse
        return false;
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
    private void IsPrevButtonUpdate()
    {
        // どちらも押し終わっていたら押せないようにする
        if (IsReady() || !_gageImage) return;
        if (!_isGagemax)
        {
            // ボタンを押しているかどうかの処理
            if (_input.UI.Cancel.IsPressed())
            {
                _gageImage.fillAmount += _gageAdd;
            }
            else
            {
                _gageImage.fillAmount = 0;
            }
        }
        // Gageがマックスになったら前のシーンに戻る
        if(_gageImage.fillAmount >= _gageMax || _isGagemax)
        {
            _isGagemax = true;
            _transition.PrevSceneTransition();
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
            _speechDubble[i].sprite = _changeSprite[(int)NetworkStateImage.PreparationOK];
        }
        else
        {
            _speechDubble[i].sprite = _changeSprite[(int)NetworkStateImage.PreparationNow];
        }

        //foreach (var player in players)
        //{
        //    Debug.Log($"{player.NickName}({player.ActorNumber}) - {player.GetButtonState()}");
        //}
    }
    // 吹き出しの準備Okかどうかの検知
    private bool IsReady()
    {
        for (int i = 0; i < _speechDubble.Length; i++)
        {
            if(_speechDubble[i].sprite != _changeSprite[(int)NetworkStateImage.PreparationOK])
            {
                return false;
            }
        }
        return true;
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