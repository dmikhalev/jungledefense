using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BossRewardPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float showDuration = 1.2f;
    [SerializeField] private float fadeOutDuration = 0.35f;

    [Header("Animation")]
    [SerializeField] private float moveUpDistance = 40f;

    private RectTransform rectTransform;
    private Vector2 initialAnchoredPosition;
    private Coroutine currentRoutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            initialAnchoredPosition = rectTransform.anchoredPosition;
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        HideInstant();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
    }

    private void OnEnemyKilled(EnemyKilledEvent e)
    {
        if (e.Enemy == null || !e.Enemy.IsBoss)
        {
            return;
        }

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        int reward = e.Enemy.Data.reward;

        currentRoutine = StartCoroutine(ShowRoutine(reward));
    }

    private IEnumerator ShowRoutine(int reward)
    {
        if (titleText != null)
        {
            titleText.text = "BOSS DEFEATED";
        }

        if (rewardText != null)
        {
            rewardText.text = reward > 0
                ? "+" + reward + " GOLD"
                : "VICTORY BONUS";
        }

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = initialAnchoredPosition;
        }

        yield return Fade(0f, 1f, fadeInDuration, 0f);
        yield return new WaitForSecondsRealtime(showDuration);
        yield return Fade(1f, 0f, fadeOutDuration, moveUpDistance);

        HideInstant();
        currentRoutine = null;
    }

    private IEnumerator Fade(float from, float to, float duration, float moveUp)
    {
        float time = 0f;
        Vector2 startPosition = initialAnchoredPosition;
        Vector2 endPosition = initialAnchoredPosition + Vector2.up * moveUp;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(time / duration);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(from, to, t);
            }

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition =
                    Vector2.Lerp(startPosition, endPosition, t);
            }

            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = to;
        }

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = endPosition;
        }
    }

    private void HideInstant()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}