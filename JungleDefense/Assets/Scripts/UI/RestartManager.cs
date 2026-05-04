using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    public GameObject restartButton;

    public void ShowRestart()
    {
        restartButton.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // важно вернуть время

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}