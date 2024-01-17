using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInUpdate : MonoBehaviourPunCallbacks
{
    public Text[] _nameLabel;
    // Start is called before the first frame update
    void Start()
    {
        //_nameLabel = GetComponent<TextMeshPro>();

    }

    // Update is called once per frame
    void Update()
    {
        foreach (var name in _nameLabel)
        {
            //if (photonView.Owner.NickName == "") break ;
            // プレイヤー名とプレイヤーIDを表示する
            name.text = $"{photonView.Owner.NickName}";
        }
    }
}
