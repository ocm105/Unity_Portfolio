using UISystem;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationView : UIView
{
    [SerializeField] Joystick joystick;
    [SerializeField] Button chatBtn;
    [SerializeField] Button[] languageBtns;
    [SerializeField] Button homeBtn;

    private PlayerInfo playerInfo;
    private LanguageType languageType;

    public void Show()
    {
        ShowLayer();
    }
    protected override void OnFirstShow()
    {
        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
        chatBtn.onClick.AddListener(OnClick_ChatBtn);
        homeBtn.onClick.AddListener(OnClick_HomeBtn);

        for (int i = 0; i < languageBtns.Length; i++)
        {
            LanguageType type = (LanguageType)i;
            languageBtns[i].onClick.AddListener(() => ChangeLanguage(type));
        }
    }
    protected override void OnShow()
    {
        playerInfo._playerMoveControl.SetJoystick(joystick);
        Init();
    }

    private void Init()
    {
        languageType = LanguageType.Korean;
        LocalizationManager.Instance.ChangeLanguage((int)languageType);
        SetActiveButton(languageType);
    }

    #region Function
    // 언어 버튼 활성화 / 비활성화
    private void SetActiveButton(LanguageType type)
    {
        bool isOn = false;
        for (int i = 0; i < languageBtns.Length; i++)
        {
            isOn = (int)type == i ? false : true;
            languageBtns[i].interactable = isOn;
        }
        languageType = type;
    }
    // 언어 변경
    private void ChangeLanguage(LanguageType type)
    {
        SetActiveButton(type);
        LoadingManager.Instance.SetFadeOut(() =>
        {
            LocalizationManager.Instance.ChangeLanguage((int)languageType);
            LoadingManager.Instance.SetFadeIn();
        });
    }
    #endregion

    #region Event
    private void OnClick_ChatBtn()
    {
        if (playerInfo.npcControl != null)
            Les_UIManager.Instance.Popup<DescriptPopup>().Open(playerInfo.npcControl.npcDialogIndex);
    }
    private void OnClick_HomeBtn()
    {
        LoadingManager.Instance.SceneLoad(Constants.Scene.Title);
    }
    #endregion
}
