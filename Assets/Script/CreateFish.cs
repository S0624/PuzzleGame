using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFish : MonoBehaviour
{
    // プレハブの取得
    public GameObject _fishPrefab;
    // 魚の最大数
    private int _fishMax = 3;
    public GameObject[] _fish;
    private int _fishCreateTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        _fish = new GameObject[_fishMax];

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void FishUpdate()
    {
        _fishCreateTimer++;
        if (_fishCreateTimer > 60)
        {
            for (int i = 0; i < _fish.Length; i++)
            {
                if (_fish[i] == null)
                {
                    _fish[i] = Instantiate(_fishPrefab);
                    break;
                }
            }
            _fishCreateTimer = 0;
        }
        for (int i = 0; i < _fish.Length; i++)
        {
            if (_fish[i] == null) return;
            if (_fish[i].GetComponent<FishAnimController>().IsDestory())
            {
                Destroy(_fish[i]);
            }
        }
    }

}
