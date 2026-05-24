using System.Collections;
using UnityEngine;

public class TowerShootFeedback : MonoBehaviour
{
    [SerializeField] private Transform feedbackTarget;
    [SerializeField] private float scaleMultiplier = 1.08f;
    [SerializeField] private float recoilDistance = 0.08f;
    [SerializeField] private float recoilDuration = 0.06f;
    [SerializeField] private float returnDuration = 0.08f;

    private Vector3 originalLocalScale;
    private Vector3 originalLocalPosition;
    private Coroutine routine;

    private Transform Target => feedbackTarget != null ? feedbackTarget : transform;

    private void Awake()
    {
        originalLocalScale = Target.localScale;
        originalLocalPosition = Target.localPosition;
    }

    public void Play()
    {
        Play(Vector3.zero);
    }

    public void Play(Vector3 shotDirection)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }

        routine = StartCoroutine(PlayRoutine(shotDirection));
    }

    private IEnumerator PlayRoutine(Vector3 shotDirection)
    {
        Transform target = Target;

        Vector3 recoilOffset = Vector3.zero;

        if (shotDirection.sqrMagnitude > 0.001f)
        {
            recoilOffset = -shotDirection.normalized * recoilDistance;
        }

        Vector3 recoilPosition = originalLocalPosition + recoilOffset;
        Vector3 recoilScale = originalLocalScale * scaleMultiplier;

        yield return Animate(target, recoilPosition, recoilScale, recoilDuration);
        yield return Animate(target, originalLocalPosition, originalLocalScale, returnDuration);

        target.localPosition = originalLocalPosition;
        target.localScale = originalLocalScale;
        routine = null;
    }

    private IEnumerator Animate(Transform target, Vector3 endPosition, Vector3 endScale, float duration)
    {
        if (duration <= 0f)
        {
            target.localPosition = endPosition;
            target.localScale = endScale;
            yield break;
        }

        Vector3 startPosition = target.localPosition;
        Vector3 startScale = target.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 2f);

            target.localPosition = Vector3.Lerp(startPosition, endPosition, eased);
            target.localScale = Vector3.Lerp(startScale, endScale, eased);

            yield return null;
        }

        target.localPosition = endPosition;
        target.localScale = endScale;
    }
}
