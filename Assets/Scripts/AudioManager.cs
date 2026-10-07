using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;
    public AudioSource sfxSource;
    public AudioSource footStep;
    public AudioSource slideSound;

    public AudioClip backgroundMusic;
    public AudioClip jumpSFX;
    public AudioClip runSFX;
    public AudioClip slideSFX;
    public AudioClip fallSFX;
    public AudioClip gameoverSFX;
    public AudioClip clickSFX;


    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        if (audioSource != null) audioSource.volume = musicVol;
        if (sfxSource != null) sfxSource.volume = sfxVol;

        if(footStep != null && runSFX != null)
        {
            footStep.clip = runSFX;
            footStep.loop = true;
            footStep.volume = sfxVol;
        }

        if(slideSound != null && slideSFX != null)
        {
            slideSound.clip = slideSFX;
            slideSound.loop = true;
            slideSound.volume = sfxVol;
        }

        PlayBGM();
    }


    public void PlayBGM()
    {
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void pauseBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void resumBGM()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }

    public void StopBGM()
    {
        audioSource.Stop();
    }

    public void PlayJumpSFX()
    {
        sfxSource.time = 0.1f;
        sfxSource.PlayOneShot(jumpSFX);
    }

    public void PlayRunSFX()
    {
        if (footStep != null && !footStep.isPlaying)
        {
            footStep.Play();
        }
    }

    public void StopRunSFX()
    {
        if (footStep != null && footStep.isPlaying)
        {
            footStep.Stop();
        }
    }

    public void PlaySlideSFX()
    {
        if(slideSound != null && !slideSound.isPlaying)
        {
            slideSound.Play();
        }
    }

    public void StopSlideSFX()
    {
        if(slideSound != null && slideSound.isPlaying)
        {
            slideSound.Stop();
        }
    }

    public void PlayGameOverSFX()
    {
        sfxSource.PlayOneShot(gameoverSFX);
    }

    public void PlayFallSFX()
    {
        sfxSource.PlayOneShot(fallSFX);
    }

    public void PlayButtonClickSFX()
    {
        if (sfxSource != null && clickSFX != null)
        {
            sfxSource.clip = clickSFX;
            sfxSource.time = 0.4f;
            sfxSource.Play();
        }
    }
}
