using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider soundSlider;
    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f); //.75 default value
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.75f);
        mixer.SetFloat("MusicVol", Mathf.Log10(musicSlider.value) * 20);
        mixer.SetFloat("SoundVol", Mathf.Log10(soundSlider.value) * 20);
    }
    public void SetMusicLevel(float sliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }
    public void SetSoundLevel(float sliderValue)
    {
        mixer.SetFloat("SoundVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SoundVolume", sliderValue);
    }
}