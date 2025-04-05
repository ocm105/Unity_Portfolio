using System;


[Serializable]
public class LocalSettingInfo
{
    public float bgmVolume;
    public float sfxVolume;
    public bool isBgm;
    public bool isSfx;

    public LocalSettingInfo()
    {
        bgmVolume = 1f;
        sfxVolume = 1f;
        isBgm = true;
        isSfx = true;
    }
}

[Serializable]
public class GameMaxScoreInfo
{
    public int dongleMaxScore;

    public GameMaxScoreInfo()
    {
        dongleMaxScore = 0;
    }
}