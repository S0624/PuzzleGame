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
    // スフィアの色
    private const string _isSphereSetColor = "Color";
    // スフィアの方向
    private const string _isSphereDir = "Dir";
    // スフィアの設置
    private const string _isSphereSet = "Set";
    // スフィアの設置
    private const string _isSphereGet = "Get";
    // スフィアの設置
    private const string _isSet = "IsSet";
    // フィールドの取得
    private const string _fielddata = "Field";
    // 邪魔スフィアの取得
    private const string _isObstacle = "obs";
    // 邪魔スフィアの取得
    private const string _isObstacleTest = "test";

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
    public static void ButtonDown(this Player player, bool isState)
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
    // 位置を参照する
    public static Vector2 IsGetSphereCoordinate(this Player player)
    {
        if (player.CustomProperties[_isSpherePos] == null) return Vector2.zero;
        return (Vector2)player.CustomProperties[_isSpherePos];
    }
    // 位置を取得する
    public static void IsSetSphereCoordinate(this Player player, Vector2 isState)
    {
        propsToSet[_isSpherePos] = isState;
    }
    // 色を参照する
    public static int GetSphereColor(this Player player)
    {
        if (player.CustomProperties[_isSphereSetColor] == null) return 0;
        return (int)player.CustomProperties[_isSphereSetColor];
    }
    // 色を取得する
    public static void SetSphereColor(this Player player, int isState)
    {
        propsToSet[_isSphereSetColor] = isState;
    }
    // 方向を参照する
    public static int GetSphereDirection(this Player player)
    {
        if (player.CustomProperties[_isSphereDir] == null) return 0;
        return (int)player.CustomProperties[_isSphereDir];
    }
    // 位置を取得する
    public static void SetSphereDirection(this Player player, int isState)
    {
        propsToSet[_isSphereDir] = isState;
    }
    // 設置したかどうかを参照する
    public static bool GetSphereSet(this Player player)
    {
        if (player.CustomProperties[_isSphereSet] == null) return false;
        return (bool)player.CustomProperties[_isSphereSet];
    }
    // 設置したかどうかを取得する
    public static void SetSphereSet(this Player player, bool isState)
    {
        propsToSet[_isSphereSet] = isState;
    }
    // 設置したかどうかを参照する
    public static bool GetIsSphereSet(this Player player)
    {
        if (player.CustomProperties[_isSphereGet] == null) return false;
        return (bool)player.CustomProperties[_isSphereGet];
    }
    // 設置したかどうかを取得する
    public static void SetIsSphereSet(this Player player, bool isState)
    {
        propsToSet[_isSphereGet] = isState;
    }
    // 設置したかどうかを参照する
    public static bool TestGetSphereSet(this Player player)
    {
        if (player.CustomProperties[_isSet] == null) return false;
        return (bool)player.CustomProperties[_isSet];
    }
    // 設置したかどうかを取得する
    public static void TestSetIsSphereSet(this Player player, bool isState)
    {
        propsToSet[_isSet] = isState;
    }
    /// <summary>
    /// フィールドのデータを記憶させる
    /// </summary>
    /// <param name="player"></param>
    /// <param name="data"></param>
    public static void SetFieldData(this Player player, int[] data)
    {
        propsToSet[_fielddata] = data;

        //        propsToSet[_isObstacleH] = height;

    }
    /// <summary>
    /// おじゃまの取得(横)
    /// </summary>
    /// <returns></returns>
    public static int[] GetFieldData(this Player player, int num)
    {
        var players = PhotonNetwork.PlayerList;
        //if ((int[]) players[0].CustomProperties[_isObstacleW] == null) return null;
        //return (int[])players[0].CustomProperties[_isObstacleW];
        if ((int[])players[num].CustomProperties[_fielddata] == null) return null;
        return (int[])players[num].CustomProperties[_fielddata];
    }
    //    /// <summary>
    //    /// おじゃまのデータを記憶させる
    //    /// </summary>
    //    /// <param name="player"></param>
    //    /// <param name="width"></param>
    //    public static void SetObstacleData(this Player player, int[,] width)
    //    {
    //        propsToSet[_isObstacleW] = width;

    ////        propsToSet[_isObstacleH] = height;

    //    }
    /// <summary>
    ///// おじゃまの取得(横)
    ///// </summary>
    ///// <returns></returns>
    //public static int[,] GetObstaclWidth(this Player player,int test)
    //{
    //    var players = PhotonNetwork.PlayerList;
    //    return (int[,])players[test].CustomProperties[_isObstacleW];
    //}
    /// <summary>
    /// おじゃまのデータを記憶させる
    /// </summary>
    /// <param name="player"></param>
    /// <param name="width"></param>
    public static void SetObstacleData(this Player player, int obs)
    {
        propsToSet[_isObstacle] = obs;
    }
    
    /// <summary>
    /// おじゃまの取得(横)
    /// </summary>
    /// <returns></returns>
    public static int GetObstacle(this Player player)
    {
        if (player.CustomProperties[_isObstacle] == null) return 0;
        return (int)player.CustomProperties[_isObstacle];
    }

    public static void SetObstacleDataTest(this Player player, int obs)
    {
        propsToSet[_isObstacleTest] = obs;
    }

    public static int GetObstacleTotal(this Player player)
    {
        if (player.CustomProperties[_isObstacleTest] == null) return 0;
        return (int)player.CustomProperties[_isObstacleTest];
    }
}

