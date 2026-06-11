using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteScreenUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject visualRoot;

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text starsText;

    [Header("Buttons")]
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button levelSelectButton;

    [Header("Optional")]
    [SerializeField] private LevelSelectPanel levelSelectPanel;

    private void Awake()
    {
        WireButtons();
        HideInstant();
    }

    private void OnValidate()
    {
        if (visualRoot == null)
        {
            visualRoot = gameObject;
        }
    }

    public void Show(int levelIndex, int stars, bool hasNextLevel)
    {
        WireButtons();

        if (visualRoot != null)
        {
            visualRoot.SetActive(true);
        }

        if (titleText != null)
        {
            titleText.text = "LEVEL COMPLETE";
        }

        if (levelText != null)
        {
            levelText.text = "LEVEL " + (levelIndex + 1);
        }

        if (starsText != null)
        {
            starsText.text = FormatStars(stars);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(hasNextLevel);
            nextLevelButton.interactable = hasNextLevel;
        }
    }

    public void HideInstant()
    {
        if (visualRoot != null)
        {
            visualRoot.SetActive(false);
        }
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        HideInstant();
        LevelManager.Instance?.LoadNextLevel();
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1f;
        HideInstant();
        LevelManager.Instance?.RestartCurrentLevel();
    }

    public void OpenLevelSelect()
    {
        Time.timeScale = 1f;
        HideInstant();

        if (levelSelectPanel != null)
        {
            levelSelectPanel.Show();
        }
    }

    private void WireButtons()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveListener(LoadNextLevel);
            nextLevelButton.onClick.AddListener(LoadNextLevel);
        }

        if (replayButton != null)
        {
            replayButton.onClick.RemoveListener(ReplayLevel);
            replayButton.onClick.AddListener(ReplayLevel);
        }

        if (levelSelectButton != null)
        {
            levelSelectButton.onClick.RemoveListener(OpenLevelSelect);
            levelSelectButton.onClick.AddListener(OpenLevelSelect);
        }
    }

    private static string FormatStars(int stars)
    {
        stars = Mathf.Clamp(stars, 0, 3);

        return stars switch
        {
            3 => "* * *",
            2 => "* *",
            1 => "*",
            _ => "0"
        };
    }
}
