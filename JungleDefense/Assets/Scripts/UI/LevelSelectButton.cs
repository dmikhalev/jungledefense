using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text starsLabel;
    [SerializeField] private Button button;
    [SerializeField] private GameObject lockOverlay;

    public int LevelIndex => levelIndex;

    private LevelManager levelManager;

    private void Reset()
    {
        button = GetComponent<Button>();
        label = GetComponentInChildren<TMP_Text>();
    }

    public void Initialize(LevelManager manager)
    {
        levelManager = manager;

        if (button == null)
        {
            button = GetComponent<Button>();
        }

        bool unlocked =
            SaveManager.Instance == null ||
            SaveManager.Instance.IsLevelUnlocked(levelIndex);

        if (label != null)
        {
            label.text = (levelIndex + 1).ToString();
        }

        if (starsLabel != null)
        {
            int stars = SaveManager.Instance != null
                ? SaveManager.Instance.GetStars(levelIndex)
                : 0;

            starsLabel.text = FormatStars(stars);
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = unlocked;

            if (unlocked)
            {
                button.onClick.AddListener(LoadLevel);
            }
        }

        if (lockOverlay != null)
        {
            lockOverlay.SetActive(!unlocked);
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
            _ => ""
        };
    }

    private void LoadLevel()
    {
        if (levelManager == null)
        {
            return;
        }

        levelManager.LoadLevel(levelIndex);

        LevelSelectPanel panel = GetComponentInParent<LevelSelectPanel>();
        if (panel != null)
        {
            panel.gameObject.SetActive(false);
        }
    }
}
