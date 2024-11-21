using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : Popup
{
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    public override void Init()
    {
        base.Init();

        _bgmVolumeSlider.value = SoundManager.Instance.BGMVolume;
        _sfxVolumeSlider.value = SoundManager.Instance.SFXVolume;
    }

    public void OnBGMVolumeChanged()
    {
        SoundManager.Instance.BGMVolume = _bgmVolumeSlider.value;
    }

    public void OnSFXVolumeChanged()
    {
        SoundManager.Instance.SFXVolume = _sfxVolumeSlider.value;
    }

    public void OnClickCloseButton()
    {
        PopupManager.ClosePopup<SettingPopup>();
    }
}
