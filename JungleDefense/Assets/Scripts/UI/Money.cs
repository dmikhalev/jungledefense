using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Update()
    {
        if (GameManager.Instance == null || moneyText == null)
        {
            return;
        }

        moneyText.text = $"Money: {GameManager.Instance.money}";
    }
}
