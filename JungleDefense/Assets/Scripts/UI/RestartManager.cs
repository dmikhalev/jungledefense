using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    [SerializeField] private GameObject restartButton;

    private void Awake()
    {
        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }
    }

    public void ShowRestart()
    {
        if (restartButton != null)
        {
            restartButton.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
