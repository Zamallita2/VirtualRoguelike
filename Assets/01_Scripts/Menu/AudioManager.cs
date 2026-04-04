using UnityEngine;

public class AudioManager : MonoBehaviour
{
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

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        // Reutilizar los AudioSources que ya existen en el Inspector
        AudioSource[] existing = GetComponents<AudioSource>();

        musicSource = existing.Length >= 1 ? existing[0] : gameObject.AddComponent<AudioSource>();
        sfxSource = existing.Length >= 2 ? existing[1] : gameObject.AddComponent<AudioSource>();

        // ✅ CORRECCIÓN: NO llamar Stop() - solo configurar propiedades
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;

        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f;
        sfxSource.volume = sfxVolume;

        Debug.Log("[AudioManager] ✅ Inicializado correctamente");
    }

    // ── Se llama desde MenuManager.OnSurfaceDetected → ShowMenuSequence ──
    public void PlayAmbientMusic()
    {
        Debug.Log("[AudioManager] 🎵 PlayAmbientMusic() LLAMADO");

        if (musicSource == null)
        {
            Debug.LogError("[AudioManager] ❌ musicSource es NULL - Esto no debería pasar");
            return;
        }

        if (ambientMusic == null)
        {
            Debug.LogError("[AudioManager] ❌❌❌ ambientMusic es NULL");
            Debug.LogError("[AudioManager] SOLUCIÓN: Arrastra el clip de música medieval al campo 'Ambient Music' en el Inspector del AudioManager");
            return;
        }

        Debug.Log($"[AudioManager] Clip asignado: {ambientMusic.name}");
        Debug.Log($"[AudioManager] MusicSource estado - Clip: {musicSource.clip?.name ?? "null"}, Playing: {musicSource.isPlaying}, Volume: {musicSource.volume}");

        // ✅ CORRECCIÓN: Verificar que no esté sonando el mismo clip
        if (musicSource.isPlaying && musicSource.clip == ambientMusic)
        {
            Debug.Log("[AudioManager] 🎵 La música ya está sonando - NO se reinicia");
            return;
        }

        musicSource.clip = ambientMusic;
        musicSource.volume = musicVolume;
        musicSource.loop = true;

        Debug.Log($"[AudioManager] ▶️ REPRODUCIENDO música: {ambientMusic.name} (Volume: {musicVolume})");
        musicSource.Play();

        // Verificar después de 0.1 segundos
        StartCoroutine(VerifyMusicPlaying());
    }

    System.Collections.IEnumerator VerifyMusicPlaying()
    {
        yield return new WaitForSeconds(0.1f);

        if (musicSource.isPlaying)
        {
            Debug.Log($"[AudioManager] ✅✅✅ MÚSICA CONFIRMADA SONANDO - Clip: {musicSource.clip.name}, Time: {musicSource.time}");
        }
        else
        {
            Debug.LogError($"[AudioManager] ❌❌❌ MÚSICA NO ESTÁ SONANDO después de Play()");
            Debug.LogError($"[AudioManager] Estado - Enabled: {musicSource.enabled}, Mute: {musicSource.mute}, Volume: {musicSource.volume}");
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            Debug.Log("[AudioManager] 🔇 Música detenida");
        }
    }

    public void PlayHoverSound()
    {
        if (buttonHoverSound != null)
            sfxSource.PlayOneShot(buttonHoverSound, sfxVolume * 0.4f);
    }

    public void PlayButtonClick()
    {
        if (buttonClickSound != null)
            sfxSource.PlayOneShot(buttonClickSound, sfxVolume);
        else
            Debug.LogWarning("[AudioManager] ⚠️ buttonClickSound no asignado en el Inspector");
    }

    public void PlayZoomSound()
    {
        if (zoomSound != null)
            sfxSource.PlayOneShot(zoomSound, sfxVolume);
    }

    public void PlayDoorOpenSound()
    {
        if (doorOpenSound == null)
        {
            Debug.LogWarning("[AudioManager] ⚠️ doorOpenSound no asignado en el Inspector");
            return;
        }
        sfxSource.PlayOneShot(doorOpenSound, Mathf.Min(sfxVolume * 1.2f, 1f));
        Debug.Log("[AudioManager] 🚪 Sonido puerta reproducido");
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        if (musicSource) musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        if (sfxSource) sfxSource.volume = sfxVolume;
    }
}