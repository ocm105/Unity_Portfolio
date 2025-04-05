using System;
using System.Collections;
using UISystem;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    private const string MAIN_BGM = "BGM_MAIN";
    private AudioClip main_Bgm;

    private GameObject mBGMSoundObj;
    private GameObject mSFXSoundObj;
    private GameObject mUISoundObj;
    private string currentBgmKeyName;

    [HideInInspector] public AudioSource mBGMAudio;
    [HideInInspector] public AudioSource mSFXAudio;
    [HideInInspector] public AudioSource mUIAudio;

    private float mBGMVolume = 1.0f;
    private float mSFXVolume = 1.0f;
    private float mUIVolume = 1.0f;

    private bool isBGM = true;
    private bool isSFX = true;
    private bool isUI = true;

    protected override void OnAwakeSingleton()
    {
        main_Bgm = Resources.Load<AudioClip>($"Sound/{MAIN_BGM}");
        Init();
        DontDestroyOnLoad(this);
    }

    private void Init()
    {
        GetLocalSetting();

        if (mBGMSoundObj != null)
        {
            Destroy(mBGMSoundObj);
            mBGMSoundObj = null;
        }
        mBGMSoundObj = new GameObject("BGMSound");
        mBGMAudio = mBGMSoundObj.GetComponent<AudioSource>() == null ? mBGMSoundObj.AddComponent<AudioSource>() : mBGMSoundObj.GetComponent<AudioSource>();
        mBGMSoundObj.transform.parent = this.transform;
        currentBgmKeyName = "";

        if (mSFXSoundObj != null)
        {
            Destroy(mSFXSoundObj);
            mSFXSoundObj = null;
        }
        mSFXSoundObj = new GameObject("SFXSound");
        mSFXAudio = mSFXSoundObj.GetComponent<AudioSource>() == null ? mSFXSoundObj.AddComponent<AudioSource>() : mSFXSoundObj.GetComponent<AudioSource>();
        mSFXSoundObj.transform.parent = this.transform;

        if (mUISoundObj != null)
        {
            Destroy(mUISoundObj);
            mUISoundObj = null;
        }
        mUISoundObj = new GameObject("UISound");
        mUIAudio = mUISoundObj.GetComponent<AudioSource>() == null ? mUISoundObj.AddComponent<AudioSource>() : mUISoundObj.GetComponent<AudioSource>();
        mUISoundObj.transform.parent = this.transform;
    }

    private void GetLocalSetting()
    {
        LocalSettingInfo settingInfo = LocalSave.GetSettingInfo();
        mBGMVolume = settingInfo.bgmVolume;
        mSFXVolume = settingInfo.sfxVolume;
        mUIVolume = settingInfo.sfxVolume;

        isBGM = settingInfo.isBgm;
        isSFX = settingInfo.isSfx;
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
        mBGMAudio.mute = isBGM;
        mBGMAudio.volume = mBGMVolume;
        mBGMAudio.Play();
        currentBgmKeyName = key;
    }
    public void PlayBGMSound(AudioClip audioClip)
    {
        if (currentBgmKeyName == audioClip.name)
            return;

        StopBGMSound();
        SetAudioSource(mBGMAudio, audioClip, true);
        mBGMAudio.mute = isBGM;
        mBGMAudio.volume = mBGMVolume;
        mBGMAudio.Play();
        currentBgmKeyName = audioClip.name;
    }

    public void PlaySFXSound(string key, bool isLoop = false)
    {
        SetAudioSource(mSFXAudio, key, isLoop);
        mSFXAudio.mute = isSFX;
        mSFXAudio.time = 0.0f;
        if (isLoop)
        {
            mSFXAudio.volume = mSFXVolume;
            mSFXAudio.Play();
        }
        else
            mSFXAudio.PlayOneShot(mSFXAudio.clip, mSFXVolume);
    }
    public void PlaySFXSound(AudioClip audioClip, bool isLoop = false)
    {
        SetAudioSource(mSFXAudio, audioClip);
        mSFXAudio.mute = isSFX;
        mSFXAudio.time = 0.0f;
        if (isLoop)
        {
            mSFXAudio.volume = mSFXVolume;
            mSFXAudio.Play();
        }
        else
            mSFXAudio.PlayOneShot(mSFXAudio.clip, mSFXVolume);
    }

    public void PlayUISound(string key)
    {
        SetAudioSource(mUIAudio, key);
        mUIAudio.mute = isSFX;
        mUIAudio.time = 0.0f;
        mUIAudio.PlayOneShot(mUIAudio.clip, mUIVolume);
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
        mBGMVolume = value;
        if (mBGMAudio == null) return;

        mBGMAudio.volume = mBGMVolume;
    }
    public void SFXVolumSet(bool mute, float value)
    {
        mSFXVolume = value;
        if (mSFXAudio == null) return;

        mSFXAudio.volume = mSFXVolume;
    }
    public void UIVolumSet(bool mute, float value)
    {
        mUIVolume = value;
        if (mUIAudio == null) return;

        mUIAudio.volume = mUIVolume;
    }
    #endregion
}
