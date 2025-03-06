using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UISystem;
using DG.Tweening;
using System;
using System.Collections;

public class GameResultPopup : UIPopup
{
    [SerializeField] GameObject window;
    [SerializeField] GameObject nowScoreParent;
    [SerializeField] GameObject maxScoreParent;
    [SerializeField] TextMeshProUGUI nowScoreText;
    [SerializeField] TextMeshProUGUI maxScoreText;

    [SerializeField] Button closeButton;

    private int nowScore = 0;
    private int maxScore = 0;
    private int frame = 0, maxFrame = 30;

    public PopupState Open(int nowScore, int maxScore)
    {
        this.nowScore = nowScore;
        this.maxScore = maxScore;
        ShowLayer();
        Init();

        return state;
    }
    protected override void OnFirstShow()
    {
        closeButton.onClick.AddListener(OnClick_CloseBtn);
    }
    protected override void OnShow()
    {
        ShowTween(() => StartCoroutine(ScoreCoroutine()));
    }
    private void Init()
    {
        frame = 0;
        nowScoreParent.SetActive(false);
        maxScoreParent.SetActive(false);
        nowScoreText.text = "-";
        nowScoreText.text = "-";
        closeButton.gameObject.SetActive(false);
    }

    private IEnumerator ScoreCoroutine()
    {
        nowScoreParent.SetActive(true);
        int temp = nowScore / maxFrame;

        while (frame < maxFrame)
        {
            nowScoreText.text = (temp * frame).ToString("n0");
            yield return null;
            frame++;
        }
        nowScoreText.text = nowScore.ToString("n0");

        yield return new WaitForSeconds(0.5f);

        if (nowScore > maxScore) maxScore = nowScore;

        maxScoreParent.SetActive(true);

        temp = maxScore / maxFrame;

        while (frame < maxFrame)
        {
            maxScoreText.text = (temp * frame).ToString("n0");
            yield return null;
            frame++;
        }
        maxScoreText.text = maxScore.ToString("n0");

        yield return new WaitForSeconds(0.5f);
        closeButton.gameObject.SetActive(true);
    }

    private void OnClick_CloseBtn()
    {
        CloseTween(() => OnResult(PopupResults.Close));
    }

    #region Tween
    private void ShowTween(Action call = null)
    {
        window.transform.localScale = Vector3.zero;
        window.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic).onComplete = () => call?.Invoke();
    }
    private void CloseTween(Action call = null)
    {
        window.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutCubic).OnComplete(call.Invoke);
    }
    #endregion
}
