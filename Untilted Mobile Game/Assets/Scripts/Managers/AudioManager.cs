using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("AUDIO MIXER GROUPS")]
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    private AudioSource musicSource;
    private AudioSource[] sfxSources = new AudioSource[5];

    [Header("SOUND DATABASE")]
    [SerializeField] private SOAudioDatabase soundDatabase;

    [SerializeField] private float blendTime = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Start_InstantiateAudioSources();
    }

    #region START 

    private void Start_InstantiateAudioSources()
    {
        for (int i = 0; i < sfxSources.Length; i++)
        {
            GameObject sfxGo = new GameObject($"Sfx Source {i + 1}");
            sfxGo.transform.parent = transform.GetChild(0);
            AudioSource sfxSrc = sfxGo.AddComponent<AudioSource>();
            sfxSources[i] = sfxSrc;
            sfxSources[i].outputAudioMixerGroup = sfxMixerGroup;
            sfxSources[i].playOnAwake = false;
            sfxSources[i].loop = false;
        }

        GameObject musicGo = new GameObject("Music Source");
        musicGo.transform.parent = transform.GetChild(1);
        AudioSource musicSrc = musicGo.AddComponent<AudioSource>();
        musicSource = musicSrc;
        musicSource.outputAudioMixerGroup = musicMixerGroup;
        musicSource.playOnAwake = false;
        musicSource.loop = true;
    }

    private void Start_TestPlay()
    {
        soundDatabase.GetSound("TREN");

        sfxSources[0].clip = soundDatabase.GetSound("TREN").clip;

        Invoke(nameof(Play), 3f);
    }

    void Play()
    {
        PlaySFX("ELECTRICIDAD SUELO");
    }

    #endregion START

    public void PlayMusic(string soundName)
    {
        if (musicSource.isPlaying)
        {
            StartCoroutine(CR_BlendMusic(soundName));
        }
        else
        {
            musicSource.clip = GetClip(soundName);
            musicSource.Play();
        }
    }

    public void PlaySFX(string soundName)
    {
        AudioSource src = GetSource();

        if (src == null)
        {
            StartCoroutine(CR_BlendSounds(soundName));
        }
        else
        {
            src.clip = GetClip(soundName);
            src.Play();
        }
    }

    public void PlaySFX(string soundName, int fixedIndex)
    {
        AudioSource src = sfxSources[Random.Range(0, sfxSources.Length)];

        src.clip = GetClip(soundName);
        src.Play();
    }

    private AudioSource GetSource()
    {
        foreach (AudioSource source in sfxSources)
        {
            if (!source.isPlaying) return source;
        }

        return null;
    }

    public AudioClip GetClip(string soundName)
    {
        if (soundDatabase.GetSound(soundName) == null)
        {
            Debug.Log($"Sound \"{soundName}\" was not found");
            return null;
        }

        return soundDatabase.GetSound(soundName).clip;
    }

    private IEnumerator CR_BlendSounds(string soundName)
    {
        AudioSource source = sfxSources[Random.Range(0, sfxSources.Length)];

        //Fade out
        float timeFadeOut = 0;
        while (timeFadeOut < 1)
        {
            source.volume = Mathf.Lerp(source.volume, 0, timeFadeOut);
            timeFadeOut += Time.deltaTime * blendTime;
            yield return null;
        }

        source.clip = GetClip(soundName);
        source.Play();

        //Fade In
        float timeFadeIn = 0;
        while (timeFadeIn < 1)
        {
            source.volume = Mathf.Lerp(source.volume, 1, timeFadeIn);
            timeFadeIn += Time.deltaTime * blendTime;
            yield return null;
        }

        yield break;
    }

    private IEnumerator CR_BlendMusic(string soundName)
    {
        //Fade out
        float timeFadeOut = 0;
        while (timeFadeOut < 1)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, 0, timeFadeOut);
            timeFadeOut += Time.deltaTime * blendTime;
            yield return null;
        }

        musicSource.clip = GetClip(soundName);
        musicSource.Play();

        //Fade In
        float timeFadeIn = 0;
        while (timeFadeIn < 1)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, 1, timeFadeIn);
            timeFadeIn += Time.deltaTime * blendTime;
            yield return null;
        }

        yield break;
    }
}
