using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    private AudioSource _bgmSource;
    private List<AudioSource> _sfxAudio;

    [SerializeField] private AudioClip _bgm;

    private int sfxChannel = 8;

    private void Awake()
    {
        _sfxAudio = new();

        CreateAudioSources(sfxChannel);
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

    public void PlaySFX(string path, bool isOneShot = false)
    {
        // TODO: Run with pooling in resource manager
        AudioClip clip = Resources.Load<AudioClip>(path);

        if (isOneShot)
        {
            PlaySFXOneShot(clip);
        }
        else
        {
            PlaySFX(clip);
        }
    }

    private void PlaySFX(AudioClip clip)
    {

        foreach (var audio in _sfxAudio)
        {

            if (audio.isPlaying)
            {
                if (audio.clip == clip) return;
            }
            else
            {
                audio.volume = GameManager.Instance.Option.SfxVolume;
                audio.clip = clip;
                audio.Play();

                return;
            }
        }
    }

    private void PlaySFXOneShot(AudioClip clip)
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

    public void StopSFX(string path)
    {
        // TODO: Run with pooling in resource manager
        AudioClip clip = Resources.Load<AudioClip>(path);

        foreach (var audio in _sfxAudio)
        {
            if (audio.clip == clip)
            {
                audio.Stop();

                return;
            }
        }
    }
}