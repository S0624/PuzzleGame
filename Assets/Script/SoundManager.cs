using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("サウンドデータ")]
    public AudioClip[] _soundBGMData;
    public AudioClip[] _sounSEData;
    public AudioSource _bgmSource;
    public AudioSource _seSource;
    //public AudioClip _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        BGMPlay();
    }

    // Update is called once per frame
    void Update()
    {
    }
    // サウンドを鳴らす
    public void BGMPlay()
    {
        // 再生
        _bgmSource.Play();
    }
    // サウンドを止める
    public void BGMStop()
    {
        _bgmSource.Stop();
    }
    // サウンドを変更する
    public void BGMChenge(int soundnum)
    {
        BGMStop();
        _bgmSource.clip = _soundBGMData[soundnum];
        BGMPlay();
    }
    public void SoundBGMVolume(float vol)
    {
        _bgmSource.volume = vol;
    }

    // SEを鳴らす
    public void SEPlay()
    {
        // 再生
        _seSource.Play();
    }
    // サウンドを止める
    public void SEStop()
    {
        _seSource.Stop();
    }
    // サウンドを変更する
    public void SEChenge(int soundnum)
    {
        SEStop();
        _seSource.clip = _soundBGMData[soundnum];
        SEPlay();
    }
    public void SoundSEVolume(float vol)
    {
        _seSource.volume = vol;
    }
}
