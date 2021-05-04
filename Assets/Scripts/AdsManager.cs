using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    const string AndroidGameId = "4115155";
    const string myVideoPlacement = "Rewarded_Android";

    private SaveManager saveManager;

    private void Awake()
    {
        var adsManagers = FindObjectsOfType<AdsManager>();

        if (adsManagers.Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        Advertisement.AddListener(this);

        // change this to only initialize if internet connection. Check call to google.com 
        Advertisement.Initialize(AndroidGameId, true);
    }

    public bool IsRewardAdReady() => Advertisement.IsReady(myVideoPlacement);

    public void ShowRewardAd()
    {
        if (!IsRewardAdReady())
        { 
            Debug.Log("Ad not ready. Please wait a few seconds and try again.");
            return;
        }

        Advertisement.Show(myVideoPlacement);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Failed:
                Debug.Log("Ad failed for some reason. Please try again");
                break;
            case ShowResult.Skipped:
                Debug.LogError("Ad skipped. Please watch ad to receve lifes.");
                break;
            case ShowResult.Finished:
                HandleRewardAdWatched();
                break;
        }
    }

    private void HandleRewardAdWatched()
    {
        var currentSave = saveManager.GetSaveData();
        currentSave.Lives = 2;
        saveManager.SaveData(currentSave);

        var levelState = FindObjectOfType<LevelState>();
        levelState.RewardAdWatched();
    }
    
    public void OnUnityAdsDidError(string message){ }

    public void OnUnityAdsDidStart(string placementId) { }

    public void OnUnityAdsReady(string placementId) { }

    private void OnDestroy()
    {
        //Advertisement.RemoveListener(this);
    }
}
