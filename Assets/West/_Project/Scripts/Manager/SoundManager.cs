using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField] AudioSource bgmAudioSource = null;
    [SerializeField] AudioSource sfxAudioSource = null;

    Dictionary<string, AudioClip> bgmClipDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> sfxClipDic = new Dictionary<string, AudioClip>();

    private float bgmVolume = 1.0f;
    public float BGMVolume
    {
        get { return bgmVolume; }
        set
        {
            bgmVolume = value;

            bgmAudioSource.volume = bgmVolume;
            PlayerPrefs.SetFloat("SETTING_BGM_VOLUME", bgmVolume);
        }
    }

    private float sfxVolume = 1.0f;
    public float SFXVolume
    {
        get { return sfxVolume; }
        set
        {
            sfxVolume = value;

            sfxAudioSource.volume = sfxVolume;
            PlayerPrefs.SetFloat("SETTING_SFX_VOLUME", sfxVolume);
        }
    }

    private void Start()
    {
        SetSoundSetting();
        LoadClipFile();
    }

    public void PlayBGM(string bgmSound)
    {
        if (bgmClipDic.ContainsKey(bgmSound))
        {
            PlayBGMSound(bgmClipDic[bgmSound]);
        }
        else
        {
            Debug.LogWarningFormat("Play BGM Error ==> {0}", bgmSound.ToString());
        }
    }

    public void PlayBGMSound(AudioClip audioClip, bool isLoop = true)
    {
        if (bgmAudioSource == null) { return; }

        bgmAudioSource.Stop();
        bgmAudioSource.clip = audioClip;
        bgmAudioSource.loop = isLoop;

        bgmAudioSource.Play();
    }

    public void PlaySFX(string sfxSound)
    {
        if (sfxClipDic.ContainsKey(sfxSound))
        {
            PlaySFXSound(sfxClipDic[sfxSound]);
        }
        else
        {
            Debug.LogWarningFormat("Play SFX Error ==> {0}", sfxSound.ToString());
        }
    }

    public void PlaySFXSound(AudioClip audioClip, bool isLoop = false)
    {
        if (sfxAudioSource == null) { return; }

        sfxAudioSource.PlayOneShot(audioClip);
    }

    public void StopSFXSound()
    {
        if (sfxAudioSource == null) { return; }

        sfxAudioSource.Stop();
    }

    public void MuteAllSound(bool isMute)
    {
        bgmAudioSource.mute = isMute;
        sfxAudioSource.mute = isMute;
    }

    void SetSoundSetting()
    {
        bgmVolume = PlayerPrefs.GetFloat("SETTING_BGM_VOLUME", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("SETTING_SFX_VOLUME", 1.0f);

        bgmAudioSource.volume = bgmVolume;
        sfxAudioSource.volume = sfxVolume;
    }

    void LoadClipFile()
    {
        // BGM 파일로드
        List<AudioClip> bgmClipList = ResourceManager.GetResources<AudioClip>("Sound/BGM");
        for (int i = 0; i < bgmClipList.Count; ++i)
        {
            if (bgmClipDic.ContainsKey(bgmClipList[i].name)) continue;

            bgmClipDic.Add(bgmClipList[i].name, bgmClipList[i]);
        }

        // SFX 파일로드
        List<AudioClip> sfxClipList = ResourceManager.GetResources<AudioClip>("Sound/SFX");
        for (int i = 0; i < sfxClipList.Count; ++i)
        {
            if (sfxClipDic.ContainsKey(sfxClipList[i].name)) continue;

            sfxClipDic.Add(sfxClipList[i].name, sfxClipList[i]);
        }
    }
}
