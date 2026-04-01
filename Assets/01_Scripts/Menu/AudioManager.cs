// Script: AudioManager.cs

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    private AudioSource musicSource;
    private AudioSource sfxSource;

    [Header("Music")]
    public AudioClip ambientMusic;

    [Header("Sound Effects")]
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;
    public AudioClip zoomSound;
    public AudioClip doorOpenSound;

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    void Awake()
    {
        // Crear dos AudioSources
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
        sfxSource.playOnAwake = false;
    }

    public void PlayAmbientMusic()
    {
        if (ambientMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = ambientMusic;
            musicSource.Play();
        }
    }

    public void PlayHoverSound()
    {
        if (buttonHoverSound != null)
        {
            sfxSource.PlayOneShot(buttonHoverSound, sfxVolume * 0.5f);
        }
    }

    public void PlayButtonClick()
    {
        if (buttonClickSound != null)
        {
            sfxSource.PlayOneShot(buttonClickSound, sfxVolume);
        }
    }

    public void PlayZoomSound()
    {
        if (zoomSound != null)
        {
            sfxSource.PlayOneShot(zoomSound, sfxVolume);
        }
    }

    public void PlayDoorOpenSound()
    {
        if (doorOpenSound != null)
        {
            sfxSource.PlayOneShot(doorOpenSound, sfxVolume * 1.2f);
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }
}