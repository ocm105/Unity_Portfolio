using UnityEngine;
using UISystem;
using GoogleMobileAds.Api;

public partial class GoogleAdsView : UIView
{
    private NativeOverlayAd nativeOverlayAd;

    // 구글 광고 로드 (네이티브 고급 광고)
    public void LoadNativeOverlayAds()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        nativeOverlayAdsBtn.interactable = false;
        LoadingManager.Instance.SetLoading(true);
        CreateNativeOverlayAds();
    }
    // 구글 광고 생성 (네이티브 고급 광고)
    private void CreateNativeOverlayAds()
    {
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/3986624511";
#else
        adUnitId = "unused";
#endif
        DestroyNativeOverlayAds();

        var adRequest = new AdRequest();
        var options = new NativeAdOptions
        {
            AdChoicesPlacement = AdChoicesPlacement.TopRightCorner,
            MediaAspectRatio = MediaAspectRatio.Any,
        };
        NativeOverlayAd.Load(adUnitId, adRequest, options, (ad, error) =>
        {
            if (error != null)
            {
                Debug.LogError("Native Overlay ad failed to load an ad with error: " + error);
                LoadingManager.Instance.SetLoading(false);
                return;
            }

            if (ad == null)
            {
                Debug.LogError("Unexpected error: Native Overlay ad load event fired with null ad and null error.");
                LoadingManager.Instance.SetLoading(false);
                return;
            }
            Debug.Log("Native Overlay ad loaded with response : " + ad.GetResponseInfo());
            nativeOverlayAd = ad;
            RenderNativeOverlayAds();

            RegisterEventHandlers(ad);

            if (nativeOverlayAd != null)
            {
                nativeOverlayAd.Show();
            }
        });
    }
    // 구글 광고 삭제 (네이티브 고급 광고)
    private void DestroyNativeOverlayAds()
    {
        if (nativeOverlayAd != null)
        {
            nativeOverlayAd.Destroy();
            nativeOverlayAd = null;
        }
    }
    // 구글 광고 숨김 (네이티브 고급 광고)
    private void HideNativeOverlayAds()
    {
        if (nativeOverlayAd != null) nativeOverlayAd.Hide();
    }
    // 구글 광고 디자인 (네이티브 고급 광고)
    private void RenderNativeOverlayAds()
    {
        if (nativeOverlayAd != null)
        {
            Debug.Log("Rendering Native Overlay ad.");

            NativeTemplateStyle style = new NativeTemplateStyle
            {
                TemplateId = NativeTemplateId.Medium,
            };

            nativeOverlayAd.RenderTemplate(style, AdPosition.Bottom);
        }
    }
    private void RegisterEventHandlers(NativeOverlayAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Native Overlay ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Native Overlay ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Native Overlay ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Native Overlay ad full screen content opened.");
            LoadingManager.Instance.SetLoading(false);
            nativeOverlayAdsBtn.interactable = true;
            //LoadNativeOverlayAds():
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Native Overlay ad full screen content closed.");
            nativeOverlayAdsBtn.interactable = true;
            //LoadNativeOverlayAds():
        };
    }
}
