using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private GameManager subscribedGameManager;
    private int lastDisplayedMoney = int.MinValue;

    private void OnEnable()
    {
        TrySubscribe();
        Refresh();
    }

    private void Update()
    {
        if (subscribedGameManager == null)
        {
            TrySubscribe();
        }

        Refresh();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void TrySubscribe()
    {
        if (subscribedGameManager != null || GameManager.Instance == null)
        {
            return;
        }

        subscribedGameManager = GameManager.Instance;
        subscribedGameManager.MoneyChanged += UpdateMoney;
    }

    private void Unsubscribe()
    {
        if (subscribedGameManager == null)
        {
            return;
        }

        subscribedGameManager.MoneyChanged -= UpdateMoney;
        subscribedGameManager = null;
    }

    private void Refresh()
    {
        if (GameManager.Instance != null)
        {
            UpdateMoney(GameManager.Instance.money);
        }
    }

    private void UpdateMoney(int value)
    {
        if (moneyText == null || value == lastDisplayedMoney)
        {
            return;
        }

        lastDisplayedMoney = value;
        moneyText.text = $"Money: {value}";
    }
}
