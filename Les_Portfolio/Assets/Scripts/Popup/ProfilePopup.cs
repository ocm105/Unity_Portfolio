using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UISystem;
using DG.Tweening;
using System;

public class ProfilePopup : UIPopup
{
    [SerializeField] GameObject frame;
    [SerializeField] Button githubButton;
    [SerializeField] Button closeButton;

    public PopupState Open()
    {
        ShowLayer();

        return state;
    }
    protected override void OnFirstShow()
    {
        githubButton.onClick.AddListener(OnClick_GithubBtn);
        closeButton.onClick.AddListener(OnClick_CloseBtn);
    }
    protected override void OnShow()
    {
        ShowTween();
    }

    private void OnClick_GithubBtn()
    {
        Application.OpenURL("https://github.com/ocm105/Les_Portfolio.git");
    }
    private void OnClick_CloseBtn()
    {
        CloseTween(() => OnResult(PopupResults.Close));
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
