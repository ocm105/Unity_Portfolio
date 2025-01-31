using System;
using UnityEngine;
using UISystem;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class DescriptData
{
    public int index;
    public int next_index;
    public string descript_key;
}

public partial class NetworkManager : SingletonMonoBehaviour<NetworkManager>
{
    public const string DESCRIPT_DATA_PATH = "https://docs.google.com/spreadsheets/d/13vYZKF0P7r1ipcfow_eBzGAeFe0IoGscadxUe-_L8oU/export?format=csv";

    public IEnumerator GetDescriptRequest(Action<Dictionary<int, DescriptData>> callback = null)
    {
        yield return StartCoroutine(Request_Get(DESCRIPT_DATA_PATH, (dataState, resData) =>
        {
            switch (dataState)
            {
                case GAMEDATA_STATE.CONNECTDATAERROR:
                case GAMEDATA_STATE.PROTOCOLERROR:
                    PopupState popup = Les_UIManager.Instance.Popup<BasePopup_OneBtn>().Open("데이터를 받아오지 못했습니다.");
                    popup.OnClose = p => Application.Quit();
                    popup.OnOK = p => Application.Quit();
                    break;
                case GAMEDATA_STATE.REQUESTSUCCESS:
                    callback?.Invoke(CSVReader.ReadFromResource<DescriptData>(resData));
                    break;
            }
        }));
    }
}

