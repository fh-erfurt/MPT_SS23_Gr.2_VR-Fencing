using UnityEngine;


public class AudioManager : MonoBehaviour {

    [Header("Audio Source (Trainer)")]
    public AudioSource audioSource;


    // Singleton
    public static AudioManager instance;

    private void Awake() {
        // Singleton
        if (instance == null) {
            instance = this;
        }
    }


    public void playClipAtTrainerPosition(AudioClip clip) {
        audioSource.clip = clip; 
        audioSource.Play();
    }


    public bool isAudioStillPlaying() {
        return audioSource.isPlaying;
    }
}