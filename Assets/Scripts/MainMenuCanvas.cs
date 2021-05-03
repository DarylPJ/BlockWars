using TMPro;
using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text playField;
 
    private void Start()
    {
        var saveManager = FindObjectOfType<SaveManager>();
        var currentSave = saveManager.GetSaveData();

        if (currentSave.CurrentLevel != SaveData.StartLevel || currentSave.Lives < 3)
        {
            playField.text = "Continue";
        };
    }
}
