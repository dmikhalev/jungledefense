using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Library")]
    [SerializeField] private SoundLibrary library;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource worldSource;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.8f;

    [Header("Music")]
    [Min(0f)]
    [SerializeField] private float defaultMusicFadeDuration = 0.5f;

    [Header("Startup")]
    [SerializeField] private bool playMusicOnStart = true;
    [SerializeField] private SoundType startupMusic = SoundType.MenuMusic;

    private readonly Dictionary<SoundType, float> nextAllowedPlayTime = new();
    private readonly Dictionary<SoundType, int> lastClipIndexByType = new();
    private readonly HashSet<SoundType> reportedMissingSounds = new();

    private Coroutine musicFadeRoutine;
    private SoundType currentMusicType = SoundType.None;

    private bool lastMusicEnabled = true;
    private bool lastSoundEnabled = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureSources();
        ApplySettings(force: true);
    }

    private void Start()
    {
        if (playMusicOnStart && startupMusic != SoundType.None)
        {
            PlayMusic(startupMusic);
        }
    }

    private void Update()
    {
        ApplySettings(force: false);
    }

    public void Play(SoundType type)
    {
        if (type == SoundType.None)
        {
            return;
        }

        if (!TryGetEntry(type, out SoundLibrary.SoundEntry entry))
        {
            return;
        }

        if (entry.channel == AudioChannel.Music)
        {
            PlayMusic(type);
            return;
        }

        if (!IsSoundEnabled() || IsOnCooldown(type))
        {
            return;
        }

        AudioClip clip = GetRandomClip(type, entry);

        if (clip == null)
        {
            ReportMissingSound(type, "has no AudioClip assigned");
            return;
        }

        AudioSource source =
            entry.channel == AudioChannel.UI
                ? uiSource
                : worldSource;

        if (source == null)
        {
            ReportMissingSound(type, "has no available AudioSource");
            return;
        }

        RegisterCooldown(type, entry.minimumInterval);

        source.pitch = GetRandomValue(entry.pitchRange, 1f);

        float randomVolume = GetRandomValue(entry.volumeRange, 1f);
        float finalVolume = Mathf.Clamp01(
            entry.volume *
            randomVolume *
            sfxVolume *
            masterVolume
        );

        source.PlayOneShot(clip, finalVolume);
    }

    public void PlayMusic(SoundType type)
    {
        PlayMusic(type, defaultMusicFadeDuration);
    }

    public void PlayMusic(SoundType type, float fadeDuration)
    {
        if (type == SoundType.None ||
            !TryGetEntry(type, out SoundLibrary.SoundEntry entry))
        {
            return;
        }

        AudioClip clip = GetRandomClip(type, entry);

        if (clip == null)
        {
            ReportMissingSound(type, "has no AudioClip assigned");
            return;
        }

        if (musicSource == null)
        {
            ReportMissingSound(type, "has no music AudioSource");
            return;
        }

        if (currentMusicType == type &&
            musicSource.clip == clip &&
            musicSource.isPlaying)
        {
            return;
        }

        StartMusicTransition(type, clip, entry, Mathf.Max(0f, fadeDuration));
    }

    public void StopMusic()
    {
        StopMusic(defaultMusicFadeDuration);
    }

    public void StopMusic(float fadeDuration)
    {
        if (musicSource == null)
        {
            return;
        }

        StopMusicFadeRoutine();
        musicFadeRoutine = StartCoroutine(
            FadeOutAndStopMusic(Mathf.Max(0f, fadeDuration))
        );
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplySettings(force: true);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplySettings(force: true);
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
    }

    private void StartMusicTransition(
        SoundType type,
        AudioClip clip,
        SoundLibrary.SoundEntry entry,
        float fadeDuration)
    {
        StopMusicFadeRoutine();

        musicFadeRoutine = StartCoroutine(
            TransitionMusic(type, clip, entry, fadeDuration)
        );
    }

    private IEnumerator TransitionMusic(
        SoundType type,
        AudioClip clip,
        SoundLibrary.SoundEntry entry,
        float fadeDuration)
    {
        float targetVolume = GetMusicTargetVolume(entry);

        if (musicSource.isPlaying && musicSource.clip != null)
        {
            yield return FadeMusicVolume(0f, fadeDuration);
        }

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = entry.loop;
        musicSource.pitch = GetRandomValue(entry.pitchRange, 1f);
        musicSource.volume = 0f;

        currentMusicType = type;
        musicSource.Play();

        ApplySettings(force: true);

        yield return FadeMusicVolume(targetVolume, fadeDuration);

        musicFadeRoutine = null;
    }

    private IEnumerator FadeOutAndStopMusic(float fadeDuration)
    {
        yield return FadeMusicVolume(0f, fadeDuration);

        musicSource.Stop();
        musicSource.clip = null;
        currentMusicType = SoundType.None;
        musicFadeRoutine = null;
    }

    private IEnumerator FadeMusicVolume(float targetVolume, float duration)
    {
        if (musicSource == null)
        {
            yield break;
        }

        float startVolume = musicSource.volume;

        if (duration <= 0f)
        {
            musicSource.volume = targetVolume;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, progress);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    private void StopMusicFadeRoutine()
    {
        if (musicFadeRoutine == null)
        {
            return;
        }

        StopCoroutine(musicFadeRoutine);
        musicFadeRoutine = null;
    }

    private void EnsureSources()
    {
        musicSource = EnsureSource(musicSource, "MusicSource");
        uiSource = EnsureSource(uiSource, "UISource");
        worldSource = EnsureSource(worldSource, "WorldSource");

        musicSource.playOnAwake = false;
        uiSource.playOnAwake = false;
        worldSource.playOnAwake = false;

        musicSource.spatialBlend = 0f;
        uiSource.spatialBlend = 0f;
        worldSource.spatialBlend = 0f;
    }

    private AudioSource EnsureSource(AudioSource source, string childName)
    {
        if (source != null)
        {
            return source;
        }

        Transform child = transform.Find(childName);

        if (child == null)
        {
            GameObject childObject = new GameObject(childName);
            childObject.transform.SetParent(transform, false);
            child = childObject.transform;
        }

        if (!child.TryGetComponent(out AudioSource createdSource))
        {
            createdSource = child.gameObject.AddComponent<AudioSource>();
        }

        return createdSource;
    }

    private void ApplySettings(bool force)
    {
        bool musicEnabled = IsMusicEnabled();
        bool soundEnabled = IsSoundEnabled();

        if (force || musicEnabled != lastMusicEnabled)
        {
            lastMusicEnabled = musicEnabled;

            if (musicSource != null)
            {
                musicSource.mute = !musicEnabled;
            }
        }

        if (force || soundEnabled != lastSoundEnabled)
        {
            lastSoundEnabled = soundEnabled;

            if (uiSource != null)
            {
                uiSource.mute = !soundEnabled;
            }

            if (worldSource != null)
            {
                worldSource.mute = !soundEnabled;
            }
        }

        if (musicSource != null &&
            currentMusicType != SoundType.None &&
            library != null &&
            library.TryGet(currentMusicType, out SoundLibrary.SoundEntry entry) &&
            musicFadeRoutine == null)
        {
            musicSource.volume = GetMusicTargetVolume(entry);
        }
    }

    private bool TryGetEntry(
        SoundType type,
        out SoundLibrary.SoundEntry entry)
    {
        if (library == null)
        {
            entry = null;
            ReportMissingSound(
                type,
                "cannot play because AudioManager has no SoundLibrary assigned"
            );
            return false;
        }

        if (!library.TryGet(type, out entry))
        {
            ReportMissingSound(type, "is missing from SoundLibrary");
            return false;
        }

        return true;
    }

    private bool IsOnCooldown(SoundType type)
    {
        return nextAllowedPlayTime.TryGetValue(type, out float nextTime) &&
               Time.unscaledTime < nextTime;
    }

    private void RegisterCooldown(SoundType type, float minimumInterval)
    {
        if (minimumInterval <= 0f)
        {
            return;
        }

        nextAllowedPlayTime[type] =
            Time.unscaledTime + minimumInterval;
    }

    private AudioClip GetRandomClip(
        SoundType type,
        SoundLibrary.SoundEntry entry)
    {
        if (entry == null ||
            entry.clips == null ||
            entry.clips.Length == 0)
        {
            return null;
        }

        if (entry.clips.Length == 1)
        {
            lastClipIndexByType[type] = 0;
            return entry.clips[0];
        }

        int previousIndex =
            lastClipIndexByType.TryGetValue(type, out int savedIndex)
                ? savedIndex
                : -1;

        int selectedIndex = Random.Range(0, entry.clips.Length - 1);

        if (previousIndex >= 0 && selectedIndex >= previousIndex)
        {
            selectedIndex++;
        }

        selectedIndex = Mathf.Clamp(
            selectedIndex,
            0,
            entry.clips.Length - 1
        );

        lastClipIndexByType[type] = selectedIndex;
        return entry.clips[selectedIndex];
    }

    private float GetMusicTargetVolume(
        SoundLibrary.SoundEntry entry)
    {
        return Mathf.Clamp01(
            entry.volume *
            musicVolume *
            masterVolume
        );
    }

    private static float GetRandomValue(
        Vector2 range,
        float fallback)
    {
        float min = Mathf.Min(range.x, range.y);
        float max = Mathf.Max(range.x, range.y);

        if (Mathf.Approximately(min, 0f) &&
            Mathf.Approximately(max, 0f))
        {
            return fallback;
        }

        return Random.Range(min, max);
    }

    private void ReportMissingSound(
        SoundType type,
        string reason)
    {
        if (reportedMissingSounds.Contains(type))
        {
            return;
        }

        reportedMissingSounds.Add(type);
        Debug.LogWarning($"Sound {type} {reason}.", this);
    }

    private static bool IsMusicEnabled()
    {
        return SettingsManager.Instance == null ||
               SettingsManager.Instance.MusicEnabled;
    }

    private static bool IsSoundEnabled()
    {
        return SettingsManager.Instance == null ||
               SettingsManager.Instance.SoundEnabled;
    }
}
