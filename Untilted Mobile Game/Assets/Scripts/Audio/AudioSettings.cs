using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public void ModifyMusicVolume(float musicLevel)
    {
        masterMixer.SetFloat("musicVol", musicSlider.value);
        Debug.Log(musicSlider.value);
    }

    public void ModifySFXVolume(float sfxLevel)
    {
        masterMixer.SetFloat("sfxVol", sfxSlider.value);
        Debug.Log(sfxSlider.value);
    }
}