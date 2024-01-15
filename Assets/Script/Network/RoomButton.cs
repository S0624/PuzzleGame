using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public string _roomName { get; private set; }
    //最大値
    public int _maxPlayer = 2;
    // マックスじゃないかどうかのフラグ
    private bool _isMax = false;
    public void Init(MatchmakingView parentView, int roomId)
    {
        //matchmakingView = parentView;
        _roomName = $"Room{roomId}";

        //button = GetComponent<Button>();
        //button.interactable = false;
        //button.onClick.AddListener(OnButtonClick);
    }

    public void SetPlayerCount(int playerCount)
    {
        // ルームが満員でない時のみ、ルーム参加ボタンを押せるようにする
        if(playerCount < _maxPlayer)
        {
            _isMax = false;
        }
        else
        {
            _isMax = true;
        }
    }
    public bool IsRoomPeopleMax()
    {
        return _isMax;  
    }
}
