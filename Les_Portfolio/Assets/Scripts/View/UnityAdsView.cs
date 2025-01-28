using UISystem;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Advertisements;

public partial class UnityAdsView : UIView, IUnityAdsInitializationListener
{
    private string adUnitId;
    private string androidGameId = "5776700";
    private string iOSGameId = "5776701";
    private string gameId;
    private bool testMode = true;

    [SerializeField] Button exitBtn;

    public void Show()
    {
        ShowLayer();
    }
    protected override void OnFirstShow()
    {
        exitBtn.onClick.AddListener(OnClick_ExitBtn);
        Init();
    }
    protected override void OnShow() { }

    private void Init()
    {
        InitializeAds();
    }

    // Unity Ads 초기화
    public void InitializeAds()
    {
#if UNITY_IOS
        gameId = iOSGameId;
        testMode = false;
#elif UNITY_ANDROID
        gameId = androidGameId;
        testMode = false;
#elif UNITY_EDITOR
        gameId = androidGameId; //Only for testing the functionality in the Editor
        testMode = true;
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    private void OnClick_ExitBtn()
    {
        LoadingManager.Instance.SceneLoad(Constants.Scene.Title);
    }
}
