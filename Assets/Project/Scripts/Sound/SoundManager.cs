using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;

    [Header("SFX Configurations")]
    public UnityEngine.Audio.AudioMixerGroup sfxMixerGroup;
    [Range(0f, 1f)] public float defaultSfxVolume = 1f;
    [Range(-3f, 3f)] public float defaultSfxPitch = 1f;
    [Range(0f, 1f)] public float defaultSfxSpatialBlend = 0f;

    [Header("Background Music")]
    public AudioClip overworldBGM;
    public AudioClip battleBGM;

    [Header("UI Sound Effects")]
    public AudioClip uiClick;
    public AudioClip cardDraw;

    [Header("Battle Sound Effects")]
    public AudioClip dash;
    public AudioClip meleeAttack;
    public AudioClip ultimateAttack;
    public AudioClip hitDamage;
    public AudioClip dieSound;

    [Header("Jingles")]
    public AudioClip winJingle;
    public AudioClip loseJingle;

    [Header("SFX Pooling")]
    public int initialPoolSize = 10;
    private List<AudioSource> sfxPool = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Instance == this && overworldBGM != null)
        {
            PlayBGM(overworldBGM, 0.7f);
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (Instance == this && overworldBGM != null)
        {
            PlayBGM(overworldBGM, 0.7f);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewPoolObject();
        }
    }

    private AudioSource CreateNewPoolObject()
    {
        GameObject go = new GameObject("PooledSFXSource");
        go.transform.SetParent(this.transform);
        AudioSource source = go.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = sfxMixerGroup;
        source.playOnAwake = false;
        source.loop = false;
        source.volume = defaultSfxVolume;
        source.pitch = defaultSfxPitch;
        source.spatialBlend = defaultSfxSpatialBlend;
        sfxPool.Add(source);
        return source;
    }

    private AudioSource GetFreeSFXSource()
    {
        for (int i = 0; i < sfxPool.Count; i++)
        {
            if (sfxPool[i] != null && !sfxPool[i].isPlaying)
            {
                return sfxPool[i];
            }
        }
        return CreateNewPoolObject();
    }

    public void PlayBGM(AudioClip newClip, float fadeDuration = 1f)
    {
        if (newClip == null) return;
        if (bgmSource.clip == newClip) return;

        if (bgmSource.isPlaying)
        {
            bgmSource.DOFade(0f, fadeDuration).OnComplete(() => 
            {
                bgmSource.clip = newClip;
                bgmSource.Play();
                bgmSource.DOFade(0.75f, fadeDuration);
            });
        }
        else
        {
            bgmSource.volume = 0f;
            bgmSource.clip = newClip;
            bgmSource.Play();
            bgmSource.DOFade(0.75f, fadeDuration);
        }
    }

    public void StopBGM(float fadeDuration = 1f)
    {
        bgmSource.DOFade(0f, fadeDuration).OnComplete(() => bgmSource.Stop());
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource source = GetFreeSFXSource();
            if (source != null)
            {
                source.clip = clip;
                source.Play();
            }
        }
    }
}