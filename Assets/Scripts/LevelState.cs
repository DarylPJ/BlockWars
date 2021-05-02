using TMPro;
using UnityEngine;

public class LevelState : MonoBehaviour
{
    private const float deathScorePenalty = 30;

    private float currentBlocks = 0;
    private float score = 0;
    private float multiplier = 1;
    private float highScore;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject onScreenInfo;
    [SerializeField] private TMP_Text winScreenScore;
    [SerializeField] private TMP_Text mainScreenScore;
    [SerializeField] private TMP_Text highScoreText;

    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite fullStar;

    [SerializeField] private StarInfo star1;
    [SerializeField] private StarInfo star2;
    [SerializeField] private StarInfo star3;


    private void Start()
    {
        currentBlocks = FindObjectsOfType<Block>().Length;        
        CalculateStarValues();
        
        highScore = star3.absoluteValue;
        highScoreText.text = $"Highscore: {highScore}";

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
        score += multiplier;
        multiplier++;
        currentBlocks--;
        mainScreenScore.text = score.ToString();

        if (score > highScore)
        {
            highScoreText.text = $"Highscore: {score}";
        }

        if (currentBlocks == 0)
        {
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        Time.timeScale = 0;
        onScreenInfo.SetActive(false);
        winMenu.SetActive(true);
        winScreenScore.text = score.ToString();

        star1.textField.text = star1.absoluteValue.ToString();
        star2.textField.text = star2.absoluteValue.ToString();
        star3.textField.text = star3.absoluteValue.ToString();

        SetStarImage(star1);
        SetStarImage(star2);
        SetStarImage(star3);
    }

    private void SetStarImage(StarInfo star)
    {
        if (score >= star.absoluteValue)
        {
            star.imageToUpdate.sprite = fullStar;
            return;
        }

        star.imageToUpdate.sprite = emptyStar;
    }

    private void CalculateStarValues()
    {
        if (star1.multiplyBlock != 0)
        {
            star1.absoluteValue = Mathf.RoundToInt(currentBlocks * star1.multiplyBlock);
        }

        if (star2.multiplyBlock != 1)
        {
            star2.absoluteValue = Mathf.RoundToInt(currentBlocks * star2.multiplyBlock);
        }

        if (star3.multiplyBlock != 0)
        {
            star3.absoluteValue = Mathf.RoundToInt(currentBlocks * star3.multiplyBlock);
        }
    }

    public void PaddleHit() => multiplier = 1;

    public void DeathScorePenaltyNeeded() 
    {
        if (score <= deathScorePenalty)
        {
            score = 0;
            return;
        }

        score -= deathScorePenalty;
    }
}
