using UnityEngine;

public class LevelSelectPanel : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private LevelSelectButton[] levelButtons;

    private void OnEnable()
    {
        AudioManager.Instance?.PlayMusic(SoundType.MenuMusic);
        Refresh();
    }

    public void Refresh()
    {
        if (levelManager == null)
        {
            levelManager = LevelManager.Instance;
        }

        if (levelButtons == null)
        {
            return;
        }

        foreach (LevelSelectButton levelButton in levelButtons)
        {
            if (levelButton != null)
            {
                levelButton.Initialize(levelManager);
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        AudioManager.Instance?.PlayMusic(SoundType.MenuMusic);
        gameObject.SetActive(true);
        Refresh();
    }
}
