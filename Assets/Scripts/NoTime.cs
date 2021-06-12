using UnityEngine;

public class NoTime : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 0;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
