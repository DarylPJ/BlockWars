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
    [SerializeField] private GameObject checkpoint;

    [SerializeField] private TMP_Text currentlevelTxt;
    [SerializeField] private TMP_Text mainScreenScore;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private string nextLevel;
    [SerializeField] private bool isCheckpoint;

    [SerializeField] private bool isDebug;

    private SaveManager saveManager;
    private SceneHandler sceneHandler;
    private AdsManager adsManager;

    private void Start()
    {
        currentlevelTxt.text = $"Level {SceneManager.GetActiveScene().name.Substring(1)}";

        checkpoint.SetActive(false);
        if (isCheckpoint)
        {
            checkpoint.SetActive(true);
        }

        saveManager = FindObjectOfType<SaveManager>();
        sceneHandler = FindObjectOfType<SceneHandler>();
        paddles = FindObjectsOfType<Paddle>();
        adsManager = FindObjectOfType<AdsManager>();

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

        if (isDebug)
        {
            lives = 100;
        }

        if (lives <= 0)
        {
            ShowDeathScreen();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Pause();
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
        Invoke(nameof(AllowStart), Time.deltaTime * 5);
    }

    private void AllowStart()
    {
        FindObjectOfType<Ball>().AllowLaunch(true);
    }

    public void OnDestroy()
    {
        Time.timeScale = 1;
    }

    public void BlockDestroyed(string blockName)
    {
        namesOfBlocksDestroyed.Add(blockName);
        currentBlocks--;
        AddBlockPoint();

        if (currentBlocks == 0)
        {
            LevelComplete();
        }
    }

    public void AddBlockPoint()
    {
        totalBlocksDestroyed++;
        mainScreenScore.text = totalBlocksDestroyed.ToString("D3");

        if (totalBlocksDestroyed >= 1000)
        {
            lives++;
            totalBlocksDestroyed = 0;
            livesText.text = lives.ToString("D2");
        }
    }

    private void LevelComplete()
    {
        Time.timeScale = 0;
        var data = saveManager.GetSaveData();
        data.CurrentLevel = nextLevel;
        data.BlocksHit = totalBlocksDestroyed;
        data.DestroyedBlocks = new List<string>();

        var checkpoint = SceneManager.GetActiveScene().name;
        if (isCheckpoint && !data.Checkpoints.Contains(checkpoint))
        {
            data.Checkpoints.Add(checkpoint);
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
        if (paddles == null)
        {
            return;
        }

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
        FindObjectOfType<Ball>().AllowLaunch(false);
        Time.timeScale = 0;
        TurnOffPaddles();
        pauseMenu.SetActive(false);
        deathScreen.SetActive(true);
        adsManager.SetErrorState();
    }

    public void RewardAdWatched()
    {
        lives = 2;
        livesText.text = lives.ToString("D2");
        deathScreen.SetActive(false);
        Pause();
    }
}
