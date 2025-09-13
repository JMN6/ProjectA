using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    public class SoundInfo
    {
        public bool isPlaying { get; set; }
        public Coroutine coroutine { get; set; }
        public AudioSource audioSource { get; set; }
    }

    private AudioSource _bgmSource;
    private List<AudioSource> _sfxAudio;

    [SerializeField] private AudioClip _bgm;

    private int sfxChannel = 8;

    private Dictionary<string, SoundInfo> _randomLoopSoundCorotines = new Dictionary<string, SoundInfo>();

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
        if (_randomLoopSoundCorotines.ContainsKey(path) == true &&
            _randomLoopSoundCorotines[path].isPlaying == true)
        {
            StopSFXRandomLoop(path);
            return;
        }

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

    private void StopSFXRandomLoop(string path)
    {
        _randomLoopSoundCorotines[path].isPlaying = false;

        StopCoroutine(_randomLoopSoundCorotines[path].coroutine);
        _randomLoopSoundCorotines[path].coroutine = null;

        _randomLoopSoundCorotines[path].audioSource?.Stop();
        _randomLoopSoundCorotines[path].audioSource = null;
    }

    public void PlaySFXRandom(string  path, int randomCount, bool isOneShot = false)
    {
        if (isOneShot)
        {
            PlaySFX(path + Random.Range(1, randomCount + 1), isOneShot);
        }
        else
        {
            if(_randomLoopSoundCorotines.ContainsKey(path))
            {
                if (_randomLoopSoundCorotines[path].isPlaying == true)
                    return;

                _randomLoopSoundCorotines[path].isPlaying = true;
                _randomLoopSoundCorotines[path].coroutine = StartCoroutine(CoPlaySFXRandom(path, randomCount));
            }
            else
            {
                _randomLoopSoundCorotines.Add(path, new SoundInfo());
                _randomLoopSoundCorotines[path].isPlaying = true;
                _randomLoopSoundCorotines[path].coroutine = StartCoroutine(CoPlaySFXRandom(path, randomCount));
                _randomLoopSoundCorotines[path].audioSource = null;
            }
        }
    }

    private IEnumerator CoPlaySFXRandom(string path, int randomCount)
    {
        while (true)
        {
            bool isPlayed = false;

            foreach (var audio in _sfxAudio)
            {
                if (!audio.isPlaying)
                {
                    isPlayed = true;
                    audio.volume = GameManager.Instance.Option.SfxVolume;

                    // TODO: Run with pooling in resource manager
                    AudioClip clip = Resources.Load<AudioClip>(path + Random.Range(1, randomCount + 1));

                    audio.PlayOneShot(clip);
                    _randomLoopSoundCorotines[path].audioSource = audio;
                    yield return new WaitForSeconds(clip.length);
                }
            }

            if(isPlayed == false)
            {
                _randomLoopSoundCorotines[path].audioSource = null;
                yield return null;
            }
        }
    }
}