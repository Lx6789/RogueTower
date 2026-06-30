using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("音频源")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;   // 用于一次性音效
    [SerializeField] private AudioSource sfxLoopSource; // 用于持续音效（如电击声）

    [Header("背景音乐")]
    [SerializeField] private AudioClip mainBgmClip;
    [SerializeField] private AudioClip gameBgmClip;

    [Header("音效音量")]
    [Range(0f, 1f)]
    [SerializeField] private float bgmVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    public float CurrentSFXVolume => sfxVolume;
    public float CurrentBGMVolume => bgmVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 自动创建音频源（如果未拖拽）
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        if (sfxLoopSource == null)
        {
            sfxLoopSource = gameObject.AddComponent<AudioSource>();
            sfxLoopSource.playOnAwake = false;
            sfxLoopSource.loop = true;
        }

        if (GameManager.Instance != null && GameManager.Instance.UserData != null)
        {
            bgmVolume = GameManager.Instance.UserData.bgmVolume;
            sfxVolume = GameManager.Instance.UserData.sfxVolume;
        }

        ApplyVolumes();
    }

    // ========== 背景音乐 ==========
    public void PlayMainBGM()
    {
        PlayBGM(mainBgmClip);
    }

    public void PlayGameBGM()
    {
        PlayBGM(gameBgmClip);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // ========== 一次性音效 ==========
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // ========== 持续循环音效 ==========
    public void PlayLoopSFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxLoopSource.clip = clip;
        sfxLoopSource.volume = sfxVolume;
        sfxLoopSource.Play();
    }

    public void StopLoopSFX()
    {
        sfxLoopSource.Stop();
    }

    // ========== 音量控制 ==========
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        GameManager.Instance.saveBGMVolume(bgmVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        GameManager.Instance.saveSFXVolume(sfxVolume);
    }

    private void ApplyVolumes()
    {
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
        sfxLoopSource.volume = sfxVolume;
    }
}