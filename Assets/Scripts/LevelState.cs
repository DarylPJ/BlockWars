using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelState : MonoBehaviour
{    
    private readonly List<string> namesOfBlocksDestroyed = new List<string>();
    
    private float currentBlocks = 0;
    private int totalBlocksDestroyed;
    private int lives;
    private Paddle[] paddles;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject onScreenInfo;
    [SerializeField] private GameObject deathScreen;

    [SerializeField] private TMP_Text mainScreenScore;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private string nextLevel;
    [SerializeField] private bool isCheckpoint;

    private SaveManager saveManager;
    private SceneHandler sceneHandler;

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        sceneHandler = FindObjectOfType<SceneHandler>();
        paddles = FindObjectsOfType<Paddle>();

        var currentData = saveManager.GetSaveData();

        var blocks = FindObjectsOfType<Block>();
        currentBlocks = blocks.Length;

        lives = currentData.Lives;
        totalBlocksDestroyed = currentData.BlocksHit;

        livesText.text = lives.ToString("D2");
        mainScreenScore.text = totalBlocksDestroyed.ToString("D3");

        var destoryedBlocks = currentData.DestroyedBlocks;
        foreach (var block in blocks)
        {
            if (!destoryedBlocks.Contains(block.name))
            {
                continue;
            }

            block.DestoryBlock();
            currentBlocks--;
            namesOfBlocksDestroyed.Add(block.name);
        }

        if (currentBlocks == 0)
        {
            LevelComplete();
        }

        if (lives <= 0)
        {
            ShowDeathScreen();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        TurnOffPaddles();
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        TurnOnPaddles();
        Time.timeScale = 1;
    }

    public void OnDestroy()
    {
        Time.timeScale = 1;
    }

    public void BlockDestroyed(string blockName)
    {
        namesOfBlocksDestroyed.Add(blockName);
        totalBlocksDestroyed++;
        currentBlocks--;

        if (totalBlocksDestroyed >= 1000)
        {
            lives++;
            totalBlocksDestroyed = 0;
        }

        mainScreenScore.text = totalBlocksDestroyed.ToString("D3");

        if (currentBlocks == 0)
        {
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        Time.timeScale = 1;
        var data = saveManager.GetSaveData();
        data.CurrentLevel = nextLevel;
        data.BlocksHit = totalBlocksDestroyed;
        data.DestroyedBlocks = new List<string>();

        if (isCheckpoint)
        {
            data.Checkpoints.Add(SceneManager.GetActiveScene().name);
        }

        saveManager.SaveData(data);

        TurnOffPaddles();
        onScreenInfo.SetActive(false);
        winMenu.SetActive(true);        
    }

    public void LoadNextLevel() =>
        sceneHandler.GoToScene(nextLevel);

    public void LooseLife() 
    {
        lives--;
        livesText.text = lives.ToString("D2");

        var data = saveManager.GetSaveData();
        data.DestroyedBlocks = namesOfBlocksDestroyed;
        data.BlocksHit = totalBlocksDestroyed;
        data.Lives = lives;
        saveManager.SaveData(data);

        if (lives <= 0)
        {
            ShowDeathScreen();
        }
    }

    private void TurnOffPaddles()
    {
        foreach (var paddle in paddles)
        {
            paddle.TurnOffPaddle();
        }
    }

    private void TurnOnPaddles()
    {
        foreach (var paddle in paddles)
        {
            paddle.TurnOnPaddle();
        }
    }


    private void ShowDeathScreen()
    {
        Time.timeScale = 0;
        TurnOffPaddles();
        pauseMenu.SetActive(false);
        deathScreen.SetActive(true);
    }
}
