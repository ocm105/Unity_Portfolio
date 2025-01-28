using System;
using System.Collections;
using UISystem;
using UnityEngine;
using UnityEngine.Networking;

public partial class NetworkManager : SingletonMonoBehaviour<NetworkManager>
{
    protected override void OnAwakeSingleton()
    {
        base.OnAwakeSingleton();
        DontDestroyOnLoad(this);
    }

    private bool NetworkCheck()
    {
        // 인터넷 연결이 안되었을 때 행동
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            PopupState popup = Les_UIManager.Instance.Popup<BasePopup_OneBtn>().Open("네트워크 연결 오류");
            popup.OnClose = p => Application.Quit();
            popup.OnOK = p => Application.Quit();
            return false;
        }
        // 데이터로 연결이 되었을 때 행동
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            return true;
        }
        // 와이파이로 연결이 되었을 때 행동
        else
        {
            return true;
        }
    }

    public IEnumerator Request_Post(string url, WWWForm form, Action<GAMEDATA_STATE, string> resultAction = null)
    {
        if (NetworkCheck())
        {
            Les_UIManager.Instance.CurrentView.Loading = true;

            // string jsonReq = JsonUtility.ToJson(reqParam);

            // Debug.Log($"<color=green>[Req_Post]</color>=> url: {url}, param: {jsonReq}");

            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                // byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonReq);
                // request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                // request.downloadHandler = new DownloadHandlerBuffer();

                // request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                // Debug.Log($"<color=yellow>[Res_Post]</color>=> url: {url}, resData: {request.downloadHandler.text}");

                switch (request.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        resultAction?.Invoke(GAMEDATA_STATE.CONNECTDATAERROR, request.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        resultAction?.Invoke(GAMEDATA_STATE.PROTOCOLERROR, request.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.Success:
                        resultAction?.Invoke(GAMEDATA_STATE.REQUESTSUCCESS, request.downloadHandler.text);
                        break;
                }

                request.Dispose();
            }

            Les_UIManager.Instance.CurrentView.Loading = false;
        }
    }

    public IEnumerator Request_Get(string url, Action<GAMEDATA_STATE, string> resultAction = null)
    {
        if (NetworkCheck())
        {
            Les_UIManager.Instance.CurrentView.Loading = true;

            // Debug.Log($"<color=green>[Req_Get]</color>=> url: {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                // Debug.Log($"<color=yellow>[Res_Get]</color>=> url: {url}, resData: {request.downloadHandler.text}");

                switch (request.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        resultAction?.Invoke(GAMEDATA_STATE.CONNECTDATAERROR, request.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        resultAction?.Invoke(GAMEDATA_STATE.PROTOCOLERROR, request.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.Success:
                        resultAction?.Invoke(GAMEDATA_STATE.REQUESTSUCCESS, request.downloadHandler.text);
                        break;
                }

                request.Dispose();
            }

            Les_UIManager.Instance.CurrentView.Loading = false;
        }
    }

}
