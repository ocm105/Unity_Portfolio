using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class WebviewPopup : UIPopup
{
    [SerializeField] Button exitButton;

    public PopupState Open()
    {
        ShowLayer();

        return state;
    }
    protected override void OnFirstShow()
    {
        exitButton.onClick.AddListener(OnClick_OkBtn);
    }
    protected override void OnShow() { }

    private void OnClick_OkBtn()
    {
        OnResult(PopupResults.Close);
    }


}
