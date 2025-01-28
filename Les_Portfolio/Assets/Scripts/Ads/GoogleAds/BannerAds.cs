using UnityEngine;
using UISystem;
using GoogleMobileAds.Api;

public partial class GoogleAdsView : UIView
{
    private BannerView bannerView;

    // 구글 광고 로드 (배너)
    public void LoadBannerAds()
    {
        bannerAdsBtn.interactable = false;
        LoadingManager.Instance.SetLoading(true);
        CreateBannerAds();
    }
    // 구글 광고 생성 (배너)
    private void CreateBannerAds()
    {
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        adUnitId = "unused";
#endif
        DestroyBannerAds();

        AdSize adSize = AdSize.GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        bannerView = new BannerView(adUnitId, adSize, AdPosition.Top);

        ListenToAdEvents();

        var adRequest = new AdRequest();
        bannerView.LoadAd(adRequest);
    }
    // 구글 광고 삭제 (배너)
    private void DestroyBannerAds()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : " + bannerView.GetResponseInfo());
            LoadingManager.Instance.SetLoading(false);
        };
        // Raised when an ad fails to load into the banner view.
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error);
            LoadingManager.Instance.SetLoading(false);
            bannerAdsBtn.interactable = true;
        };
        // Raised when the ad is estimated to have earned money.
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Banner view paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        // Raised when an impression is recorded for an ad.
        bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
            LoadingManager.Instance.SetLoading(false);
        };
        // Raised when the ad closed full screen content.
        bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
            bannerAdsBtn.interactable = true;
        };

    }
}
