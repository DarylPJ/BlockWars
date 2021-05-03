using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelState : MonoBehaviour
{    
    private float currentBlocks = 0;
    private int blocksDestroyed = 0;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject onScreenInfo;
    [SerializeField] private TMP_Text mainScreenScore;
    [SerializeField] private string nextLevel;
    [SerializeField] private bool isCheckpoint;

    private SaveManager saveManager;
    private SceneHandler sceneHandler;

    private void Start()
    {
        currentBlocks = FindObjectsOfType<Block>().Length;
        saveManager = FindObjectOfType<SaveManager>();
        sceneHandler = FindObjectOfType<SceneHandler>();

        mainScreenScore.text = blocksDestroyed.ToString("D3");
    }

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
        blocksDestroyed++;
        currentBlocks--;
        mainScreenScore.text = blocksDestroyed.ToString("D3");

        if (currentBlocks == 0)
        {
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        var data = saveManager.GetSaveData();
        data.CurrentLevel = nextLevel;

        if (isCheckpoint)
        {
            data.Checkpoints.Add(SceneManager.GetActiveScene().name);
        }

        saveManager.SaveData(data);
        
        Time.timeScale = 0;
        onScreenInfo.SetActive(false);
        winMenu.SetActive(true);        
    }

    public void LoadNextLevel() =>
        sceneHandler.GoToScene(nextLevel);

    public void LooseLife()
    {

    }
}
