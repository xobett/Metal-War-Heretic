using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Sound Database", menuName ="Scriptable Object/Audio/Sound Database")]
public class SOAudioDatabase : ScriptableObject
{
    public List<SoundData> sounds = new List<SoundData>();

    public SoundData GetSound(string soundName)
    {
        return sounds.Find(sound => sound.soundName == soundName);
    }
}