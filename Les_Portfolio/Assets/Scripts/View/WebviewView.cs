using UISystem;
using UnityEngine;
using UnityEngine.UI;

public class WebviewView : UIView
{
    [SerializeField] Button naverBtn;
    [SerializeField] Button googleBtn;
    [SerializeField] Button youtubeBtn;

    [SerializeField] Button exitBtn;

    private string naverUrl = "https://www.naver.com/";
    private string googleUrl = "https://www.google.com/";
    private string youtubeUrl = "https://www.youtube.com/";

    public void Show()
    {
        ShowLayer();
    }
    protected override void OnFirstShow()
    {
        naverBtn.onClick.AddListener(OnClick_Naver);
        googleBtn.onClick.AddListener(OnClick_Google);
        youtubeBtn.onClick.AddListener(OnClick_Youtube);
        exitBtn.onClick.AddListener(OnClick_ExitBtn);
    }
    protected override void OnShow()
    {
        SoundManager.Instance.PlayMainBGMSound();
    }

    private void OnClick_Naver()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        PopupState popup = Les_UIManager.Instance.Popup<WebviewPopup>().Open();
        popup.OnClose = p => WebviewManager.Instance.CloseWebView();
        WebviewManager.Instance.OpenWebView(naverUrl, 235, 235, 85, 185);
    }
    private void OnClick_Google()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        PopupState popup = Les_UIManager.Instance.Popup<WebviewPopup>().Open();
        popup.OnClose = p => WebviewManager.Instance.CloseWebView();
        WebviewManager.Instance.OpenWebView(googleUrl, 235, 235, 85, 185);
    }
    private void OnClick_Youtube()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        PopupState popup = Les_UIManager.Instance.Popup<WebviewPopup>().Open();
        popup.OnClose = p => WebviewManager.Instance.CloseWebView();
        WebviewManager.Instance.OpenWebView(youtubeUrl, 235, 235, 85, 185);
    }

    private void OnClick_ExitBtn()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        LoadingManager.Instance.SceneLoad(Constants.Scene.Title);
    }
}
