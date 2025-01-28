using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using System;

[Serializable]
public class PlayerData
{
    public int index;
    public float hp;        // 체력
    public float mp;        // 마나
    public float atk;       // 공격력
    public float def;       // 방어력
    public float speed;     // 스피드
}

public partial class NetworkManager : SingletonMonoBehaviour<NetworkManager>
{
    public const string PLAYER_DATA_PATH = "https://docs.google.com/spreadsheets/d/117WcavqmLLFPs3JXc53kY7xaCW4Z8mbMcyv3iAhh3bw/export?format=csv";

    public IEnumerator GetPlayerDataRequest(Action<Dictionary<int, PlayerData>> callback = null)
    {
        yield return StartCoroutine(Request_Get(PLAYER_DATA_PATH, (dataState, resData) =>
        {
            switch (dataState)
            {
                case GAMEDATA_STATE.CONNECTDATAERROR:
                    break;
                case GAMEDATA_STATE.PROTOCOLERROR:
                    break;
                case GAMEDATA_STATE.REQUESTSUCCESS:
                    callback?.Invoke(CSVReader.ReadFromResource<PlayerData>(resData));
                    break;
            }
        }));
    }
}
