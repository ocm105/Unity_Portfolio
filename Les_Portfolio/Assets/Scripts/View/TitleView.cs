using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UISystem;
using TMPro;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;

public class TitleView : UIView
{
    [SerializeField] GameObject[] mainObjects;
    [SerializeField] GameObject settingObj;
    [SerializeField] Image loadBar;
    [SerializeField] TextMeshProUGUI loadText;
    [SerializeField] Button gpgLoginButton;
    [SerializeField] Button googleAdsButton;
    [SerializeField] Button unityAdsButton;
    [SerializeField] Button localizationButton;
    [SerializeField] Button cameraViewButton;
    [SerializeField] Button playerMoveButton;
    [SerializeField] Button webviewButton;
    [SerializeField] Button gitButton;
    [SerializeField] Button dongleGameButton;

    private MainState mainState;

    public void Show()
    {
        ShowLayer();
    }
    protected override void OnFirstShow()
    {
        googleAdsButton.onClick.AddListener(OnClick_GoogleAds);
        unityAdsButton.onClick.AddListener(OnClick_UnityAds);
        localizationButton.onClick.AddListener(OnClick_Localization);
        cameraViewButton.onClick.AddListener(OnClick_CameraView);
        playerMoveButton.onClick.AddListener(OnClick_PlayerMove);
        webviewButton.onClick.AddListener(OnClick_Webview);
        gitButton.onClick.AddListener(OnClick_Git);
        dongleGameButton.onClick.AddListener(OnClick_DongleGame);
    }

    protected override void OnShow()
    {
        Init();
        PlayGamesPlatform.Activate();
        GPGLogin();
    }

    private void Init()
    {
        CheckFPS.Instance.EnableFPS(30, Color.red);
        LocalizationManager.Instance.ChangeLanguage((int)LanguageType.Korean);

        if (GameDataManager.Instance.isDataLoad_Completed)
            OnChange_MainObject(MainState.Start);
        else
            StartCoroutine(DataLoad());
    }

    #region Load
    // 필요한 데이터 다운
    private IEnumerator DataLoad()
    {
        OnChange_MainObject(MainState.Loading);
        GameDataManager.Instance.GetLocalDatas();

        // Resource Down
        yield return StartCoroutine(AddressableLoad());

        yield return new WaitForSeconds(0.1f);
        // Localization Data Load
        yield return StartCoroutine(LocalizationLoad());

        yield return new WaitForSeconds(0.1f);
        // Game Data Load
        yield return StartCoroutine(GameDataLoad());

        yield return new WaitForSeconds(0.1f);
        OnChange_MainObject(MainState.Start);
    }

    // 리소스 다운로드
    private IEnumerator AddressableLoad()
    {
        loadBar.fillAmount = 0f;

        AddressableManager.Instance.StartDownload_Addressable("All");
        while (!AddressableManager.Instance.isComplete)
        {
            loadBar.fillAmount = AddressableManager.Instance.downPercent;
            loadText.text = $"리소스 다운로드 {Mathf.RoundToInt(loadBar.fillAmount * 100)}%";
            yield return null;
        }
        yield return StartCoroutine(AddressableManager.Instance.LoadData());
        WindowDebug.SuccessLog("AddressableManager Completed");
        loadBar.fillAmount = 1f;
        loadText.text = $"리소스 다운로드 {Mathf.RoundToInt(loadBar.fillAmount * 100)}%";
    }

    // 로컬라이징 다운로드
    private IEnumerator LocalizationLoad()
    {
        loadBar.fillAmount = 0f;

        loadText.text = $"언어팩 다운로드 {Mathf.RoundToInt(loadBar.fillAmount * 100)}%";
        yield return StartCoroutine(LocalizationManager.Instance.LoadData());
        WindowDebug.SuccessLog("LocalizationManager Completed");
        loadBar.fillAmount = 1f;
        loadText.text = $"언어팩 다운로드 {Mathf.RoundToInt(loadBar.fillAmount * 100)}%";
    }

    // 게임 데이터 다운로드
    private IEnumerator GameDataLoad()
    {
        loadBar.fillAmount = 0f;

        loadText.text = $"데이터 다운로드 {Mathf.RoundToInt(loadBar.fillAmount * 100)}%";
        yield return StartCoroutine(GameDataManager.Instance.LoadData());
        WindowDebug.SuccessLog("GameDataManager Completed");
        loadBar.fillAmount = 1f;
        loadText.text = $"데이터 다운로드 {Mathf.RoundToInt(loadBar.fillAmount * 100)}%";
    }
    #endregion

    #region Function
    // 메인 화면 스테이트에 따라 오브젝트 변경
    private void OnChange_MainObject(MainState state)
    {
        mainState = state;
        bool isActive = false;

        for (int i = 0; i < mainObjects.Length; i++)
        {
            isActive = i == (int)state ? true : false;
            mainObjects[i].SetActive(isActive);
        }

        switch (state)
        {
            case MainState.Loading:
                settingObj.SetActive(false);
                break;
            case MainState.Start:
                SoundManager.Instance.PlayMainBGMSound();
                settingObj.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void GPGLogin()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }
    internal void ProcessAuthentication(SignInStatus status)
    {
        switch (status)
        {
            case SignInStatus.Success:
                string name = PlayGamesPlatform.Instance.GetUserDisplayName();
                string userID = PlayGamesPlatform.Instance.GetUserId();

                Debug.Log($"<color=green>로그인 성공 Name : {name} UserID :{userID}</color>");
                break;
            case SignInStatus.InternalError:
                Debug.Log($"<color=red>로그인 실패 InternalError</color>");
                break;
            case SignInStatus.Canceled:
                Debug.Log($"<color=red>로그인 실패 Canceled</color>");
                break;
        }
    }
    #endregion

    #region Event
    // 구글 광고
    private void OnClick_GoogleAds()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        LoadingManager.Instance.SceneLoad(Constants.Scene.GoogleAds);
    }
    // 유니티 광고
    private void OnClick_UnityAds()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        LoadingManager.Instance.SceneLoad(Constants.Scene.UnityAds);
    }
    // 로컬라이징
    private void OnClick_Localization()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        LoadingManager.Instance.SceneLoad(Constants.Scene.Localization);
    }
    // 카메라 뷰 
    private void OnClick_CameraView()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        LoadingManager.Instance.SceneLoad(Constants.Scene.CameraView);
    }
    // 플레이어 조작
    private void OnClick_PlayerMove()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        LoadingManager.Instance.SceneLoad(Constants.Scene.PlayerMove);
    }
    // 웹뷰
    private void OnClick_Webview()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        LoadingManager.Instance.SceneLoad(Constants.Scene.WebView);
    }
    // 깃
    private void OnClick_Git()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        Application.OpenURL("https://github.com/ocm105/Unity_Portfolio.git");
    }
    // 동글이 게임
    private void OnClick_DongleGame()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        LoadingManager.Instance.SceneLoad(Constants.Scene.DongleGame);
    }
    #endregion
}