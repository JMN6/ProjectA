using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    private const int SFXCHANNEL = 8;

    private AudioSource _bgmSource;
    private List<AudioSource> _sfxAudio;

    [SerializeField] private AudioClip _bgm;

    private void Awake()
    {
        _sfxAudio = new();

        CreateAudioSources(SFXCHANNEL);
    }

    private void Start()
    {
        PlayBGM();
    }

    private void CreateAudioSources(int size)
    {
        var rootObj = new GameObject("Audio").transform;
        rootObj.SetParent(transform);

        _bgmSource = new GameObject($"Music").AddComponent<AudioSource>();
        _bgmSource.transform.SetParent(rootObj);
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;

        for (int i = 0; i < size; i++)
        {
            var obj = new GameObject($"Audio{i}").AddComponent<AudioSource>();
            obj.transform.SetParent(rootObj);
            obj.playOnAwake = false;
            _sfxAudio.Add(obj);
        }
    }

    public void PlayBGM()
    {
        _bgmSource.Stop();
        _bgmSource.volume = GameManager.Instance.Option.BgmVolume;
        _bgmSource.clip = _bgm;
        _bgmSource.Play();
    }

    public void SetBGMVolume(float volume)
    {
        _bgmSource.volume = volume;
    }

    public void PlayOneShot(AudioClip clip)
    {
        foreach (var audio in _sfxAudio)
        {
            if (!audio.isPlaying)
            {
                audio.volume = GameManager.Instance.Option.SfxVolume;
                audio.PlayOneShot(clip);
                return;
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        foreach (var audio in _sfxAudio)
        {
            if (!audio.isPlaying)
            {
                audio.volume = GameManager.Instance.Option.SfxVolume;
                audio.clip = clip;
                audio.Play();
                return;
            }
        }
    }
}