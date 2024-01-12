﻿using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 部屋の管理
// IEnumerable<RoomInfo>インターフェースを実装して、foreachでルーム情報を列挙できるようにする
public class RoomDataList : IEnumerable<RoomInfo>
{
    private Dictionary<string, RoomInfo> dictionary = new Dictionary<string, RoomInfo>();

    public void Update(List<RoomInfo> changedRoomList)
    {
        foreach (var info in changedRoomList)
        {
            if (!info.RemovedFromList)
            {
                //dictionary[info.Name] = info;
            }
            else
            {
                dictionary.Remove(info.Name);
            }
        }
    }

    public void Clear()
    {
        dictionary.Clear();
    }

    // 指定したルーム名のルーム情報があれば取得する
    public bool TryGetRoomInfo(string roomName, out RoomInfo roomInfo)
    {
        Debug.Log("OKOKOKO");
        Debug.Log(roomName);
        //roomName = "a";
        return dictionary.TryGetValue(roomName, out roomInfo);
    }

    public IEnumerator<RoomInfo> GetEnumerator()
    {
        foreach (var kvp in dictionary)
        {
            yield return kvp.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
