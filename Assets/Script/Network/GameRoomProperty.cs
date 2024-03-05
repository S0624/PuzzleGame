using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

public static class GameRoomProperty
{
    // 時間
    private const string _keyStartTime = "StartTime";
    // ボタン
    private const string _isBotton = "BottonState";

    private static readonly Hashtable propsToSet = new Hashtable();

    // ゲームの開始時刻が設定されていれば取得する
    public static bool TryGetStartTime(this Room room, out int timestamp)
    {
        if (room.CustomProperties[_keyStartTime] is int value)
        {
            timestamp = value;
            return true;
        }
        else
        {
            timestamp = 0;
            return false;
        }
    }

    // ゲームの開始時刻を設定する
    public static void SetStartTime(this Room room, int timestamp)
    {
        propsToSet[_keyStartTime] = timestamp;
        room.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
    // ボタンを取得する
    public static bool GetButtonState(this Player player)
    {
        if (player.CustomProperties[_isBotton] == null) return false;
        return (bool)player.CustomProperties[_isBotton];
    }
    // ボタンを押したのかを取得する
    public static void ButtonDown(this Player player,bool isState)
    {
        //propsToSet[_isBotton] = player.GetButtonState();
        propsToSet[_isBotton] = isState;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}
