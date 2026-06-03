using System.Collections;
using UnityEngine;

public class TowerPlacementFeedback : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 1.15f;
    [SerializeField] private float duration = 0.08f;

    private Vector3 originalScale;
    private Coroutine routine;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void PlaySpawn()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }

        routine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        transform.localScale = originalScale * scaleMultiplier;

        yield return new WaitForSeconds(duration);

        transform.localScale = originalScale;
    }
}