using UISystem;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    private Button settingButton;

    private void Awake()
    {
        settingButton.onClick.AddListener(OnClick_Setting);
    }

    // 셋팅
    private void OnClick_Setting()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        Les_UIManager.Instance.Popup<SettingPopup>().Open();
    }
}
