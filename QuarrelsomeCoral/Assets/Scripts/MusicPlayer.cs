using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource _audioSource;

    public AudioClip pratakasLoop = null;
    public AudioClip fantasyLoop = null;

    private static MusicPlayer instance = null;

    public static MusicPlayer Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        } else {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
        _audioSource = GetComponent<AudioSource>();

        PlayMusic();
    }

    public void PlayMusic()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }

    public void UpdateVolume(float vol) {
        _audioSource.volume = vol;
    }

    public void ChangeSong(string name) {
        if (name == "Fantasy") _audioSource.clip = fantasyLoop;
        if (name == "Pratakas") _audioSource.clip = pratakasLoop;
        _audioSource.Play();
    }
}
