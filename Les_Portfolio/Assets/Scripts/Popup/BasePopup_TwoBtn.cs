using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BasePopup_TwoBtn : BasePopup_Toast
{
    [SerializeField] Button okButton;
    [SerializeField] Button noButton;
    [SerializeField] Button exitButton;

    protected override void OnFirstShow()
    {
        okButton.onClick.AddListener(OnClick_OkBtn);
        noButton.onClick.AddListener(OnClick_NoBtn);
        exitButton.onClick.AddListener(OnClick_NoBtn);
    }
    protected override void OnShow()
    {
        ShowTween();
    }

    private void OnClick_OkBtn()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        CloseTween(() => OnResult(PopupResults.Yes));
    }
    private void OnClick_NoBtn()
    {
        SoundManager.Instance.PlaySFXSound("Button");
        CloseTween(() => OnResult(PopupResults.No));
    }

    private void ShowTween()
    {
        frame.transform.localScale = Vector3.zero;
        frame.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic);
    }
    private void CloseTween(Action call)
    {
        frame.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutCubic).OnComplete(call.Invoke);
    }
}
