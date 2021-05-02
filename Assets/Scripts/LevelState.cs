using UnityEngine;

public class LevelState : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
