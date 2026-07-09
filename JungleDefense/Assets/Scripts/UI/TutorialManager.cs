using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    private const int TutorialLevelIndex = 0;

    [Header("UI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialText;

    private int activeLevelIndex = -1;
    private int tutorialStep = -1;
    private bool active;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Hide();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<TowerPlacedEvent>(OnTowerPlaced);
        EventBus.Subscribe<WaveStartedEvent>(OnWaveStarted);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<TowerPlacedEvent>(OnTowerPlaced);
        EventBus.Unsubscribe<WaveStartedEvent>(OnWaveStarted);
    }

    public void StartTutorialForLevel(int levelIndex)
    {
        activeLevelIndex = levelIndex;

        if (levelIndex != TutorialLevelIndex ||
            SaveManager.Instance == null ||
            SaveManager.Instance.IsTutorialCompleted())
        {
            active = false;
            tutorialStep = -1;
            Hide();
            return;
        }

        active = true;
        ShowStep(0);
    }

    private void OnTowerPlaced(TowerPlacedEvent e)
    {
        if (!active || activeLevelIndex != TutorialLevelIndex || tutorialStep != 0)
        {
            return;
        }

        ShowStep(1);
    }

    private void OnWaveStarted(WaveStartedEvent e)
    {
        if (!active || activeLevelIndex != TutorialLevelIndex || tutorialStep != 1)
        {
            return;
        }

        ShowStep(2);
        CompleteTutorial();
    }

    private void ShowStep(int step)
    {
        tutorialStep = step;

        switch (step)
        {
            case 0:
                SetText("Tap a tower button, then tap a green tile to build your first tower.");
                break;

            case 1:
                SetText("Great! Now press START to begin.");
                break;

            case 2:
                SetText("Good job! Defend the jungle and upgrade towers when you have enough gold.");
                break;

            default:
                Hide();
                break;
        }
    }

    private void SetText(string text)
    {
        if (tutorialText != null)
        {
            tutorialText.text = text;
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }
    }

    private void CompleteTutorial()
    {
        active = false;
        activeLevelIndex = -1;
        tutorialStep = -1;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CompleteTutorial();
        }

        Invoke(nameof(Hide), 2.5f);
    }

    public void Hide()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }
}
