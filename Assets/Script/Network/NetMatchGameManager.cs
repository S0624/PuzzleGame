using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetMatchGameManager : MonoBehaviourPunCallbacks
{
    private bool _isDecisionButtonPush = false;
    public TextMeshProUGUI[] _text;
    // オブジェクトの取得.
    public ColorSeedCreate _seed;
    public SphereColorManager[] _colorManager;
    private bool _isColorInit = false;
    private Vector2 _testPos = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        ColorSeedInit();
    }
    // Update is called once per frame
    void Update()
    {
        ColorSeedInit();

        if (Input.GetKeyDown("z"))
        {
            if (_isDecisionButtonPush)
            {
                _isDecisionButtonPush = false;
            }
            else
            {
                _isDecisionButtonPush = true;
            }
            Debug.Log("とおってる");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _testPos.x++;
            Debug.Log("やあ");
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _testPos.x--;
            Debug.Log("うん？");
        }

    }
    private void FixedUpdate()
    {
        // リストの情報を取得
        var players = PhotonNetwork.PlayerList;
        PhotonNetwork.LocalPlayer.ButtonDown(_isDecisionButtonPush);
        PhotonNetwork.LocalPlayer.SetSphereCoordinate(_testPos);
        PhotonNetwork.LocalPlayer.CustomUpdate();
        if (photonView.IsMine)
        {
            //foreach (var player in players)
            //{
            //    Debug.Log($"{player.NickName}({player.ActorNumber}) - {player.GetButtonState()}");
            //}
        }
        foreach (var player in players)
        {
            Debug.Log($"{player.NickName}({player.ActorNumber}) - {player.GetButtonState()}{player.GetSphereCoordinate()}");
        }
        // テスト用
        for (int i = 0; i < players.Length; i++)
        {
            _text[i].text = players[i].GetButtonState().ToString();
        }
    }
    /// <summary>
    /// 色の初期化と同期
    /// </summary>
    private void ColorSeedInit()
    {
        if (_isColorInit) return;
        // リストの情報を取得
        var players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == players[0].ActorNumber)
            {
                _seed.InitNetworkColor();
                ColorSeedInin(_seed);
                PhotonNetwork.LocalPlayer.SetCreateSeed(_seed._upSeed, _seed._downSeed);
                Debug.Log("ここにとおってる");
                _isColorInit = true;
                break;
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == players[i].ActorNumber)
            {
                if (_seed.NetworkInitColor(players[i].GetUpDColorSeed(), players[i].GetDownDColorSeed()))
                {
                    Debug.Log("通ってる" + PhotonNetwork.LocalPlayer.ActorNumber);
                    _isColorInit = true;
                    ColorSeedInin(_seed);
                }
            }
        }
    }
    /// <summary>
    /// 色の種の初期化処理
    /// </summary>
    /// <param name="seed"></param>
    private void ColorSeedInin(ColorSeedCreate seed)
    {
        foreach (var col in _colorManager)
        {
            col.SetColorSeed(seed);
            col.InitObjectName();
            col.ColorRandam();
        }
    }
}
