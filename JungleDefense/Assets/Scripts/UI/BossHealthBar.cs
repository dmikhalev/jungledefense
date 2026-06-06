using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject root;
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
        if (root != null)
        {
            root.SetActive(true);
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

        if (maxHealth <= 0f)
        {
            fillImage.fillAmount = 0f;
            return;
        }

        fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }

    private void Hide()
    {
        if (root != null)
        {
            root.SetActive(false);
        }
    }
}