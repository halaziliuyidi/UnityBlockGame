using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    //��Ƶ������ �洢���е���Ƶ���ҿ��Բ��ź�ֹͣ
    [Serializable]
    public class Sound
    {
        public string audioUse;

        [Header("��Ƶ����")]
        public AudioClip clip;

        [Header("��Ƶ����")]
        [Range(0, 1)]
        public float volume = 1;

        [Header("��Ƶ�Ƿ�������")]
        public bool PlayOnAwake;

        [Header("��Ƶ�Ƿ�Ҫѭ������")]
        public bool loop;
    }

    public List<Sound> sounds;//�洢������Ƶ����Ϣ

    public AudioMixer audioMixer;

    private Dictionary<string, AudioSource> audioDic;//ÿһ����Ƶ���������

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


    //����ĳ����Ƶ�ķ��� iswaitΪ�Ƿ�ȴ�
    public static void PlayAudio(string name, bool iswait = false)
    {
        if (!instance.audioDic.ContainsKey(name))
        {
            //�����ڴ���Ƶ
            DebugHelper.LogError("������" + name + "��Ƶ");
            return;
        }
        if (iswait)
        {
            if (!instance.audioDic[name].isPlaying)
            {
                //����ǵȴ������ ���ڲ���
                instance.audioDic[name].Play();
            }
        }
        else
        {
            //ֱ�Ӳ���
            instance.audioDic[name].Play();
        }
    }


    //ֹͣ��Ƶ�Ĳ���
    public static void StopMute(string name)
    {

        if (!instance.audioDic.ContainsKey(name))
        {
            //�����ڴ���Ƶ
            DebugHelper.LogError("������" + name + "��Ƶ");
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

