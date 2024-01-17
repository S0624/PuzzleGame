using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class UIMnager : MonoBehaviourPunCallbacks
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
    // Start is called before the first frame update
    void Start()
    {
        _input = new InputManager();
        _input.Enable();
    }

    // Update is called once per frame
    private void Update()
    {
        if (photonView.IsMine)
        {
            // 対象のキーを押した時の処理.
            if (_input.UI.Submit.WasPerformedThisFrame() || Input.GetKeyDown("z"))
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
                SpeechDubbleUpdate();
            }
        }
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
    // 吹き出しの画像のアップデート処理.
    private void SpeechDubbleUpdate()
    {
        for(int i = 0; i < _speechDubble.Length; i++)
        {
            if (_participationRoom._nameText[i].text == "")
            {
                _speechDubble[i].sprite = _changeSprite[0];
            }
            else
            {
                if(_isDecisionButtonPush)
                {
                    Debug.Log("とおってる？");
                    _speechDubble[0].sprite = _changeSprite[2];
                }
                else
                {
                    Debug.Log("わかりません");
                    _speechDubble[0].sprite = _changeSprite[1];
                }
            }
        }
    }
}