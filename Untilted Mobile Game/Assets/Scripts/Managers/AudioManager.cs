using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource[] audioSources = new AudioSource[10];

    void Start()
    {
        Start_InstantiateAudioSources();
    }

    #region START 

    private void Start_InstantiateAudioSources()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            GameObject go = new GameObject($"Audio Source {i + 1}");
            go.transform.parent = transform;
            AudioSource src = go.AddComponent<AudioSource>();
            audioSources[i] = src;
        }
    }

    #endregion START

    // Update is called once per frame
    void Update()
    {
        
    }
}
