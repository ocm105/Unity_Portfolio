using UnityEngine;
using UISystem;
using GoogleMobileAds.Api;
using System.Collections;

public partial class GoogleAdsView : UIView
{
    private RewardedInterstitialAd rewardedInterstitialAd;

    // 구글 광고 로드 (보상형 전면)
    public void LoadRewardInterstitialAds()
    {
        rewardInterstitialAdsBtn.interactable = false;
        LoadingManager.Instance.SetLoading(true);
        CreateRewardInterstitialAds();
    }
    // 구글 광고 생성 (보상형 전면)
    private void CreateRewardInterstitialAds()
    {
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/6978759866";
#else
        adUnitId = "unused";
#endif
        DestroyRewardInterstitialAds();

        var adRequest = new AdRequest();
        RewardedInterstitialAd.Load(adUnitId, adRequest, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded interstitial ad failed to load an ad with error : " + error);
                return;
            }
            Debug.Log("Rewarded interstitial ad loaded with response : " + ad.GetResponseInfo());

            rewardedInterstitialAd = ad;
            RegisterEventHandlers(ad);

            if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
            {
                rewardedInterstitialAd.Show((Reward reward) =>
                {
                    Debug.Log($"Rewarded Interstitial ad rewarded the user. Type: {reward.Type}, amount: {reward.Amount}.");
                    StartCoroutine(RewardinterstitialCoroutine());
                });
            }
            else
                Debug.LogError("Rewarded interstitial ad is not ready yet.");
        });
    }
    private IEnumerator RewardinterstitialCoroutine()
    {
        yield return new WaitForEndOfFrame();
        Les_UIManager.Instance.Popup<BasePopup_Toast>().Open("광고 보상을 받았습니다.");
    }
    // 구글 광고 삭제 (보상형 전면)
    private void DestroyRewardInterstitialAds()
    {
        if (rewardedInterstitialAd != null)
        {
            rewardedInterstitialAd.Destroy();
            rewardedInterstitialAd = null;
        }
    }
    private void RegisterEventHandlers(RewardedInterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Rewarded interstitial ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content opened.");
            LoadingManager.Instance.SetLoading(false);
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content closed.");
            rewardInterstitialAdsBtn.interactable = true;
            // LoadRewardInterstitialAds();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded interstitial ad failed to open full screen content with error : " + error);
            LoadingManager.Instance.SetLoading(false);
            rewardInterstitialAdsBtn.interactable = true;
            // LoadRewardInterstitialAds();
        };
    }
}
