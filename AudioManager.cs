using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic; // Assign in Inspector
    public AudioClip inGameMusic; // Assign in Inspector
    public AudioClip[] sfxClips;      // Array for various sound effects

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        PlayMusic(backgroundMusic);
    }

    public void PlayGameMusic()
    {
        PlayMusic(inGameMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null) {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(string clipName)
    {
        foreach (var clip in sfxClips) {
            if (clip.name == clipName) {
                sfxSource.PlayOneShot(clip);
                return;
            }
        }
        Debug.LogWarning($"Sound {clipName} not found!");
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null) musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
    }
}
