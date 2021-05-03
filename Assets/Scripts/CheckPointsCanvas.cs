using UnityEngine;
using UnityEngine.UI;

public class CheckPointsCanvas : MonoBehaviour
{
    [SerializeField] public Button[] buttons;

    void Start()
    {
        var saveManager = FindObjectOfType<SaveManager>();
        var checkpointsUnlocked = saveManager.GetSaveData().Checkpoints;

        foreach (var button in buttons)
        {
            if (checkpointsUnlocked.Contains(button.name))
            {
                button.interactable = true;
            }
        }
    }
}
