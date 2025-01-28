using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
using System;

public class AddressableManager : SingletonMonoBehaviour<AddressableManager>
{
    private AsyncOperationHandle<IList<GameObject>> fbxHandle;
    public List<GameObject> fbxList = new List<GameObject>();

    private AsyncOperationHandle<IList<GameObject>> popupHandle;
    public List<GameObject> popupList = new List<GameObject>();

    private AsyncOperationHandle<IList<GameObject>> viewHandle;
    public List<GameObject> viewList = new List<GameObject>();

    private AsyncOperationHandle<long> sizeHandle;
    public long downSize { get; private set; }
    public float downPercent { get; private set; }
    public bool isComplete { get; private set; }


    protected override void OnAwakeSingleton()
    {
        base.OnAwakeSingleton();
        DontDestroyOnLoad(this);
    }

    #region DownLoad
    public void StartDownload_Addressable(object key, Action<AsyncOperationStatus> callback = null)
    {
        isComplete = false;
        downPercent = 0f;
        StartCoroutine(DownLoadCoroutine(key, callback));
    }

    private IEnumerator DownLoadCoroutine(object key, Action<AsyncOperationStatus> callback = null)
    {
        yield return StartCoroutine(GetSizeCoroutine(key));

        // Debug.Log(sizeHandle.Status);
        if (sizeHandle.Status == AsyncOperationStatus.Succeeded && downSize > 0)
        {
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(key, true);

            downloadHandle.Completed += (handler) =>
            {
                WindowDebug.SuccessLog("Addressable DownLoad Complete");

                downPercent = 1f;
                isComplete = true;
                callback?.Invoke(handler.Status);
            };

            while (!downloadHandle.IsDone && downloadHandle.IsValid())
            {
                if (downloadHandle.Status == AsyncOperationStatus.Failed)
                {
                    WindowDebug.FailLog("Addressable DownLoad Fail");
                }
                downPercent = downloadHandle.PercentComplete;
                yield return null;
            }
        }
        else if (sizeHandle.Status == AsyncOperationStatus.Failed)
        {
            WindowDebug.FailLog("Addressable DownLoad Fail");
            PopupState popupState = Les_UIManager.Instance.Popup<BasePopup_OneBtn>().Open("리소스 다운로드 실패");
            popupState.OnOK = p => Application.Quit();
        }
        else
        {
            WindowDebug.SuccessLog("Addressable Already DownLoad");

            downPercent = 1f;
            isComplete = true;
            callback?.Invoke(AsyncOperationStatus.Succeeded);
        }
    }
    private IEnumerator GetSizeCoroutine(object key)
    {
        sizeHandle = Addressables.GetDownloadSizeAsync(key);
        yield return new WaitUntil(() => sizeHandle.IsDone);
        downSize = sizeHandle.Result;
        yield break;
    }
    #endregion

    #region Get
    public IEnumerator LoadData()
    {
        yield return StartCoroutine(GetAddressableFBX());
        yield return StartCoroutine(GetAddressablePopup());
        yield return StartCoroutine(GetAddressableView());
    }

    public IEnumerator GetAddressableFBX()
    {
        fbxHandle = Addressables.LoadAssetsAsync<GameObject>("Fbx", item =>
        {
            fbxList.Add(item);
        });
        yield return new WaitUntil(() => fbxHandle.IsDone);
    }
    public IEnumerator GetAddressablePopup()
    {
        popupHandle = Addressables.LoadAssetsAsync<GameObject>("Popup", item =>
        {
            popupList.Add(item);
        });
        yield return new WaitUntil(() => popupHandle.IsDone);
    }
    public IEnumerator GetAddressableView()
    {
        viewHandle = Addressables.LoadAssetsAsync<GameObject>("View", item =>
        {
            viewList.Add(item);
        });
        yield return new WaitUntil(() => viewHandle.IsDone);
    }

    public TextAsset GetTable(string key)
    {
        var table = Addressables.LoadAssetAsync<TextAsset>(key);
        return table.WaitForCompletion();
    }
    public GameObject GetFBX(string key)
    {
        GameObject fbx = fbxList.Where(x => x.name == key).FirstOrDefault();
        return fbx;
    }
    public GameObject GetPopup(string key)
    {
        GameObject popup = popupList.Where(x => x.name == key).FirstOrDefault();
        return popup;
    }
    public GameObject GetView(string key)
    {
        GameObject view = viewList.Where(x => x.name == key).FirstOrDefault();
        return view;
    }
    #endregion
}
