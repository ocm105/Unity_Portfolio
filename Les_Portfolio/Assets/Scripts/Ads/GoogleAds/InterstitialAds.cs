using UnityEngine;
using UISystem;
using GoogleMobileAds.Api;

public partial class GoogleAdsView : UIView
{
    private InterstitialAd interstitialAd;

    // 구글 광고 로드 (전면)
    public void LoadInterstitialAds()
    {
        interstitialAdsBtn.interactable = false;
        LoadingManager.Instance.SetLoading(true);
        CreateInterstitialAds();
    }
    // 구글 광고 생성 (전면)
    private void CreateInterstitialAds()
    {
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        adUnitId = "unused";
#endif
        DestroyInterstitialAds();

        var adRequest = new AdRequest();
        InterstitialAd.Load(adUnitId, adRequest, (ad, error) =>
          {
              if (error != null || ad == null)
              {
                  Debug.LogError("interstitial ad failed to load an ad with error : " + error);
                  LoadingManager.Instance.SetLoading(false);
                  return;
              }

              Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());

              interstitialAd = ad;
              RegisterEventHandlers(ad);

              if (interstitialAd != null && interstitialAd.CanShowAd())
                  interstitialAd.Show();
              else
                  Debug.LogError("Interstitial ad is not ready yet.");
          });
    }
    // 구글 광고 삭제 (전면)
    private void DestroyInterstitialAds()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
    }
    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Interstitial ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
            LoadingManager.Instance.SetLoading(false);
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            interstitialAdsBtn.interactable = true;
            // LoadInterstitialAds();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content with error : " + error);
            LoadingManager.Instance.SetLoading(false);
            interstitialAdsBtn.interactable = true;
            // LoadInterstitialAds();
        };
    }
}
