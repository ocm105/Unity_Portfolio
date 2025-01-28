using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BasePopup_OneBtn : BasePopup_Toast
{
    [SerializeField] Button okButton;

    protected override void OnFirstShow()
    {
        okButton.onClick.AddListener(OnClick_OkBtn);
    }
    protected override void OnShow()
    {
        ShowTween();
    }

    private void OnClick_OkBtn()
    {
        CloseTween(() => OnResult(PopupResults.OK));
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
