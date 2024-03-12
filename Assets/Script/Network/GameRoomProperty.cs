using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public static class GameRoomProperty
{
    // 時間
    private const string _keyStartTime = "Time";
    // ボタン
    private const string _isBotton = "BottonState";
    // 種
    private const string _isUpSeed = "Up";
    private const string _isDownSeed = "Down";

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
        propsToSet[_isBotton] = isState;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
    /// <summary>
    /// 生成した種を記憶させる
    /// </summary>
    /// <param name="player"></param>
    /// <param name="upseed"></param>
    public static void SetCreateSeed(this Player player, int[] upseed, int[] downseed)
    {
        propsToSet[_isUpSeed] = upseed;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();

        propsToSet[_isDownSeed] = downseed;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// 生成した種の取得
    /// </summary>
    /// <returns></returns>
    public static int[] GetUpDColorSeed(this Player player)
    {
        var players = PhotonNetwork.PlayerList;
        //if(players[0].CustomProperties[_isUpSeed] == null) return;
        Debug.Log(players[0].CustomProperties[_isUpSeed]);
        return (int[])players[0].CustomProperties[_isUpSeed];
    }
    public static int[] GetDownDColorSeed(this Player player)
    {
        var players = PhotonNetwork.PlayerList;
        //if(players[0].CustomProperties[_isUpSeed] == null) return;
        Debug.Log(players[0].CustomProperties[_isDownSeed]);
        //var a = (int[])players[0].CustomProperties[_isDownSeed];
        //Debug.Log(a.Length);
        //Debug.Log("nemui" + a[0]);
        return (int[])players[0].CustomProperties[_isDownSeed];
    }
}
