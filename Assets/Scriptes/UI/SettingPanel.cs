using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [Header("背景音乐")]
    public Slider BGMSlider;
    [Header("音效")]
    public Slider SoundEffectSlider;

    private void OnEnable()
    {
        // 从 MusicManager 获取当前音量，同步到 Slider
        if (MusicManager.Instance != null)
        {
            BGMSlider.value = MusicManager.Instance.CurrentBGMVolume;
            SoundEffectSlider.value = MusicManager.Instance.CurrentSFXVolume;
        }

        // 注册 Slider 事件（避免重复注册）
        BGMSlider.onValueChanged.RemoveAllListeners();
        BGMSlider.onValueChanged.AddListener(OnBGMVolumeChanged);

        SoundEffectSlider.onValueChanged.RemoveAllListeners();
        SoundEffectSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnDisable()
    {
        // 移除监听器（良好的内存管理习惯）
        BGMSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
        SoundEffectSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }

    private void OnBGMVolumeChanged(float value)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetBGMVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetSFXVolume(value);
    }
}
