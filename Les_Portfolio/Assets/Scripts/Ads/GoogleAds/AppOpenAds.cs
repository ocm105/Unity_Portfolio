using UnityEngine;
using UISystem;
using GoogleMobileAds.Api;
using System;


public partial class GoogleAdsView : UIView
{
    private AppOpenAd appOpenAd;
    private DateTime expireTime;    // 앱 오픈 광고 제한 시간

    // 구글 광고 로드 (앱 오픈)
    public void LoadAppOpenAds()
    {
        appOpenAdsBtn.interactable = false;
        LoadingManager.Instance.SetLoading(true);
        CreateAppOpenAds();
    }
    // 구글 광고 생성 (앱 오픈)
    private void CreateAppOpenAds()
    {
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/9257395921";
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/5575463023";
#else
        adUnitId = "unused";
#endif
        DestroyAppOpenAds();

        var adRequest = new AdRequest();
        AppOpenAd.Load(adUnitId, adRequest, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("app open ad failed to load an ad with error : " + error);
                return;
            }
            Debug.Log("App open ad loaded with response : " + ad.GetResponseInfo());

            expireTime = DateTime.Now + TimeSpan.FromHours(4);

            appOpenAd = ad;
            RegisterEventHandlers(ad);

            if (appOpenAd != null && appOpenAd.CanShowAd() && DateTime.Now < expireTime)
            {
                appOpenAd.Show();
            }
            else
                Debug.LogError("App open ad is not ready yet.");
        });

    }
    // 구글 광고 삭제 (앱 오픈)
    private void DestroyAppOpenAds()
    {
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }
    }
    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"App open ad paid {adValue.Value} {adValue.CurrencyCode}.");
            LoadingManager.Instance.SetLoading(false);
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("App open ad full screen content opened.");
            LoadingManager.Instance.SetLoading(false);
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("App open ad full screen content closed.");
            appOpenAdsBtn.interactable = true;
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("App open ad failed to open full screen content with error : " + error);
            LoadingManager.Instance.SetLoading(false);
            appOpenAdsBtn.interactable = true;
        };
    }
}
