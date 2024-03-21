using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    // 時間が経過したらデモ動画シーンに移行する
    private int _timer = 0;
    private int _limitTimer = 600;
    // ロードシーンの取得
    public LoadSceneManager _loadSceneManager;
    // Start is called before the first frame update
    void Start()
    {
        // Vsync Count を 0にすることにより、FPS を固定できるようになる
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate()
    {
        _timer++;
        if(_timer > _limitTimer)
        {
            _loadSceneManager.TitleChenge();
        }
        
    }
}
