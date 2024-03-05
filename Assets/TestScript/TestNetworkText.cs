using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestNetworkText : MonoBehaviour
{
    public TextMeshProUGUI[] _text;
    public NetworkTransition _netManager;
    //private int _playerNum 
    // Start is called before the first frame update
    void Start()
    {
        // なまえしゅとく
        for (int i = 0; i < _text.Length; i++)
        {
            _text[i].text = _netManager.PlayerData(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
