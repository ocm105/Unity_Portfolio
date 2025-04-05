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

    private AsyncOperationHandle<IList<AudioClip>> soundHandle;
    public List<AudioClip> soundList = new List<AudioClip>();

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

        // 다운로드 할 것을 가져오고 다운로드 할 것이 있을 때
        if (sizeHandle.Status == AsyncOperationStatus.Succeeded && downSize > 0)
        {
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(key, true);
            // 다운로드 완료
            downloadHandle.Completed += (handle) => DownloadComplete(handle.Status, callback);

            // 다운로드 중
            while (!downloadHandle.IsDone && downloadHandle.IsValid())
            {
                downPercent = downloadHandle.PercentComplete;
                yield return null;

                // 다운로드 실패
                if (downloadHandle.Status == AsyncOperationStatus.Failed)
                    DownloadFail();
            }
        }
        // 다운로드 할 것을 못 가져왔을 때
        else if (sizeHandle.Status == AsyncOperationStatus.Failed)
            DownloadFail();
        // 이미 다운로드가 되어있을 때
        else
            DownloadComplete(AsyncOperationStatus.Succeeded, callback);
    }
    // 다운받을 크기 Get
    private IEnumerator GetSizeCoroutine(object key)
    {
        sizeHandle = Addressables.GetDownloadSizeAsync(key);
        yield return new WaitUntil(() => sizeHandle.IsDone);
        downSize = sizeHandle.Result;
        yield break;
    }
    // 다운로드 성공
    private void DownloadComplete(AsyncOperationStatus status, Action<AsyncOperationStatus> callback = null)
    {
        WindowDebug.SuccessLog("Addressable DownLoad Complete");
        downPercent = 1f;
        isComplete = true;
        callback?.Invoke(status);
    }
    // 다운로드 실패
    private void DownloadFail()
    {
        WindowDebug.FailLog("Addressable DownLoad Fail");
        PopupState popupState = Les_UIManager.Instance.Popup<BasePopup_OneBtn>().Open("리소스 다운로드 실패");
        popupState.OnOK = p => Application.Quit();
    }
    #endregion

    #region Get
    public IEnumerator LoadData()
    {
        // yield return StartCoroutine(GetAddressableFBX());
        yield return StartCoroutine(GetAddressablePopup());
        yield return StartCoroutine(GetAddressableView());
        yield return StartCoroutine(GetAddressableSound());
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
    public IEnumerator GetAddressableSound()
    {
        soundHandle = Addressables.LoadAssetsAsync<AudioClip>("Sound", item =>
        {
            soundList.Add(item);
        });
        yield return new WaitUntil(() => soundHandle.IsDone);
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
    public AudioClip GetSound(string key)
    {
        AudioClip sound = soundList.Where(x => x.name == key).FirstOrDefault();
        return sound;
    }

    public TextAsset GetTable(string key)
    {
        var table = Addressables.LoadAssetAsync<TextAsset>(key);
        return table.WaitForCompletion();
    }
    #endregion
}
