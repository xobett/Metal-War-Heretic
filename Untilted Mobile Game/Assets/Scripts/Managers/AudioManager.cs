using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("AUDIO MIXER GROUPS")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    private AudioSource[] sfxSources = new AudioSource[5];

    [Header("SOUND DATABASE")]
    [SerializeField] private SOAudioDatabase soundDatabase;

    [SerializeField] private float blendTime = 0.6f;

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
            GameObject go = new GameObject($"Audio Source {i + 1}");
            go.transform.parent = transform;
            AudioSource src = go.AddComponent<AudioSource>();
            sfxSources[i] = src;
            sfxSources[i].outputAudioMixerGroup = sfxMixerGroup; 
        }
    }

    private void Start_TestPlay()
    {
        soundDatabase.GetSound("TREN");

        sfxSources[0].clip = soundDatabase.GetSound("TREN").clip;

        Invoke(nameof(Play), 3f);
    }

    void Play()
    {
        PlaySound("ELECTRICIDAD SUELO");
    }

    #endregion START

    public void PlaySound(string soundName)
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

    public void PlaySound(string soundName, int fixedIndex)
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
        float timeFadeOut = 1;
        while (timeFadeOut > 0)
        {
            source.volume = Mathf.Lerp(source.volume, 0, timeFadeOut);
            timeFadeOut -= Time.deltaTime * blendTime;
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
}
