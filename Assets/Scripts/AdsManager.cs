using System;
using System.Collections;
using System.Diagnostics;
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

        StartCoroutine(nameof(ManageCallingSetErrorState));
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Advertisement.AddListener(this);        
        saveManager = FindObjectOfType<SaveManager>();
    }

    private IEnumerator ManageCallingSetErrorState()
    {
        using var requestInit = UnityWebRequest.Get("http://google.com");
        yield return requestInit.SendWebRequest();

        if (requestInit.result == UnityWebRequest.Result.Success)
        {
            Advertisement.Initialize(AndroidGameId, true);
            unityAdsInitialized = true;
        }
        SetErrorState();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        while (true)
        {
            if (stopwatch.Elapsed < TimeSpan.FromSeconds(2)) 
            {
                yield return null;
                continue;
            }

            if (!unityAdsInitialized)
            {
                using var request = UnityWebRequest.Get("http://google.com");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Advertisement.Initialize(AndroidGameId, true);
                    unityAdsInitialized = true;
                }
            }

            SetErrorState();
            stopwatch.Restart();
            stopwatch.Start();
            yield return null;
        }
    }

    public void SetErrorState()
    {
        var errorMessageManager = FindObjectOfType<ErrorMessageManager>();
        if (errorMessageManager == null)
        {
            return;
        }

        if (!unityAdsInitialized)
        {
            errorMessageManager.ToggleAdsButton(false);
            errorMessageManager.UpdateErrorMessage(
                "Could not load videos. Check you are online and wait a few seconds. This page will automatically refresh");
            return;
        }

        if (!IsRewardAdReady())
        {
            errorMessageManager.ToggleAdsButton(false);
            errorMessageManager.UpdateErrorMessage(
                "Exceeded the maximum number of videos shown today. Check back tomorrow or start from a checkpoint.");
        }

        errorMessageManager.ToggleAdsButton(true);
        errorMessageManager.UpdateErrorMessage("");
    }

    public bool IsRewardAdReady() => Advertisement.IsReady(myVideoPlacement);

    public void ShowRewardAd()
    {
        if (!IsRewardAdReady())
        {
            SetErrorState();
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
