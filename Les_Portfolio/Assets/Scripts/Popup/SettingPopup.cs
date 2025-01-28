using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SettingPopup : UIPopup
{
    [SerializeField] GameObject frame;

    #region Sound
    [Header("Sound")]
    [SerializeField] Sprite soundOnSprite;
    [SerializeField] Sprite soundOffSprite;
    [SerializeField] Button bgmButton;
    [SerializeField] Button sfxButton;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    private float bgmVolume, sfxVolume;
    private bool isBgm, isSfx;
    #endregion

    #region View
    [Header("View")]
    [SerializeField] Sprite viewOnSprite;
    [SerializeField] Sprite viewOffSprite;
    [Tooltip("FPSView, QuarterView, ShoulderView")]
    [SerializeField] Button[] viewButtons;
    #endregion

    #region Language
    [Header("Language")]
    [SerializeField] Sprite lggOnSprite;
    [SerializeField] Sprite lggOffSprite;
    [Tooltip("Korean, English")]
    [SerializeField] Button[] lggButtons;
    private LanguageType languageType;
    #endregion

    [SerializeField] Button exitButton;
    private LocalSettingInfo localSettingInfo;

    public PopupState Open()
    {
        ShowLayer();
        return state;
    }

    protected override void OnFirstShow()
    {
        bgmSlider.onValueChanged.AddListener((value) => OnChange_BGM(value));
        sfxSlider.onValueChanged.AddListener((value) => OnChange_SFX(value));
        bgmButton.onClick.AddListener(() => OnClick_BGM(!isBgm));
        sfxButton.onClick.AddListener(() => OnClick_SFX(!isSfx));

        for (int i = 0; i < lggButtons.Length; i++)
        {
            LanguageType type = (LanguageType)i;
            lggButtons[i].onClick.AddListener(() => OnClick_languageBtn(type));
        }

        exitButton.onClick.AddListener(OnClick_Exit);
    }
    protected override void OnShow()
    {
        Init();
        ShowTween();
    }
    private void Init()
    {
        localSettingInfo = LocalSave.GetSettingInfo();
        bgmSlider.value = localSettingInfo.bgmVolume;
        sfxSlider.value = localSettingInfo.sfxVolume;
        isBgm = localSettingInfo.isBgm;
        isSfx = localSettingInfo.isSfx;

        OnClick_languageBtn(localSettingInfo.languageType);
    }

    #region Event
    private void OnChange_BGM(float value)
    {
        bgmVolume = value;

        if (bgmVolume <= 0f)
            bgmButton.image.sprite = soundOffSprite;
        else
            bgmButton.image.sprite = soundOnSprite;
    }
    private void OnChange_SFX(float value)
    {
        sfxVolume = value;

        if (sfxVolume <= 0f)
            sfxButton.image.sprite = soundOffSprite;
        else
            sfxButton.image.sprite = soundOnSprite;
    }
    private void OnClick_BGM(bool isOn)
    {
        isBgm = isOn;
        bgmSlider.value = isSfx ? 1f : 0f;
    }
    private void OnClick_SFX(bool isOn)
    {
        isSfx = isOn;
        sfxSlider.value = isSfx ? 1f : 0f;
    }

    private void OnClick_ViewBtn(CameraViewType type)
    {
        for (int i = 0; i < viewButtons.Length; i++)
        {
            viewButtons[i].image.sprite = i == (int)type ? viewOnSprite : viewOffSprite;
        }
    }

    private void OnClick_languageBtn(LanguageType type)
    {
        for (int i = 0; i < lggButtons.Length; i++)
        {
            lggButtons[i].image.sprite = i == (int)type ? lggOnSprite : lggOffSprite;
        }

        languageType = type;
    }

    private void OnClick_Exit()
    {
        localSettingInfo.bgmVolume = bgmSlider.value;
        localSettingInfo.sfxVolume = sfxSlider.value;
        localSettingInfo.isBgm = isBgm;
        localSettingInfo.isSfx = isSfx;

        localSettingInfo.languageType = languageType;

        CloseTween(() => OnResult(PopupResults.Close));
    }

    protected override void OnResult(PopupResults result)
    {
        if (result == PopupResults.Close)
        {
            state.ResultParam = localSettingInfo;
        }

        base.OnResult(result);
    }
    #endregion

    #region Tween
    private void ShowTween()
    {
        frame.transform.localScale = Vector3.zero;
        frame.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic);
    }
    private void CloseTween(Action call)
    {
        frame.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutCubic).OnComplete(call.Invoke);
    }
    #endregion
}
