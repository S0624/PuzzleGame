using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("サウンドデータ")]
    public AudioClip[] _soundBGMData;
    public AudioClip[] _soundSEData;
    public AudioSource _bgmSource;
    public AudioSource _seSource;
    public static float _bgmVol = 0.5f;
    public static float _seVol = 0.5f;
    //public AudioClip _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        VolumeInit();
        BGMPlay();
    }
    private void VolumeInit()
    {
        SoundBGMVolume(_bgmVol);
        SoundSEVolume(_seVol);
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
        //_bgmSource.clip = _soundBGMData[bgm];
        BGMPlay();
    }
    // SEデータのチェック
    public int BGMDataCheck(SoundBGMData bgmnum)
    {
        for (int i = 0; i < _soundBGMData.Length; i++)
        {
            if (_soundBGMData[i].name == bgmnum.ToString())
            {
                return i;
            }
        }
        return -1;
    }
    public void SoundBGMVolume(float vol)
    {
        _bgmVol = vol;
        _bgmSource.volume = _bgmVol;
    }
    // 音量を返す
    public float BGMVolume()
    {
        return _bgmVol;
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
    // SEサウンドを鳴らす
    public void SEPlay(SoundSEData soundnum)
    {
        SEStop();
        var se = SEDataCheck(soundnum);
        _seSource.clip = _soundSEData[se];
        SEPlay();
    }
    // SEデータのチェック
    public int SEDataCheck(SoundSEData senum)
    {
        for(int i = 0; i < _soundSEData.Length; i++)
        {
            if( _soundSEData[i].name == senum.ToString())
            {
                return i;
            }
        }
        return -1;
    }

    public void SoundSEVolume(float vol)
    {
        _seVol = vol;
        _seSource.volume = vol;
    }
    // 音量を返す
    public float SEVolume()
    {
        return _seVol;
    }
    
}
