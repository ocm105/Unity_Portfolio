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

    public PopupState Open()
    {
        ShowLayer();
        return state;
    }

    protected override void OnFirstShow()
    {
        bgmSlider.onValueChanged.AddListener((value) => OnChange_BGM(value));
        sfxSlider.onValueChanged.AddListener((value) => OnChange_SFX(value));
        bgmButton.onClick.AddListener(() => OnClick_BGM(!GameDataManager.Instance.localSettingInfo.isBgm));
        sfxButton.onClick.AddListener(() => OnClick_SFX(!GameDataManager.Instance.localSettingInfo.isSfx));

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
        bgmSlider.value = GameDataManager.Instance.localSettingInfo.bgmVolume;
        sfxSlider.value = GameDataManager.Instance.localSettingInfo.sfxVolume;
        bgmButton.image.sprite = SetSoundImage(GameDataManager.Instance.localSettingInfo.isBgm);
        sfxButton.image.sprite = SetSoundImage(GameDataManager.Instance.localSettingInfo.isSfx);
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
        GameDataManager.Instance.localSettingInfo.bgmVolume = value;
        SoundManager.Instance.BGMVolumSet(GameDataManager.Instance.localSettingInfo.isBgm, value);
    }
    private void OnChange_SFX(float value)
    {
        GameDataManager.Instance.localSettingInfo.sfxVolume = value;
        SoundManager.Instance.SFXVolumSet(GameDataManager.Instance.localSettingInfo.isSfx, value);
    }

    private void OnClick_BGM(bool isOn)
    {
        SoundManager.Instance.PlaySFXSound("Button");
        GameDataManager.Instance.localSettingInfo.isBgm = isOn;
        bgmButton.image.sprite = SetSoundImage(isOn);
        SoundManager.Instance.BGMVolumSet(isOn, GameDataManager.Instance.localSettingInfo.bgmVolume);
    }
    private void OnClick_SFX(bool isOn)
    {
        SoundManager.Instance.PlaySFXSound("Button");
        GameDataManager.Instance.localSettingInfo.isSfx = isOn;
        sfxButton.image.sprite = SetSoundImage(isOn);
        SoundManager.Instance.SFXVolumSet(isOn, GameDataManager.Instance.localSettingInfo.sfxVolume);
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
