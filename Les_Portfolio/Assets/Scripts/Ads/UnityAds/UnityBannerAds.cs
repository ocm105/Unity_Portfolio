using UISystem;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class UnityBannerAds : MonoBehaviour
{
    private string adUnitId;
    [SerializeField] Button bannerAdsBtn;

    private void Awake()
    {
        bannerAdsBtn.onClick.AddListener(LoadUnityBannerAds);
    }
    private void Start()
    {
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
    }
    // Unity 광고 로드 (전면)
    public void LoadUnityBannerAds()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        bannerAdsBtn.interactable = false;
        LoadingManager.Instance.SetLoading(true);
        CreateUnityBannerAds();
    }
    // Unity 광고 생성 (전면)
    public void CreateUnityBannerAds()
    {
#if UNITY_IOS
        adUnitId = "Banner_iOS";
#elif UNITY_ANDROID
        adUnitId = "Banner_Android";
#endif
        Debug.Log("Create Ad: " + adUnitId);
        BannerLoadOptions bannerLoadOptions = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
        Advertisement.Banner.Load(adUnitId, bannerLoadOptions);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Ad Loaded: " + adUnitId);
        BannerOptions bannerOptions = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };
        Advertisement.Banner.Show(adUnitId, bannerOptions);
        LoadingManager.Instance.SetLoading(false);
    }
    private void OnBannerError(string message)
    {
        // Optionally execute additional code, such as attempting to load another ad.
        Debug.Log($"Banner Error: {message}");
        LoadingManager.Instance.SetLoading(false);
        bannerAdsBtn.interactable = true;
    }
    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    private void OnBannerClicked() { }
    private void OnBannerShown() { }
    private void OnBannerHidden() { }
}
