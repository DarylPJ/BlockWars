using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    private SaveManager saveManager;

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
    }

    public void GoToScene(string sceneName) =>
        SceneManager.LoadScene(sceneName);

    public void GoToUnlockedScene() =>
        SceneManager.LoadScene(saveManager.GetSaveData().CurrentLevel);
}
