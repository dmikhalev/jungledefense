using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        EventBus.Subscribe<BossSpawnedEvent>(OnBossSpawned);
        EventBus.Subscribe<BossHealthChangedEvent>(OnBossHealthChanged);
        EventBus.Subscribe<BossRemovedEvent>(OnBossRemoved);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<BossSpawnedEvent>(OnBossSpawned);
        EventBus.Unsubscribe<BossHealthChangedEvent>(OnBossHealthChanged);
        EventBus.Unsubscribe<BossRemovedEvent>(OnBossRemoved);
    }

    private void Start()
    {
        Hide();
    }

    private void OnBossSpawned(BossSpawnedEvent e)
    {
        if (visualRoot != null)
        {
            visualRoot.SetActive(true);
        }

        if (bossNameText != null)
        {
            bossNameText.text = e.BossName;
        }

        SetFill(e.CurrentHealth, e.MaxHealth);
    }

    private void OnBossHealthChanged(BossHealthChangedEvent e)
    {
        SetFill(e.CurrentHealth, e.MaxHealth);
    }

    private void OnBossRemoved(BossRemovedEvent e)
    {
        Hide();
    }

    private void SetFill(float currentHealth, float maxHealth)
    {
        if (fillImage == null)
        {
            return;
        }
        fillImage.fillAmount = maxHealth <= 0f
            ? 0f
            : Mathf.Clamp01(currentHealth / maxHealth);
    }

    private void Hide()
    {
        if (visualRoot != null)
        {
            visualRoot.SetActive(false);
        }
    }
}