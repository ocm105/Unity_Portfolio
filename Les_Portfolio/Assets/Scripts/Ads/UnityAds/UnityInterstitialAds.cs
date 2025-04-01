using UISystem;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class UnityInterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string adUnitId;
    [SerializeField] Button interstitialAdsBtn;

    private void Awake()
    {
        interstitialAdsBtn.onClick.AddListener(LoadUnityInterstitialAds);
    }
    // Unity 광고 로드 (전면)
    public void LoadUnityInterstitialAds()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        interstitialAdsBtn.interactable = false;
        LoadingManager.Instance.SetLoading(true);
        CreateUnityInterstitialAds();
    }
    // Unity 광고 생성 (전면)
    public void CreateUnityInterstitialAds()
    {
#if UNITY_IOS
        adUnitId = "Interstitial_iOS";
#elif UNITY_ANDROID
        adUnitId = "Interstitial_Android";
#endif
        Debug.Log("Create Ad: " + adUnitId);
        Advertisement.Load(adUnitId, this);
    }
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
        Advertisement.Show(adUnitId, this);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        interstitialAdsBtn.interactable = true;
        LoadingManager.Instance.SetLoading(false);
    }
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        // Use the error details to determine whether to try to load another ad.
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        LoadingManager.Instance.SetLoading(false);
        interstitialAdsBtn.interactable = true;
    }
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        // Use the error details to determine whether to try to load another ad.
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        LoadingManager.Instance.SetLoading(false);
        interstitialAdsBtn.interactable = true;
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

}
