using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private GameObject resetConfirmationPanel;

    [Header("Controls")]
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private TMP_Dropdown fpsDropdown;

    [Header("Info")]
    [SerializeField] private TMP_Text versionText;

    private bool isRefreshing;
    private bool isVisible;

    private void Awake()
    {
        BindControlEvents();
        HideInstant();
        HideResetConfirmation();
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        UnbindControlEvents();
    }

    public void Show()
    {
        if (isVisible)
        {
            return;
        }

        isVisible = true;
        TutorialManager.Instance?.HideTemporarily();

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        HideResetConfirmation();
        Refresh();
    }

    public void Hide()
    {
        if (!isVisible)
        {
            return;
        }

        SaveControlsToSettings();

        isVisible = false;
        HideResetConfirmation();

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        TutorialManager.Instance?.RestoreIfNeeded();
    }

    public void HideInstant()
    {
        isVisible = false;

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public void OnMusicChanged(bool isOn)
    {
        if (isRefreshing)
        {
            return;
        }

        SettingsManager.Instance?.SetMusicEnabled(isOn);
    }

    public void OnSoundChanged(bool isOn)
    {
        if (isRefreshing)
        {
            return;
        }

        SettingsManager.Instance?.SetSoundEnabled(isOn);
    }

    public void OnFpsDropdownChanged(int index)
    {
        if (isRefreshing)
        {
            return;
        }

        SettingsManager.Instance?.SetFpsMode(index);
    }

    public void OnResetProgressClicked()
    {
        if (resetConfirmationPanel != null)
        {
            resetConfirmationPanel.SetActive(true);
        }
    }

    public void OnCancelResetClicked()
    {
        HideResetConfirmation();
    }

    public void OnConfirmResetClicked()
    {
        HideResetConfirmation();
        SettingsManager.Instance?.ResetProgress();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadLevel(0);
        }

        Hide();
    }

    private void HideResetConfirmation()
    {
        if (resetConfirmationPanel != null)
        {
            resetConfirmationPanel.SetActive(false);
        }
    }

    private void Refresh()
    {
        if (SettingsManager.Instance == null)
        {
            return;
        }

        isRefreshing = true;

        if (musicToggle != null)
        {
            musicToggle.SetIsOnWithoutNotify(SettingsManager.Instance.MusicEnabled);
        }

        if (soundToggle != null)
        {
            soundToggle.SetIsOnWithoutNotify(SettingsManager.Instance.SoundEnabled);
        }

        if (fpsDropdown != null)
        {
            EnsureFpsDropdownOptions();
            fpsDropdown.SetValueWithoutNotify(SettingsManager.Instance.GetFpsDropdownIndex());
            fpsDropdown.RefreshShownValue();
        }

        if (versionText != null)
        {
            versionText.text = $"v{Application.version}";
        }

        isRefreshing = false;
    }

    private void BindControlEvents()
    {
        if (musicToggle != null)
        {
            musicToggle.onValueChanged.AddListener(OnMusicChanged);
        }

        if (soundToggle != null)
        {
            soundToggle.onValueChanged.AddListener(OnSoundChanged);
        }

        if (fpsDropdown != null)
        {
            fpsDropdown.onValueChanged.AddListener(OnFpsDropdownChanged);
        }
    }

    private void UnbindControlEvents()
    {
        if (musicToggle != null)
        {
            musicToggle.onValueChanged.RemoveListener(OnMusicChanged);
        }

        if (soundToggle != null)
        {
            soundToggle.onValueChanged.RemoveListener(OnSoundChanged);
        }

        if (fpsDropdown != null)
        {
            fpsDropdown.onValueChanged.RemoveListener(OnFpsDropdownChanged);
        }
    }

    private void SaveControlsToSettings()
    {
        if (SettingsManager.Instance == null)
        {
            return;
        }

        if (musicToggle != null)
        {
            SettingsManager.Instance.SetMusicEnabled(musicToggle.isOn);
        }

        if (soundToggle != null)
        {
            SettingsManager.Instance.SetSoundEnabled(soundToggle.isOn);
        }

        if (fpsDropdown != null)
        {
            SettingsManager.Instance.SetFpsMode(fpsDropdown.value);
        }
    }

    private void EnsureFpsDropdownOptions()
    {
        if (fpsDropdown == null || fpsDropdown.options.Count >= 3)
        {
            return;
        }

        fpsDropdown.ClearOptions();
        fpsDropdown.options.Add(new TMP_Dropdown.OptionData("Auto"));
        fpsDropdown.options.Add(new TMP_Dropdown.OptionData("60"));
        fpsDropdown.options.Add(new TMP_Dropdown.OptionData("120"));
    }
}
