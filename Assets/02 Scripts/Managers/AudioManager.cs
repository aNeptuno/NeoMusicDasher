using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    #region "Singleton"
    public static AudioManager Instance { get; private set; } // Singleton instance

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // If not, set it to this instance
            DontDestroyOnLoad(gameObject); // Make this instance persist across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy this instance if another instance already exists
        }
    }
    #endregion

    public Sound[] musicSounds, noteSounds, wrongSound, buttonSound;

    public AudioSource musicSource, noteSource, wrongSource, buttonSource;

    public int currentTrackIndex = 0;

    public float volumeMultiplier = 5.0f;

    public int lastNoteIndex = -1;

    void Start()
    {
        PlayTrack(currentTrackIndex);
    }

    void Update()
    {
        if (!musicSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    private void PlayTrack(int index)
    {
        if (index >= 0 && index < musicSounds.Length)
        {
            musicSource.clip = musicSounds[index].clip;
            musicSource.Play();
        }
    }

    public void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicSounds.Length;
        PlayTrack(currentTrackIndex);
    }

    public void PlayButton()
    {
        Sound s = buttonSound[0];
        if (s != null)
        {
            buttonSource.clip = s.clip;
            if (!buttonSource.isPlaying)
                buttonSource.Play();
        }
    }


    public void PlayNote()
    {
        int index = Random.Range(0, noteSounds.Length);

        if (lastNoteIndex  == -1)
            lastNoteIndex = index;
        else if (lastNoteIndex == index)
        {
            if (index == noteSounds.Length - 1)
                index--;
            else
                index++;
            lastNoteIndex = index;
        }

        Sound s = noteSounds[index];
        if (s != null)
        {
            noteSource.clip = s.clip;
            noteSource.volume = Mathf.Clamp(volumeMultiplier, 0f, 10f); // Ajustar volumen con multiplicador
            if (!noteSource.isPlaying)
                noteSource.Play();
        }
    }

    public void PlayWrong()
    {
        //MyDebugLog.Instance.MyDebugFunc("SFX: ",name);

        Sound s = wrongSound[0];
        if (s != null)
        {
            wrongSource.clip = s.clip;
            if (wrongSource.isPlaying) wrongSource.Stop();
                wrongSource.Play();
        }
    }

    #region  "Audio Controls"
    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    #endregion
}
