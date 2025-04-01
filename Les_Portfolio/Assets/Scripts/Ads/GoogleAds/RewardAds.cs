using UnityEngine;
using UISystem;
using GoogleMobileAds.Api;
using System.Collections;

public partial class GoogleAdsView : UIView
{
    private RewardedAd rewardedAd;

    // 구글 광고 로드 (보상형)
    public void LoadRewardAds()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        rewardAdsBtn.interactable = false;
        LoadingManager.Instance.SetLoading(true);
        CreateRewardAds();
    }
    // 구글 광고 생성 (보상형)
    private void CreateRewardAds()
    {
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        adUnitId = "unused";
#endif
        DestroyRewardAds();

        var adRequest = new AdRequest();
        RewardedAd.Load(adUnitId, adRequest, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                return;
            }
            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

            rewardedAd = ad;
            RegisterEventHandlers(ad);

            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log($"Rewarded ad rewarded the user. Type: {reward.Type}, amount: {reward.Amount}.");
                    StartCoroutine(RewardCoroutine());
                });
            }
            else
                Debug.LogError("Rewarded ad is not ready yet.");
        });
    }
    private IEnumerator RewardCoroutine()
    {
        yield return null;
        Les_UIManager.Instance.Popup<BasePopup_Toast>().Open("광고 보상을 받았습니다.");
    }
    // 구글 광고 삭제 (보상형)
    private void DestroyRewardAds()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
    }
    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
            LoadingManager.Instance.SetLoading(false);
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            rewardAdsBtn.interactable = true;
            //LoadRewardAds();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content with error : " + error);
            LoadingManager.Instance.SetLoading(false);
            rewardAdsBtn.interactable = true;
            //LoadRewardAds();
        };
    }
}
