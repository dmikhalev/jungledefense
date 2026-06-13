using TMPro;
using UnityEngine;

public class CurrentLevelLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (label == null || LevelManager.Instance == null)
        {
            return;
        }

        label.text = $"Lv. {LevelManager.Instance.CurrentLevelNumber}";
    }
}