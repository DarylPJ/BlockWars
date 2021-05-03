using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointsCanvas : MonoBehaviour
{
    [SerializeField] public Button[] buttons;
    [SerializeField] public GameObject warningCanvas;
    [SerializeField] public TMP_Text nextButtonText;
    [SerializeField] public Button nextButton;

    private SceneHandler sceneHandler;
    private SaveManager saveManager;

    private SaveData currentSaveData;

    void Start()
    {
        sceneHandler = FindObjectOfType<SceneHandler>();
        
        saveManager = FindObjectOfType<SaveManager>();
        currentSaveData = saveManager.GetSaveData();

        var checkpointsUnlocked = currentSaveData.Checkpoints;

        foreach (var button in buttons)
        {
            if (checkpointsUnlocked.Contains(button.name))
            {
                button.interactable = true;
            }
        }
    }

    // Level needs to be in format L1.1
    public void LevelSelected(string level)
    {
        gameObject.SetActive(false);
        warningCanvas.SetActive(true);

        nextButtonText.text = $"Level {level.Substring(1)}";

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() => HandleNavigationToScene(level));
    }

    public void HandleNavigationToScene(string level)
    {
        var saveData = new SaveData();
        saveData.Checkpoints = currentSaveData.Checkpoints;
        saveData.CurrentLevel = level;

        saveManager.SaveData(saveData);
        sceneHandler.GoToScene(level);
    }

    public void BackToOverview()
    {
        warningCanvas.SetActive(false);
        gameObject.SetActive(true);
    }
}
