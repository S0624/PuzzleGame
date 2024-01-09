using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingView : MonoBehaviourPunCallbacks
{
    // プレイヤーネームの取得
    public InputField _playerNameInputField = default;
    // passwordの取得
    public InputField _passwordInputField = default;
    public int _passwordLenght = 6;
    // プレイヤーのデータを管理するフラグ
    private bool _isPass = false;
    private bool _isName = false;
    [SerializeField]
    private Button _joinRoomButton = default;
    // ルームの管理に使用
    private RoomDataList _roomList = new RoomDataList();
    private RoomButton _roomButton = new RoomButton();
    //private List<RoomButton> _roomDataList = new List<RoomButton>();

    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        // マスターサーバーに接続するまでは、入力できないようにする
        _canvasGroup.interactable = false;

        // パスワードを入力する前は、ルーム参加ボタンを押せないようにする
        _joinRoomButton.interactable = false;

        _playerNameInputField.onValueChanged.AddListener(OnPlayerNameInputFieldValueChanged);
        _passwordInputField.onValueChanged.AddListener(OnPasswordInputFieldValueChanged);
        _joinRoomButton.onClick.AddListener(OnJoinRoomButtonClick);
    }

    public override void OnConnectedToMaster()
    {
        // マスターサーバーに接続したら、入力できるようにする
        _canvasGroup.interactable = true;
    }
    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList)
    {
        _roomList.Update(changedRoomList);
        // 全てのルーム参加ボタンの表示を更新する
        //foreach (var roomButton in _roomDataList)
        {
            if (_roomList.TryGetRoomInfo(_roomButton._roomName, out var roomInfo))
            {
                _roomButton.SetPlayerCount(roomInfo.PlayerCount);
            }
            else
            {
                _roomButton.SetPlayerCount(0);
            }
        Debug.Log(roomInfo.PlayerCount);
        }
    }
    // プレイヤーが必要な情報を打ち終わっているかどうか.
    private void OnPlayerNameInputFieldValueChanged(string value)
    {
        // 何かしら名前を入力した時のみ、ルーム参加ボタンを押せるようにする
        if ((value.Length > 0))
        {
            _isName = true;
        }
        else
        {
            _isName = false;
        }
        RoomButtonPush();
    }
    private void OnPasswordInputFieldValueChanged(string value)
    {
        // パスワードを6桁入力した時のみ、ルーム参加ボタンを押せるようにする
        if ((value.Length == _passwordLenght))
        {
            _isPass = true;
        }
        else
        {
            _isPass = false;
        }
        RoomButtonPush();
    }
    // ボタンが押せるかの処理
    private void RoomButtonPush()
    {
        bool isPush = false;
        //foreach (var roomButton in _roomDataList)
        {
            //if (roomButton.IsRoomPeopleMax() && _isName && _isPass)
            if (!_roomButton.IsRoomPeopleMax() &&  _isName && _isPass)
            {
                isPush = true;
            } 
            else
            {
                isPush = false;
            }
        }
        _joinRoomButton.interactable = isPush;
        // 名前を記憶させる
        PhotonNetwork.NickName = _playerNameInputField.text;
    }

    private void OnJoinRoomButtonClick()
    {
        // ルーム参加処理中は、入力できないようにする
        _canvasGroup.interactable = false;

        // ルームを非公開に設定する（新規でルームを作成する場合）
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = false;

        // パスワードと同じ名前のルームに参加する（ルームが存在しなければ作成してから参加する）
        PhotonNetwork.JoinOrCreateRoom(_passwordInputField.text, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        // ルームへの参加が成功したら、UIを非表示にする
        gameObject.SetActive(false);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // ルームへの参加が失敗したら、パスワードを再び入力できるようにする
        _playerNameInputField.text = string.Empty;
        _passwordInputField.text = string.Empty;
        _canvasGroup.interactable = true;
    }
}