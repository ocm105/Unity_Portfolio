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
    #endregion

    #region Application
    [Header("Application")]
    [SerializeField] Button apkExitButton;
    [SerializeField] Button apkExitCancel;
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
        bgmButton.onClick.AddListener(() => OnClick_BGM(!localSettingInfo.isBgm));
        sfxButton.onClick.AddListener(() => OnClick_SFX(!localSettingInfo.isSfx));

        apkExitButton.onClick.AddListener(OnClick_ApplicationExit);
        apkExitCancel.onClick.AddListener(OnClick_Exit);
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
        bgmButton.image.sprite = SetSoundImage(localSettingInfo.isBgm);
        sfxButton.image.sprite = SetSoundImage(localSettingInfo.isSfx);
    }

    private void SetLocalSetting()
    {
        LocalSave.SetSettingInfo(localSettingInfo);
    }

    #region Event
    private Sprite SetSoundImage(bool isOn)
    {
        if (isOn)
            return soundOnSprite;
        else
            return soundOffSprite;
    }
    private void OnChange_BGM(float value)
    {
        localSettingInfo.bgmVolume = value;
        SetLocalSetting();
    }
    private void OnChange_SFX(float value)
    {
        localSettingInfo.sfxVolume = value;
        SetLocalSetting();
    }

    private void OnClick_BGM(bool isOn)
    {
        SoundManager.Instance.PlaySFXSound("Button");
        localSettingInfo.isBgm = isOn;
        bgmButton.image.sprite = SetSoundImage(isOn);
        SoundManager.Instance.BGMVolumSet(isOn, localSettingInfo.bgmVolume);
        SetLocalSetting();
    }
    private void OnClick_SFX(bool isOn)
    {
        SoundManager.Instance.PlaySFXSound("Button");
        localSettingInfo.isSfx = isOn;
        sfxButton.image.sprite = SetSoundImage(isOn);
        SoundManager.Instance.SFXVolumSet(isOn, localSettingInfo.sfxVolume);
        SetLocalSetting();
    }

    private void OnClick_ApplicationExit()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        PopupState popupState = Les_UIManager.Instance.Popup<BasePopup_TwoBtn>().Open("앱을 종료하시겠습니까?");
        popupState.OnYes = p => Application.Quit();
    }

    private void OnClick_Exit()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        CloseTween(() => OnResult(PopupResults.Close));
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
