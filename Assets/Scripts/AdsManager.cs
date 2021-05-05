using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Networking;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    const string AndroidGameId = "4115155";
    const string myVideoPlacement = "Rewarded_Android";

    private SaveManager saveManager;

    private bool unityAdsInitialized = false;

    private void Awake()
    {
        var adsManagers = FindObjectsOfType<AdsManager>();

        if (adsManagers.Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        StartCoroutine(nameof(CheckInternetConnection));
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Advertisement.AddListener(this);        
        saveManager = FindObjectOfType<SaveManager>();
    }

    private IEnumerator CheckInternetConnection()
    {
        float timeCheck = 2.0f;//will check google.com every two seconds
        float t1;
        float t2;
        while (!unityAdsInitialized)
        {
            using var request = UnityWebRequest.Get("http://google.com");
            yield return request.SendWebRequest();

            t1 = Time.fixedTime;
            if (request.result == UnityWebRequest.Result.Success) 
            {
                Advertisement.Initialize(AndroidGameId, true);
                unityAdsInitialized = true;

                break;//will end the coroutine
            }

            t2 = Time.fixedTime - t1;
            if (t2 < timeCheck)
                yield return new WaitForSeconds(timeCheck - t2);
        }
    }

    public bool IsRewardAdReady() => Advertisement.IsReady(myVideoPlacement);

    public void ShowRewardAd()
    {
        if (!IsRewardAdReady())
        {
            FindObjectOfType<ErrorMessageManager>().UpdateErrorMessage("Could not load video. Check you are online and try again in a few seconds.");
            return;
        }

        Advertisement.Show(myVideoPlacement);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Failed:
                FindObjectOfType<ErrorMessageManager>().UpdateErrorMessage("Loading the video failed. Check you are online and try again in a few seconds.");
                break;
            case ShowResult.Skipped:
                FindObjectOfType<ErrorMessageManager>().UpdateErrorMessage("Video skipped! Watch the whole video to receve 2 lives.");
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
        Advertisement.RemoveListener(this);
    }
}
