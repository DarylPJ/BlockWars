using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text playField;
    [SerializeField] private Button checkpointsButton;
 
    private void Start()
    {
        var saveManager = FindObjectOfType<SaveManager>();
        var currentSave = saveManager.GetSaveData();

        if (currentSave.CurrentLevel != SaveData.StartLevel)
        {
            playField.text = "Continue";
        };

        if (currentSave.Checkpoints.Count == 0)
        {
            checkpointsButton.interactable = false;
        }
    }
}
