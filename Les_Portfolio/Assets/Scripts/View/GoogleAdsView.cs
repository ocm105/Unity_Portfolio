using UnityEngine;
using UISystem;
using GoogleMobileAds.Api;
using UnityEngine.UI;

public partial class GoogleAdsView : UIView
{
    private string adUnitId;
    [SerializeField] Button bannerAdsBtn;
    [SerializeField] Button interstitialAdsBtn;
    [SerializeField] Button rewardInterstitialAdsBtn;
    [SerializeField] Button rewardAdsBtn;
    [SerializeField] Button nativeOverlayAdsBtn;
    [SerializeField] Button appOpenAdsBtn;

    [SerializeField] Button exitBtn;

    public void Show()
    {
        ShowLayer();
    }
    protected override void OnFirstShow()
    {
        bannerAdsBtn.onClick.AddListener(LoadBannerAds);
        interstitialAdsBtn.onClick.AddListener(LoadInterstitialAds);
        rewardInterstitialAdsBtn.onClick.AddListener(LoadRewardInterstitialAds);
        rewardAdsBtn.onClick.AddListener(LoadRewardAds);
        nativeOverlayAdsBtn.onClick.AddListener(LoadNativeOverlayAds);
        appOpenAdsBtn.onClick.AddListener(LoadAppOpenAds);

        exitBtn.onClick.AddListener(OnClick_ExitBtn);
    }
    protected override void OnShow()
    {
        SoundManager.Instance.PlayMainBGMSound();
        Init();
    }

    private void Init()
    {
        MobileAds.Initialize(initStatus => { });
    }

    private void OnClick_ExitBtn()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        DestroyBannerAds();
        DestroyInterstitialAds();
        DestroyRewardInterstitialAds();
        DestroyRewardAds();
        DestroyNativeOverlayAds();
        DestroyAppOpenAds();
        LoadingManager.Instance.SceneLoad(Constants.Scene.Title);
    }

}
