using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource m_MusicAudio;
    AudioSource m_SoundAudio;

    private void Awake()
    {
        m_MusicAudio = gameObject.AddComponent<AudioSource>();
        m_MusicAudio.playOnAwake = false;
        m_MusicAudio.loop = true;

        m_SoundAudio = gameObject.AddComponent<AudioSource>();
        m_SoundAudio.loop = false;
    }

    private float SoundVolume
    {
        get { return PlayerPrefs.GetFloat("SoundVolume", 1f); }
        set
        {
            m_SoundAudio.volume = value;
            PlayerPrefs.SetFloat("SoundVolume", value);
        }
    }

    private float MusicVolume
    {
        get { return PlayerPrefs.GetFloat("MusicVolume", 1f); }
        set
        {
            m_MusicAudio.volume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
    }

    string oldName;
    public void PlayMusic(string name)
    {
        if (this.MusicVolume < 0.1f)
            return;

        if (m_MusicAudio.clip != null && oldName == name)
        {
            m_MusicAudio.Play();
            return;
        }

        oldName = name;
        Manager.ResourceManager.LoadMusic(name, (UnityEngine.Object obj) =>
        {
            m_MusicAudio.clip = obj as AudioClip;
            m_MusicAudio.Play();
        });
    }

    public void PauseMusic() => m_MusicAudio.Pause();
    public void UnPauseMusic() => m_MusicAudio.UnPause();
    public void StopMusic() => m_MusicAudio.Stop();

    public void PlaySound(string name)
    {
        if (this.SoundVolume < 0.1f)
            return;

        Manager.ResourceManager.LoadSound(name, (UnityEngine.Object obj) =>
        {
            m_SoundAudio.PlayOneShot(obj as AudioClip);
        });
    }

    public void SetMusicVolume(float value)
    {
        this.MusicVolume = value;
    }

    public void SetSoundVolume(float value)
    {
        this.SoundVolume = value;
    }
}
