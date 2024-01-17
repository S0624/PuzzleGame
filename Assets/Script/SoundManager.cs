using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("サウンドデータ")]
    public AudioClip[] _soundData;
    private AudioSource _audioSource;
    //public AudioClip _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _soundData[0];
        SoundPlay();
    }

    // Update is called once per frame
    void Update()
    {
    }
    // サウンドを鳴らす
    public void SoundPlay()
    {
        // 再生
        _audioSource.Play();
    }
    // サウンドを止める
    public void SoundStop()
    {
        _audioSource.Stop();
    }
    // サウンドを変更する
    public void SoundChenge(int soundnum)
    {
        SoundStop();
        _audioSource.clip = _soundData[soundnum];
        SoundPlay();
    }
    public void SoundBGMVolume(float vol)
    {
        _audioSource.volume = vol;
    }
}
