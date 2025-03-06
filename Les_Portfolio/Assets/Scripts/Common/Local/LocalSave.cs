using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class LocalSave
{
    private const string PREFS_DATA_SETTING = "PREFS_DATA_SETTING";
    private const string PREFS_GAME_MAXSCORE = "PREFS_GAME_MAXSCORE";

    #region Base
    private static T GetLocalData<T>(string key)
    {
        string serializeData = GetString(key);
        var binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream(Convert.FromBase64String(serializeData));

        T info = (T)binaryFormatter.Deserialize(memoryStream);

        return info;
    }
    private static void SetLocalData<T>(string key, T info)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, info);

        string serialize_data = Convert.ToBase64String(ms.GetBuffer());

        SetString(key, serialize_data);
    }
    #endregion

    #region Util
    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key, 0);
    }
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static string GetString(string key)
    {
        return PlayerPrefs.GetString(key, string.Empty);
    }
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static bool GetBool(string key)
    {
        return PlayerPrefs.GetInt(key, 0) == 1 ? true : false;
    }
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    private static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }
    private static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion

    #region Setting
    public static LocalSettingInfo GetSettingInfo()
    {
        LocalSettingInfo info;

        string key = PREFS_DATA_SETTING;

        if (!HasKey(key))
        {
            info = new LocalSettingInfo();
            SetSettingInfo(info);
        }
        else
        {
            info = GetLocalData<LocalSettingInfo>(key);
        }

        return info;
    }
    public static void SetSettingInfo(LocalSettingInfo info)
    {
        string key = PREFS_DATA_SETTING;

        SetLocalData(key, info);
    }
    #endregion

    public static GameMaxScoreInfo GetGameMaxScoreInfo()
    {
        GameMaxScoreInfo info;

        string key = PREFS_GAME_MAXSCORE;

        if (!HasKey(key))
        {
            info = new GameMaxScoreInfo();
            SetGameMaxScoreInfo(info);
        }
        else
        {
            info = GetLocalData<GameMaxScoreInfo>(key);
        }

        return info;
    }
    public static void SetGameMaxScoreInfo(GameMaxScoreInfo info)
    {
        string key = PREFS_GAME_MAXSCORE;

        SetLocalData(key, info);
    }

}
