using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class GameDataManager : SingletonMonoBehaviour<GameDataManager>
{
    public Dictionary<int, DescriptData> discription_Data = new Dictionary<int, DescriptData>();
    public Dictionary<int, PlayerData> player_Data = new Dictionary<int, PlayerData>();
    public bool isDataLoad_Completed { get; private set; }

    public GameMaxScoreInfo gameMaxScoreInfo;

    protected override void OnAwakeSingleton()
    {
        base.OnAwakeSingleton();
        DontDestroyOnLoad(this);
    }

    public IEnumerator LoadData()
    {
        if (isDataLoad_Completed == false)
        {
            yield return StartCoroutine(NetworkManager.Instance.GetDescriptRequest((resData) => discription_Data = resData));
            yield return StartCoroutine(NetworkManager.Instance.GetPlayerDataRequest((resData) => player_Data = resData));
        }

        gameMaxScoreInfo = LocalSave.GetGameMaxScoreInfo();
        isDataLoad_Completed = true;
    }

    public void GameMaxScoreUpdate()
    {
        LocalSave.SetGameMaxScoreInfo(gameMaxScoreInfo);
    }
}
