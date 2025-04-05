using System;
using System.Collections;
using UISystem;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    private const string MAIN_BGM = "BGM_MAIN";
    private AudioClip main_Bgm;

    [SerializeField] GameObject mBGMSoundObj;
    [SerializeField] GameObject mSFXSoundObj;
    [SerializeField] GameObject mUISoundObj;
    private string currentBgmKeyName;

    [HideInInspector] public AudioSource mBGMAudio;
    [HideInInspector] public AudioSource mSFXAudio;
    [HideInInspector] public AudioSource mUIAudio;

    protected override void OnAwakeSingleton()
    {
        main_Bgm = Resources.Load<AudioClip>($"Sound/{MAIN_BGM}");
        Init();
        DontDestroyOnLoad(this);
    }

    private void Init()
    {
        mBGMAudio = mBGMSoundObj.GetComponent<AudioSource>() == null ? mBGMSoundObj.AddComponent<AudioSource>() : mBGMSoundObj.GetComponent<AudioSource>();
        mSFXAudio = mSFXSoundObj.GetComponent<AudioSource>() == null ? mSFXSoundObj.AddComponent<AudioSource>() : mSFXSoundObj.GetComponent<AudioSource>();
        mUIAudio = mUISoundObj.GetComponent<AudioSource>() == null ? mUISoundObj.AddComponent<AudioSource>() : mUISoundObj.GetComponent<AudioSource>();
        currentBgmKeyName = "";
    }

    private void SetAudioSource(AudioSource audio, string audio_name, bool isLoop = false)
    {
        audio.playOnAwake = false;
        audio.loop = isLoop;

        if (audio.clip == null || audio.clip.name != audio_name)
            audio.clip = AddressableManager.Instance.GetSound(audio_name);
    }
    private void SetAudioSource(AudioSource audio, AudioClip audioClip, bool isLoop = false)
    {
        audio.playOnAwake = false;
        audio.loop = isLoop;

        if (audio.clip == null || audio.clip != audioClip)
            audio.clip = audioClip;
    }

    #region PlaySound
    public void PlayMainBGMSound()
    {
        PlayBGMSound(main_Bgm);
    }
    public void PlayBGMSound(string key)
    {
        if (currentBgmKeyName == key)
            return;

        StopBGMSound();
        SetAudioSource(mBGMAudio, key, true);
        mBGMAudio.mute = !GameDataManager.Instance.localSettingInfo.isBgm;
        mBGMAudio.volume = GameDataManager.Instance.localSettingInfo.bgmVolume;
        mBGMAudio.Play();
        currentBgmKeyName = key;
    }
    public void PlayBGMSound(AudioClip audioClip)
    {
        if (currentBgmKeyName == audioClip.name)
            return;

        StopBGMSound();
        SetAudioSource(mBGMAudio, audioClip, true);
        mBGMAudio.mute = !GameDataManager.Instance.localSettingInfo.isBgm;
        mBGMAudio.volume = GameDataManager.Instance.localSettingInfo.bgmVolume;
        mBGMAudio.Play();
        currentBgmKeyName = audioClip.name;
    }

    public void PlaySFXSound(string key, bool isLoop = false)
    {
        SetAudioSource(mSFXAudio, key, isLoop);
        mSFXAudio.mute = !GameDataManager.Instance.localSettingInfo.isSfx;
        mSFXAudio.time = 0.0f;
        if (isLoop)
        {
            mSFXAudio.volume = GameDataManager.Instance.localSettingInfo.sfxVolume;
            mSFXAudio.Play();
        }
        else
            mSFXAudio.PlayOneShot(mSFXAudio.clip, GameDataManager.Instance.localSettingInfo.sfxVolume);
    }
    public void PlaySFXSound(AudioClip audioClip, bool isLoop = false)
    {
        SetAudioSource(mSFXAudio, audioClip);
        mSFXAudio.mute = !GameDataManager.Instance.localSettingInfo.isSfx;
        mSFXAudio.time = 0.0f;
        if (isLoop)
        {
            mSFXAudio.volume = GameDataManager.Instance.localSettingInfo.sfxVolume;
            mSFXAudio.Play();
        }
        else
            mSFXAudio.PlayOneShot(mSFXAudio.clip, GameDataManager.Instance.localSettingInfo.sfxVolume);
    }

    public void PlayUISound(string key)
    {
        SetAudioSource(mUIAudio, key);
        mUIAudio.mute = !GameDataManager.Instance.localSettingInfo.isSfx;
        mUIAudio.time = 0.0f;
        mUIAudio.PlayOneShot(mUIAudio.clip, GameDataManager.Instance.localSettingInfo.sfxVolume);
    }
    #endregion

    #region Stop & Pause
    public void PauseBGMSound(bool isPause)
    {
        if (mBGMAudio == null) return;

        if (isPause)
            mBGMAudio.Pause();
        else
            mBGMAudio.UnPause();
    }

    public void StopBGMSound()
    {
        currentBgmKeyName = "";
        if (mBGMAudio == null) return;
        mBGMAudio.Stop();
    }

    public void StopSFXSound()
    {
        if (mSFXAudio == null) return;
        mSFXAudio.Stop();
    }

    public void StopUISound()
    {
        if (mUIAudio == null) return;
        mUIAudio.Stop();
    }
    #endregion

    #region SoundControl
    public void BGMVolumSet(bool mute, float value)
    {
        GameDataManager.Instance.localSettingInfo.bgmVolume = value;
        if (mBGMAudio == null) return;

        mBGMAudio.mute = !mute;
        mBGMAudio.volume = value;
    }
    public void SFXVolumSet(bool mute, float value)
    {
        GameDataManager.Instance.localSettingInfo.sfxVolume = value;
        if (mSFXAudio == null) return;

        mSFXAudio.mute = !mute;
        mSFXAudio.volume = value;
        UIVolumSet(mute, value);
    }
    public void UIVolumSet(bool mute, float value)
    {
        GameDataManager.Instance.localSettingInfo.sfxVolume = value;
        if (mUIAudio == null) return;

        mUIAudio.mute = !mute;
        mUIAudio.volume = value;
    }
    #endregion
}
