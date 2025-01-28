using UISystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class UnityRewardAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string adUnitId;
    [SerializeField] Button rewardAdsBtn;

    private void Awake()
    {
        rewardAdsBtn.onClick.AddListener(LoadUnityRewardAds);
    }
    // Unity 광고 로드 (보상형)
    public void LoadUnityRewardAds()
    {
        rewardAdsBtn.interactable = false;
        LoadingManager.Instance.SetLoading(true);
        CreateUnityRewardAds();
    }
    // Unity 광고 생성 (보상형)
    public void CreateUnityRewardAds()
    {
#if UNITY_IOS
        adUnitId = "Rewarded_iOS";
#elif UNITY_ANDROID
        adUnitId = "Rewarded_Android";
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
        if (adUnitId.Equals(adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            LoadingManager.Instance.SetLoading(false);
            Les_UIManager.Instance.Popup<BasePopup_Toast>().Open("광고 보상을 받았습니다.");
            rewardAdsBtn.interactable = true;
        }
    }
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        // Use the error details to determine whether to try to load another ad.
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        LoadingManager.Instance.SetLoading(false);
        rewardAdsBtn.interactable = true;
    }
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        // Use the error details to determine whether to try to load another ad.
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        LoadingManager.Instance.SetLoading(false);
        rewardAdsBtn.interactable = true;
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
}
