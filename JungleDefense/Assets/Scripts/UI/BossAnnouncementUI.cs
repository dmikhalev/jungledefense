using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BossAnnouncementUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float showDuration = 2f;
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine currentRoutine;

    private void OnEnable()
    {
        EventBus.Subscribe<BossSpawnedEvent>(OnBossSpawned);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<BossSpawnedEvent>(OnBossSpawned);
    }

    private void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void OnBossSpawned(BossSpawnedEvent e)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(ShowAnnouncement(e.BossName));
    }

    private IEnumerator ShowAnnouncement(string bossName)
    {
        titleText.text = "BOSS INCOMING";
        bossNameText.text = bossName;
        subtitleText.text = "has entered the battlefield";

        yield return Fade(0f, 1f);

        yield return new WaitForSecondsRealtime(showDuration);

        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float from, float to)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            canvasGroup.alpha =
                Mathf.Lerp(from, to, time / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = to;
    }
}