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

    public AudioListener AudioListener;
    public bool mIsBGMSoundVolume = true;
    public bool mIsSFXSoundVolume = true;
    public bool mIsUISoundVolume = true;

    public float mBGMVolume = 1.0f;
    public float mSFXVolume = 1.0f;
    public float mUIVolume = 1.0f;

    protected override void OnAwakeSingleton()
    {
        main_Bgm = Resources.Load<AudioClip>($"Sound/{MAIN_BGM}");
        Init();
        DontDestroyOnLoad(this);
    }

    void Init()
    {
        // LocalSettingInfo settingInfo = FileSave.GetSettingInfo();
        // mBGMVolume = settingInfo.bgmVolum;
        // mSFXVolume = settingInfo.sfxVolum;
        // mUIVolume = settingInfo.sfxVolum;
        // mTTSVolume = settingInfo.sfxVolum;

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
        if (mIsBGMSoundVolume == false)
            return;
        if (currentBgmKeyName == key)
            return;

        StopBGMSound();
        SetAudioSource(mBGMAudio, key, true);
        mBGMAudio.volume = mBGMVolume;
        mBGMAudio.Play();
        currentBgmKeyName = key;
    }
    public void PlayBGMSound(AudioClip audioClip)
    {
        if (mIsBGMSoundVolume == false)
            return;
        if (currentBgmKeyName == audioClip.name)
            return;

        StopBGMSound();
        SetAudioSource(mBGMAudio, audioClip, true);
        mBGMAudio.volume = mBGMVolume;
        mBGMAudio.Play();
        currentBgmKeyName = audioClip.name;
    }

    public void PlaySFXSound(string key, bool isLoop = false)
    {
        if (mIsSFXSoundVolume == false)
            return;

        SetAudioSource(mSFXAudio, key, isLoop);
        mSFXAudio.time = 0.0f;
        if (isLoop)
            mSFXAudio.Play();
        else
            mSFXAudio.PlayOneShot(mSFXAudio.clip, mSFXVolume);
    }
    public void PlaySFXSound(AudioClip audioClip, bool isLoop = false)
    {
        if (mIsSFXSoundVolume == false)
            return;

        SetAudioSource(mSFXAudio, audioClip);
        mSFXAudio.time = 0.0f;
        if (isLoop)
            mSFXAudio.Play();
        else
            mSFXAudio.PlayOneShot(mSFXAudio.clip, mSFXVolume);
    }

    public void PlayUISound(string key)
    {
        if (mIsUISoundVolume == false)
            return;

        SetAudioSource(mUIAudio, key);
        mUIAudio.time = 0.0f;
        mUIAudio.PlayOneShot(mUIAudio.clip, mUIVolume);
    }
    #endregion

    #region Stop & Pause
    public void PauseBGMSound(bool isPause)
    {
        AudioSource[] audios = mBGMSoundObj.GetComponents<AudioSource>();

        for (int i = 0; i < audios.Length; i++)
        {
            if (audios[i] == null) continue;

            if (isPause)
                audios[i].Pause();
            else
                audios[i].UnPause();
        }
    }

    public void StopBGMSound()
    {
        AudioSource[] audios = mBGMSoundObj.GetComponents<AudioSource>();

        for (int i = 0; i < audios.Length; i++)
        {
            if (audios[i] == null) continue;

            audios[i].Stop();
        }
        mBGMAudio?.Stop();
        currentBgmKeyName = "";
    }

    public void StopSFXSound()
    {
        AudioSource[] audios = mSFXSoundObj.GetComponents<AudioSource>();

        for (int i = 0; i < audios.Length; i++)
        {
            if (audios[i] == null) continue;

            audios[i].Stop();
        }
    }

    public void StopUISound()
    {
        AudioSource[] audios = mUISoundObj.GetComponents<AudioSource>();

        for (int i = 0; i < audios.Length; i++)
        {
            if (audios[i] == null) continue;

            audios[i].Stop();
        }
    }
    #endregion

    #region SoundControl
    public void BGMVolumSet(float value)
    {
        AudioSource[] audios = mBGMSoundObj.GetComponents<AudioSource>();
        mBGMVolume = value;

        for (int i = 0; i < audios.Length; i++)
        {
            if (audios[i] == null) continue;

            audios[i].volume = mBGMVolume;
        }
    }
    public void SFXVolumSet(float value)
    {
        AudioSource[] audios = mSFXSoundObj.GetComponents<AudioSource>();
        mSFXVolume = value;

        for (int i = 0; i < audios.Length; i++)
        {
            if (audios[i] == null) continue;

            audios[i].volume = mSFXVolume;
        }
    }
    public void UIVolumSet(float value)
    {
        AudioSource[] audios = mUISoundObj.GetComponents<AudioSource>();
        mUIVolume = value;

        for (int i = 0; i < audios.Length; i++)
        {
            if (audios[i] == null) continue;

            audios[i].volume = mUIVolume;
        }
    }
    #endregion
}
