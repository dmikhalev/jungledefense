using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private void Update()
    {
        float fps = 1f / Time.unscaledDeltaTime;
        text.text = Mathf.RoundToInt(fps).ToString();
    }
}