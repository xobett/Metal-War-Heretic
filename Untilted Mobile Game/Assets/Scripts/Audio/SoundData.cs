using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundData
{
    public string soundName;
    public AudioClip clip;
    public SoundType soundType = SoundType.SFX;
}

public enum SoundType
{
    SFX,
    Music
}