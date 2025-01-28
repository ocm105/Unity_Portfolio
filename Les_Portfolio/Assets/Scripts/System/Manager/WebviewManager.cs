using System;
using UISystem;
using UnityEngine;

public class WebviewManager : SingletonMonoBehaviour<WebviewManager>
{
    private WebViewObject webViewObject;

    bool isWebview = false;

    private void Init()
    {
        webViewObject = new GameObject("WebViewObject").AddComponent<WebViewObject>();
        webViewObject.transform.parent = transform;
        webViewObject.Init((msg) =>
        {
            Debug.Log(string.Format("CallFromJS[{0}]", msg));
        });
    }

    /// <summary> WebView FullScreenMode </summary>
    public void OpenWebView(string url, Action call = null)
    {
        if (isWebview == true)
            return;
        try
        {
            Init();


            webViewObject.LoadURL(url);
            webViewObject.SetVisibility(true);
            webViewObject.SetMargins(0, 0, 0, 0);

            isWebview = true;

            if (call != null)
                call.Invoke();
        }
        catch (System.Exception e)
        {
            print($"WebView Error : {e}");
        }
    }

    /// <summary> WebView MarginValue (int)  </summary>
    public void OpenWebView(string url, int left, int right, int top, int bottom, Action call = null)
    {
        if (isWebview == true)
            return;
        try
        {
            Init();

            webViewObject.LoadURL(url);
            webViewObject.SetVisibility(true);
            webViewObject.SetMargins(left, top, right, bottom);

            isWebview = true;

            if (call != null)
                call.Invoke();
        }
        catch (System.Exception e)
        {
            print($"WebView Error : {e}");
        }
    }

    /// <summary> WebView MarginPersent (float) </summary>
    public void OpenWebView(string url, float left, float right, float top, float bottom, Action call = null)
    {
        if (isWebview == true)
            return;
        try
        {
            Init();

            int _left = Mathf.RoundToInt(Screen.width * (left * 0.01f));
            int _right = Mathf.RoundToInt(Screen.width * (right * 0.01f));
            int _top = Mathf.RoundToInt(Screen.height * (top * 0.01f));
            int _bottom = Mathf.RoundToInt(Screen.height * (bottom * 0.01f));


            webViewObject.LoadURL(url);
            webViewObject.SetVisibility(true);
            webViewObject.SetMargins(_left, _top, _right, _bottom);

            isWebview = true;

            if (call != null)
                call.Invoke();
        }
        catch (System.Exception e)
        {
            print($"WebView Error : {e}");
        }
    }


    public void CloseWebView(Action call = null)
    {
        if (webViewObject != null)
        {
            Destroy(webViewObject);
            isWebview = false;

            if (call != null)
                call.Invoke();
        }
    }
}
