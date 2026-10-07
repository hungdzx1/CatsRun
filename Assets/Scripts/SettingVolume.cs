using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingVolume : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sfxSlider;
    public static SettingVolume instance;


    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        InitVolume();
    }

    public void InitVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        if(volumeSlider != null) volumeSlider.value = savedVolume;
        if(sfxSlider != null) sfxSlider.value = savedSFX;
        
        SetVolumeMusic(savedVolume);
        SetVolumeSFX(savedSFX);
    }   

    public void SetVolumeMusic(float volume)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.audioSource.volume = volume;
        }
        
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetVolumeSFX(float volume)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.sfxSource.volume = volume;
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}
