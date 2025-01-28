using System;


[Serializable]
public class LocalSettingInfo
{
    public float bgmVolume;
    public float sfxVolume;
    public bool isBgm;
    public bool isSfx;

    public LanguageType languageType;

    public LocalSettingInfo()
    {
        bgmVolume = 1f;
        sfxVolume = 1f;
        isBgm = true;
        isSfx = true;

        languageType = LanguageType.Korean;
    }
}