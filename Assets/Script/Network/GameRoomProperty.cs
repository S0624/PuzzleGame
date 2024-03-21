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
    // スフィアの座標
    private const string _isSpherePos = "Pos";

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
    // カスタムプロパティの更新処理
    // その都度更新するのではなく一度にまとめて更新する
    public static void CustomUpdate(this Player player)
    {
        player.SetCustomProperties(propsToSet);
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
    }
    /// <summary>
    /// 生成した種を記憶させる
    /// </summary>
    /// <param name="player"></param>
    /// <param name="upseed"></param>
    public static void SetCreateSeed(this Player player, int[] upseed, int[] downseed)
    {
        propsToSet[_isUpSeed] = upseed;

        propsToSet[_isDownSeed] = downseed;

    }

    /// <summary>
    /// 生成した種の取得(上)
    /// </summary>
    /// <returns></returns>
    public static int[] GetUpDColorSeed(this Player player)
    {
        var players = PhotonNetwork.PlayerList;
        //if(players[0].CustomProperties[_isUpSeed] == null) return;
        Debug.Log(players[0].CustomProperties[_isUpSeed]);
        return (int[])players[0].CustomProperties[_isUpSeed];
    }
    /// <summary>
    /// 生成した種の取得(下)
    /// </summary>
    /// <returns></returns>
    public static int[] GetDownDColorSeed(this Player player)
    {
        var players = PhotonNetwork.PlayerList;
        return (int[])players[0].CustomProperties[_isDownSeed];
    }
    // 位置を取得する
    public static Vector2 GetSphereCoordinate(this Player player)
    {
        if (player.CustomProperties[_isSpherePos] == null) return Vector2.zero;
        return (Vector2)player.CustomProperties[_isSpherePos];
    }
    // 位置を取得する
    public static void SetSphereCoordinate(this Player player, Vector2 isState)
    {
        propsToSet[_isSpherePos] = isState;
    }
}

