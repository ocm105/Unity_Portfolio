using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using TMPro;
using UnityEngine.UI;

public class DescriptPopup : UIPopup
{
    [SerializeField] TextMeshProUGUI descriptsText;
    [SerializeField] Button nextButton;

    private int descriptIndex;
    private string nowMessage;
    private bool isDone = false;
    private Coroutine coroutine;

    public PopupState Open(int index)
    {
        ShowLayer();
        Init(index);

        return state;
    }
    protected override void OnFirstShow()
    {
        nextButton.onClick.AddListener(OnClick_NextBtn);
    }
    protected override void OnShow() { }

    private void Init(int index)
    {
        descriptIndex = index;
        nowMessage = LocalizationManager.Instance.GetLocalizeText(GameDataManager.Instance.discription_Data[descriptIndex].descript_key);
        coroutine = StartCoroutine(SpeechText(nowMessage));
    }
    #region Event
    private void OnClick_NextBtn()
    {
        if (isDone)
        {
            if (GameDataManager.Instance.discription_Data[descriptIndex].next_index == -1)
                OnResult(PopupResults.Close);
            else
            {
                descriptIndex++;
                nowMessage = LocalizationManager.Instance.GetLocalizeText(GameDataManager.Instance.discription_Data[descriptIndex].descript_key);
                coroutine = StartCoroutine(SpeechText(nowMessage));
            }
        }
        else
        {
            if (coroutine != null) StopCoroutine(coroutine);
            descriptsText.text = nowMessage;
            isDone = true;
        }
    }
    private IEnumerator SpeechText(string key)
    {
        isDone = false;
        string text = string.Empty;
        descriptsText.text = text;

        for (int i = 0; i < nowMessage.Length; i++)
        {
            yield return new WaitForSeconds(0.05f);
            text += nowMessage[i];
            descriptsText.text = text;
        }

        isDone = true;
    }

    #endregion
}
