using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    //音频管理器 存储所有的音频并且可以播放和停止
    [Serializable]
    public class Sound
    {
        public string audioUse;

        [Header("音频剪辑")]
        public AudioClip clip;

        [Header("音频音量")]
        [Range(0, 1)]
        public float volume = 1;

        [Header("音频是否自启动")]
        public bool PlayOnAwake;

        [Header("音频是否要循环播放")]
        public bool loop;
    }

    public List<Sound> sounds;//存储所有音频的信息

    public AudioMixer audioMixer;

    private Dictionary<string, AudioSource> audioDic;//每一个音频的名称组件

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(AudioManager)) as AudioManager;
            }
            return instance;
        }
    }

    public void Initialized()
    {
        audioDic = new Dictionary<string, AudioSource>();
        AudioInit();
        SetBgmState(GameDataManager.Instance.GetBgmState());
        SetSoundState(GameDataManager.Instance.GetSoundState());

    }
    private void AudioInit()
    {
        foreach (var sound in sounds)
        {
            GameObject obj = new GameObject(sound.clip.name);
            obj.transform.SetParent(transform);

            AudioSource source = obj.AddComponent<AudioSource>();
            source.clip = sound.clip;
            source.volume = sound.volume;
            source.playOnAwake = sound.PlayOnAwake;
            source.loop = sound.loop;
            if (sound.PlayOnAwake)
            {
                source.Play();
            }
            audioDic.Add(sound.audioUse, source);
        }
    }


    //播放某个音频的方法 iswait为是否等待
    public static void PlayAudio(string name, bool iswait = false)
    {
        if (!instance.audioDic.ContainsKey(name))
        {
            //不存在此音频
            DebugHelper.LogError("不存在" + name + "音频");
            return;
        }
        if (iswait)
        {
            if (!instance.audioDic[name].isPlaying)
            {
                //如果是等待的情况 不在播放
                instance.audioDic[name].Play();
            }
        }
        else
        {
            //直接播放
            instance.audioDic[name].Play();
        }
    }


    //停止音频的播放
    public static void StopMute(string name)
    {

        if (!instance.audioDic.ContainsKey(name))
        {
            //不存在此音频
            DebugHelper.LogError("不存在" + name + "音频");
            return;
        }
        else
        {
            instance.audioDic[name].Stop();

        }
    }

    public void SetSoundState(bool state)
    {
        foreach (var sound in audioDic)
        {
            if (!sound.Key.Equals("BGM"))
            {
                sound.Value.mute = !state;
            }
        }

    }

    public void SetBgmState(bool state)
    {
        foreach (var sound in audioDic)
        {
            if (sound.Key.Equals("BGM"))
            {
                sound.Value.mute = !state;
            }
        }
    }

}

