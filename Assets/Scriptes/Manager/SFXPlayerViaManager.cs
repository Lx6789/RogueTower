using UnityEngine;

/// <summary>
/// 挂载在特效预制体上，通过 MusicManager 统一管理音效音量。
/// 循环音效会创建独立 AudioSource，避免多实例冲突；
/// 一次性音效复用 MusicManager 的 PlayOneShot 通道。
/// </summary>
public class SFXPlayerViaManager : MonoBehaviour
{
    [Header("音频剪辑")]
    [SerializeField] private AudioClip clip;

    [Header("播放设置")]
    [SerializeField] private bool isLoop = false;        // 是否循环
    [SerializeField, Range(0f, 1f)] private float volume = 1f; // 本地音量倍数

    private AudioSource privateLoopSource;  // 仅循环音效使用

    /// <summary>
    /// 初始化并立即开始播放
    /// </summary>
    public void InitAndPlay(AudioClip audioClip, bool loop = false)
    {
        this.clip = audioClip;
        this.isLoop = loop;
        Play();
    }

    /// <summary>
    /// 手动播放音效（可在需要时调用）
    /// </summary>
    public void Play()
    {
        if (clip == null)
        {
            Debug.LogWarning($"{gameObject.name}: AudioClip 为空，无法播放");
            return;
        }

        if (isLoop)
        {
            PlayLoop();
        }
        else
        {
            PlayOneShot();
        }
    }

    private void PlayOneShot()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlaySFX(clip);
        }
        else
        {
            Debug.LogError("MusicManager 实例不存在");
        }
    }

    private void PlayLoop()
    {
        // 创建独立的 AudioSource
        if (privateLoopSource == null)
        {
            privateLoopSource = gameObject.AddComponent<AudioSource>();
            privateLoopSource.loop = true;
            privateLoopSource.playOnAwake = false;
        }

        // 从 MusicManager 获取全局音效音量并乘以本地音量
        float globalVolume = MusicManager.Instance != null ? MusicManager.Instance.CurrentSFXVolume : 1f;
        privateLoopSource.volume = globalVolume * volume;
        privateLoopSource.clip = clip;
        privateLoopSource.Play();
    }

    /// <summary>
    /// 停止音效（仅循环音效有效）
    /// </summary>
    public void Stop()
    {
        if (isLoop && privateLoopSource != null && privateLoopSource.isPlaying)
        {
            privateLoopSource.Stop();
        }
    }

    private void OnDestroy()
    {
        Stop();
    }

    // 如果你使用对象池，可在 OnEnable/OnDisable 中处理
    private void OnDisable()
    {
        Stop();
    }
}