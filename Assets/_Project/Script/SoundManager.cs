using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.TeamBattle;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundBGM
{
    NONE = -1,
    snd_bgm_splash_001,
}

public enum SoundFX
{
    //Common 0~ 1000
    NONE = -1,
    UnknownSound = 0,
    roulette_spinclick = 100,
    sfx_click,
    sfx_pinsound,
    sfx_result,
    sfx_roulettecheck,
}


public class SoundManager : CookApps.Playgrounds.Utility.SingletonMonoBehaviour<SoundManager>
{
    /////////////////////////////////////////////////////////////
    // public

    public bool IsReady => this.isReady;

    public float BGMVolume { get; set; } = 1.0f;
    public float SFXVolume { get; set; } = 1.0f;

    public bool IsPlayingGacha { get; set; } = false;

    [SerializeField] private AudioMixer _mixer;

    public ClockStone.AudioObject PlayBGM(SoundBGM bgm)
    {
        return this.PlayBGM(bgm.ToString());
    }

    public ClockStone.AudioObject PlaySFX(SoundFX sfx, bool forceInSilence = false)
    {
        if (forceInSilence)
            return this.PlaySFXWithoutSilence(sfx.ToString());
        else
            return this.PlaySFX(sfx.ToString());
    }

    public ClockStone.AudioObject PlaySFX(string sfxString, bool forceInSilence = false)
    {
        if (forceInSilence)
            return this.PlaySFXWithoutSilence(sfxString);
        else
            return this.PlaySFX(sfxString);
    }

    //public ClockStone.AudioObject PlayVOX(SoundVOX vox, bool forceInSilence = false)
    //{
    //    if (forceInSilence)
    //        return this.PlaySFXWithoutSilence(vox.ToString());
    //    else
    //        return this.PlaySFX(vox.ToString());
    //}

    public ClockStone.AudioObject PlayAMB(SoundFX amb, bool forceInSilence = false)
    {
        if (forceInSilence)
            return this.PlayAMB(amb.ToString());
        else
            return this.PlayAMB(amb.ToString());
    }

    public ClockStone.AudioObject PlayVOX(string voxString, bool forceInSilence = false)
    {
        if (forceInSilence)
            return this.PlaySFXWithoutSilence(voxString);
        else
            return this.PlaySFX(voxString);
    }

    public bool StopSFX(SoundFX sfx)
    {
        return this.StopSFX(sfx.ToString());
    }


    public bool StopBGM()
    {
        return AudioController.StopMusic();
    }

    public bool StopBGM(float fadeOut)
    {
        return AudioController.StopMusic(fadeOut);
    }

    public bool StopAMB()
    {
        return AudioController.StopAmbienceSound();
    }

    public bool StopVOX(string audioID)
    {
        if (!this.isReady) return false;

        return AudioController.Stop(audioID);
    }

    public void Silence(bool isSilence)
    {
        this.isSilence = isSilence;
    }

    public void PauseSFX()
    {
        this.onSFX = false;
    }
    // public void UnPauseSFX()
    // {
    //     this.onSFX = Preference.LoadPreference(Pref.SFX_V, true);
    // }
    //
    // public void PauseBGM()
    // {
    //     if (Preference.LoadPreference(Pref.BGM_V, 1f) > 0f)
    //     {
    //         int volume = Convert.ToInt32(-80f + 0.01f * 80f);
    //         _mixer.SetFloat("BGM", volume);
    //         _mixer.SetFloat("AMB", volume); // AMB(환경음) 볼륨 제어를 BGM에 포함
    //     }
    // }

    // public void UnPauseBGM()
    // {
    //     if (this.isReady)
    //     {
    //         _mixer.SetFloat("BGM", Convert.ToInt32(-80f + Preference.LoadPreference(Pref.BGM_V, 1f) * 80f));
    //         _mixer.SetFloat("AMB", Convert.ToInt32(-80f + Preference.LoadPreference(Pref.BGM_V, 1f) * 80f)); // AMB(환경음) 볼륨 제어를 BGM에 포함
    //     }
    //     //AudioController.SetCategoryVolume("BGM", Preference.LoadPreference(Pref.BGM_V, 0.8f));
    // }
    //
    // public void PauseVOX()
    // {
    //     if (Preference.LoadPreference(Pref.VOX_V, 1f) > 0f)
    //     {
    //         int volume = Convert.ToInt32(-80f + 0.01f * 80f);
    //         _mixer.SetFloat("VOX", volume);
    //     }
    // }

    // public void UnPauseVOX()
    // {
    //     if (this.isReady)
    //         _mixer.SetFloat("VOX", Convert.ToInt32(-80f + Preference.LoadPreference(Pref.VOX_V, 1f) * 80f));
    //     //AudioController.SetCategoryVolume("BGM", Preference.LoadPreference(Pref.BGM_V, 0.8f));
    // }
    //
    // public void PauseVOXUI()
    // {
    //     if (Preference.LoadPreference(Pref.VOX_V, 1f) > 0f)
    //     {
    //         int volume = Convert.ToInt32(-80f + 0.01f * 80f);
    //         _mixer.SetFloat("VOX_UI", volume);
    //     }
    // }

    // public void UnPauseVOXUI()
    // {
    //     if (this.isReady)
    //         _mixer.SetFloat("VOX_UI", Convert.ToInt32(-80f + Preference.LoadPreference(Pref.VOX_V, 1f) * 80f));
    //     //AudioController.SetCategoryVolume("BGM", Preference.LoadPreference(Pref.BGM_V, 0.8f));
    // }
    //
    // public void SetBGMVolume(float v)
    // {
    //     BGMVolume = v;
    //
    //     Preference.SavePreference(Pref.BGM_V, BGMVolume);
    //
    //     AudioController.SetCategoryVolume("BGM", BGMVolume);
    //
    //     // int volume = Convert.ToInt32((-80f + v * 80f) * 0.5f);
    //     // if (this.isReady)
    //     //     _mixer.SetFloat("BGM", volume);
    //     // if (v == 0)
    //     //     _mixer.SetFloat("BGM", -80f);
    // }

    // public void SetSFXVolume(float v)
    // {
    //     SFXVolume = v;
    //
    //     Preference.SavePreference(Pref.SFX_V, SFXVolume);
    //
    //     AudioController.SetCategoryVolume("SFX", SFXVolume);
    //
    //     // int volume = Convert.ToInt32((-80f + v * 80f) * 0.5f);
    //     // if (this.isReady)
    //     // {
    //     //     _mixer.SetFloat("SFX", volume);
    //     //     _mixer.SetFloat("AMB", volume);
    //     // }
    //     //
    //     // if (v == 0)
    //     // {
    //     //     _mixer.SetFloat("SFX", -80f);
    //     //     _mixer.SetFloat("AMB", -80f);
    //     // }
    // }

    public void SetVOXVolume(float v)
    {
        int volume = Convert.ToInt32((-80f + v * 80f) * 0.5f);
        if (this.isReady)
            _mixer.SetFloat("VOX", volume);
        if (v == 0)
            _mixer.SetFloat("VOX", -80f);
    }

    public void SetVOXUIVolume(float v)
    {
        int volume = Convert.ToInt32((-80f + v * 80f) * 0.5f);
        if (this.isReady)
            _mixer.SetFloat("VOX_UI", volume);
        if (v == 0)
            _mixer.SetFloat("VOX_UI", -80f);
    }

    public void SetAMBVolume(float v)
    {
        int volume = Convert.ToInt32((-80f + v * 80f) * 0.5f);
        if (this.isReady)
            _mixer.SetFloat("AMB", volume);
        if (v == 0)
            _mixer.SetFloat("AMB", -80f);
    }

    // public void Initialize()
    // {
    //     this.isReady = true;
    //     this.UpdateOption();
    // }

    /////////////////////////////////////////////////////////////
    // Common Use

    public void PlayButtonClick()
    {
        this.PlaySFX(SoundFX.sfx_click);
    }

    // public void PlayCancel()
    // {
    //     this.PlaySFX(SoundFX.sfx_cancel);
    // }

    // public void UpdateOption()
    // {
    //     // this.onBGM = Preference.LoadPreference(Pref.BGM_V, true);
    //     // this.onSFX = Preference.LoadPreference(Pref.SFX_V, true);
    //     // this.onSFX = Preference.LoadPreference(Pref.VOX_V, true);
    //
    //     BGMVolume = Preference.LoadPreference(Pref.BGM_V, 1.0f);
    //     SFXVolume = Preference.LoadPreference(Pref.SFX_V, 1.0f);
    // }


    public void StopAllSound()
    {
        AudioController.StopAll();
    }

    protected void Start()
    {
        // Run.Wait(() => { return AssetBundleManager.Instance.ReadyToStart; }, () =>
        // {
        //     AssetBundleManager.Instance.LoadAsset(AssetBundleType.SOUND, (result) =>
        //     {
        //         if (result != null)
        //         {
        //             GameObject soundManagerPrefab = AssetBundleManager.Instance.LoadObjectFromBundle(AssetBundleType.SOUND, "AudioController");
        //             GameObject bossMonsterObj = Instantiate(soundManagerPrefab, Vector3.zero, Quaternion.identity);
        //             bossMonsterObj.transform.SetParent(this.transform);
        //             this.isReady = true;
        //         }
        //     });
        // });\
        this.isReady = true;
    }

    /////////////////////////////////////////////////////////////
    // private

    private bool isSilence = false;

    private bool onBGM = true;
    private bool onSFX = true;
    private bool onAMB = true;

    private bool isReady = false;

    public ClockStone.AudioObject PlayBGM(string audioID)
    {
        if (!this.isReady) return null;

        if (!this.onBGM)
            return null;

        ClockStone.AudioObject currentAudioObj = AudioController.GetCurrentMusic();
        if (currentAudioObj != null && currentAudioObj.audioID.Equals(audioID))
            return null;

        AudioController.StopMusic();

        return AudioController.PlayMusic(audioID, BGMVolume);
    }

    private ClockStone.AudioObject PlaySFX(string audioID)
    {
        if (!this.isReady) return null;

        if (!this.onSFX)
            return null;

        if (this.isSilence)
            return AudioController.Play(audioID, 0.2f);
        else
            return AudioController.Play(audioID, SFXVolume);
    }

    private ClockStone.AudioObject PlayAMB(string audioID)
    {
        if (!this.isReady) return null;

        if (!this.onAMB)
            return null;

        ClockStone.AudioObject currentAudioObj = AudioController.GetCurrentAmbienceSound();
        if (currentAudioObj != null && currentAudioObj.audioID.Equals(audioID))
            return null;

        AudioController.StopAmbienceSound();

        return AudioController.PlayAmbienceSound(audioID);
    }

    private ClockStone.AudioObject PlaySFXWithoutSilence(string audioID)
    {
        if (!this.isReady) return null;

        if (!this.onSFX)
            return null;

        Debug.Log("PlaySFXWithoutSilence " + audioID);
        return AudioController.Play(audioID);
    }

    private bool StopSFX(string audioID)
    {
        if (!this.isReady) return false;

        return AudioController.Stop(audioID);
    }
}
