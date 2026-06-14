using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
        audioClips = new Dictionary<string, AudioClip>();
    }
    private AudioSource audioSource;
    private Dictionary<string, AudioClip> audioClips;
    // 辅助函数：加载音频文件
    public AudioClip LoadAudio(string path)
    {
        return (AudioClip)Resources.Load(path);
    }
    // 辅助函数：获取音频文件
    private AudioClip GetAudio(string path)
    {
        if(!audioClips.ContainsKey(path))
        {
            audioClips.Add(path, LoadAudio(path));
        }
        return audioClips[path];
    }
    public void PlayBGM(string name,float volume=1.0f,bool isLoop=true)
    {
        audioSource.Stop();
        audioSource.clip = GetAudio(name);
        audioSource.loop = isLoop;
        audioSource.Play();
    }
    public void StopBGM()
    {
        audioSource.Stop();
    }
    // 播放音效
    public void PlaySound(string path,float volume=1.0f)
    {
        audioSource.PlayOneShot(LoadAudio(path), volume);
    }
    public void PlaySound(AudioSource audioSource,string path,float volume = 1.0f)
    {
        audioSource.PlayOneShot(LoadAudio(path),volume);
    }
}
