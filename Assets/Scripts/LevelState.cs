using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelState : MonoBehaviour
{    
    private float currentBlocks = 0;
    private int blocksDestroyed;
    private int lives;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject onScreenInfo;

    [SerializeField] private TMP_Text mainScreenScore;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private string nextLevel;
    [SerializeField] private bool isCheckpoint;

    private SaveManager saveManager;
    private SceneHandler sceneHandler;

    private void Awake()
    {
        var levelState = FindObjectsOfType<LevelState>();

        if (levelState.Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentBlocks = FindObjectsOfType<Block>().Length;
        saveManager = FindObjectOfType<SaveManager>();
        sceneHandler = FindObjectOfType<SceneHandler>();

        var currentData = saveManager.GetSaveData();

        lives = currentData.Lives;
        blocksDestroyed = currentData.BlocksHit;

        livesText.text = lives.ToString("D2");
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

        if (blocksDestroyed == 1000)
        {
            lives++;
            blocksDestroyed = 0;
        }

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
        data.Lives = lives;
        data.BlocksHit = blocksDestroyed;

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
        lives--;
        livesText.text = lives.ToString("D2");

        if (lives == 0)
        {
            Debug.Log("Dead!");
        }
    }
}
