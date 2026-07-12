using UnityEngine;

public enum FpsMode
{
    Auto = 0,
    Fps60 = 60,
    Fps120 = 120
}

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private const string MusicEnabledKey = "settings_music_enabled";
    private const string SoundEnabledKey = "settings_sound_enabled";
    private const string FpsModeKey = "settings_fps_mode";

    [Header("Defaults")]
    [SerializeField] private bool defaultMusicEnabled = true;
    [SerializeField] private bool defaultSoundEnabled = true;
    [SerializeField] private FpsMode defaultFpsMode = FpsMode.Fps120;

    public bool MusicEnabled { get; private set; }
    public bool SoundEnabled { get; private set; }
    public FpsMode CurrentFpsMode { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
        ApplySettings();
    }

    public void SetMusicEnabled(bool enabled)
    {
        MusicEnabled = enabled;
        PlayerPrefs.SetInt(MusicEnabledKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSoundEnabled(bool enabled)
    {
        SoundEnabled = enabled;
        PlayerPrefs.SetInt(SoundEnabledKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetFpsMode(int dropdownIndex)
    {
        CurrentFpsMode = dropdownIndex switch
        {
            1 => FpsMode.Fps60,
            2 => FpsMode.Fps120,
            _ => FpsMode.Auto
        };

        PlayerPrefs.SetInt(FpsModeKey, (int)CurrentFpsMode);
        PlayerPrefs.Save();

        ApplyFrameRate();
    }

    public void ResetProgress()
    {
        SaveManager.Instance?.DeleteSaveFiles();
    }

    public void ApplySettings()
    {
        ApplyFrameRate();
    }

    private void LoadSettings()
    {
        MusicEnabled = PlayerPrefs.GetInt(
            MusicEnabledKey,
            defaultMusicEnabled ? 1 : 0
        ) == 1;

        SoundEnabled = PlayerPrefs.GetInt(
            SoundEnabledKey,
            defaultSoundEnabled ? 1 : 0
        ) == 1;

        CurrentFpsMode = (FpsMode)PlayerPrefs.GetInt(
            FpsModeKey,
            (int)defaultFpsMode
        );
    }

    private void ApplyFrameRate()
    {
        QualitySettings.vSyncCount = 0;

        switch (CurrentFpsMode)
        {
            case FpsMode.Fps60:
                Application.targetFrameRate = 60;
                break;

            case FpsMode.Fps120:
                Application.targetFrameRate = 120;
                break;

            default:
                Application.targetFrameRate = -1;
                break;
        }
    }

    public int GetFpsDropdownIndex()
    {
        return CurrentFpsMode switch
        {
            FpsMode.Fps60 => 1,
            FpsMode.Fps120 => 2,
            _ => 0
        };
    }
}
