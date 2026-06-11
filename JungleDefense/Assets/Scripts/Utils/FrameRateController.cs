using UnityEngine;

public class FrameRateController : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 120;
    }
}