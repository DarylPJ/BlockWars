using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Networking;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    const string AndroidGameId = "4115155";
    const string RewardAdId = "Rewarded_Android";
    const string InterstitialAdId = "Interstitial_Android";
    const bool devMode = true;

    private SaveManager saveManager;

    private bool unityAdsInitialized = false;

    private float timeSinceAd = 0;

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

    private void Update()
    {
        timeSinceAd += Time.deltaTime;
    }

    private IEnumerator ManageCallingSetErrorState()
    {
        using var requestInit = UnityWebRequest.Get("http://google.com");
        yield return requestInit.SendWebRequest();

        if (requestInit.result == UnityWebRequest.Result.Success)
        {
            Advertisement.Initialize(AndroidGameId, devMode);
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
                    Advertisement.Initialize(AndroidGameId, devMode);
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

    private bool IsRewardAdReady() => Advertisement.IsReady(RewardAdId);

    public void ShowRewardAd()
    {
        if (!IsRewardAdReady())
        {
            SetErrorState();
            return;
        }

        Advertisement.Show(RewardAdId);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId != RewardAdId) 
        {
            return;
        }

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
        RestartInterstitial();
    }

    public void ShowInterstitial()
    {
        if (timeSinceAd < TimeSpan.FromMinutes(5).TotalSeconds)
        {
            return;
        }     
        
        if (Advertisement.IsReady(InterstitialAdId))
        {
            Advertisement.Show(InterstitialAdId);
        }

        RestartInterstitial();
    }

    private void RestartInterstitial() => timeSinceAd = 0;

    public void OnUnityAdsDidError(string message){ }

    public void OnUnityAdsDidStart(string placementId) { }

    public void OnUnityAdsReady(string placementId) { }

    private void OnDestroy()
    {
        Advertisement.RemoveListener(this);
    }
}
