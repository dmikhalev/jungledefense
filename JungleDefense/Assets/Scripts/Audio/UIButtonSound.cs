using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [SerializeField] private SoundType sound = SoundType.ButtonClick;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Play);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(Play);
        }
    }

    private void Play()
    {
        AudioManager.Instance?.Play(sound);
    }
}
