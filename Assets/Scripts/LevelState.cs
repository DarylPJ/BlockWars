using TMPro;
using UnityEngine;

public class LevelState : MonoBehaviour
{
    private float totalBlocks = 0;
    private float score = 0;
    private float multiplier = 1;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject onScreenInfo;
    [SerializeField] private TMP_Text winScreenScore;

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

    public void BlockDestroyed()
    {
        score += multiplier;
        multiplier++;
        totalBlocks--;

        if (totalBlocks == 0)
        {
            Time.timeScale = 0;
            onScreenInfo.SetActive(false);
            winMenu.SetActive(true);
            winScreenScore.text = score.ToString();
        }
    }

    public void PaddleHit() => multiplier = 1;

    public void AddBlock() =>  totalBlocks++;
}
