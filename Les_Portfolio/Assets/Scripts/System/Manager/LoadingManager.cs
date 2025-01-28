using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

public class LoadingManager : SingletonMonoBehaviour<LoadingManager>
{
    [SerializeField] GameObject fadeCanvas;
    [SerializeField] Image fadeImage;
    [SerializeField] float fadeTime = 1f;

    [SerializeField] GameObject loadingObj;

    private Color fadeInColor = Color.clear;
    private Color fadeOutColor = Color.black;
    private bool isFade = false;

    protected override void OnAwakeSingleton()
    {
        base.OnAwakeSingleton();
        DontDestroyOnLoad(this);
    }

    #region Fade
    public void SetFadeIn(Action call = null)
    {
        fadeImage.DOColor(fadeInColor, fadeTime).onComplete =
        () =>
        {
            isFade = false;
            fadeCanvas.SetActive(false);
            fadeImage.gameObject.SetActive(false);
            call.Invoke();
        };
    }
    public void SetFadeOut(Action call = null)
    {
        fadeCanvas.SetActive(true);
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOColor(fadeOutColor, fadeTime).onComplete =
        () =>
        {
            isFade = true;
            call.Invoke();
        };
    }
    #endregion

    #region SceneLoad
    public void SceneLoad(string sceneName)
    {
        StartCoroutine(SceneLoadCoroutine(sceneName));
    }
    private IEnumerator SceneLoadCoroutine(string sceneName)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SetFadeOut();
        yield return new WaitUntil(() => isFade == true);

        AsyncOperation load_op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        load_op.allowSceneActivation = false;

        yield return new WaitUntil(() => load_op.progress >= 0.9f);
        load_op.allowSceneActivation = true;
        yield return new WaitUntil(() => load_op.isDone);

        yield return null;

        // SceneManager.UnloadSceneAsync(currentSceneName);
        SetFadeIn();
        yield return new WaitUntil(() => isFade == false);

        yield break;
    }
    #endregion

    #region Loading
    public void SetLoading(bool isOn)
    {
        fadeCanvas.SetActive(isOn);
        loadingObj.SetActive(isOn);
    }
    #endregion
}
