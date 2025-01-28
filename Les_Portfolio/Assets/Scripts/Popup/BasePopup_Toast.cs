using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using TMPro;
using DG.Tweening;

public class BasePopup_Toast : UIPopup
{
    [SerializeField] protected GameObject frame;
    [SerializeField] TextMeshProUGUI messageText;

    public PopupState Open(string msg)
    {
        ShowLayer();
        SetMessage(msg);
        return state;
    }
    protected override void OnFirstShow()
    {
    }
    protected override void OnShow()
    {
        Toast_Tween();
    }

    protected virtual void SetMessage(string msg)
    {
        messageText.text = LocalizationManager.Instance.GetLocalizeText(msg);
    }

    private void Toast_Tween()
    {
        frame.transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(frame.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic));
        sequence.AppendInterval(2f);
        sequence.Append(frame.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCubic));
        sequence.SetAutoKill(true);

        sequence.OnComplete(() => OnResult(PopupResults.Close));
    }
}
