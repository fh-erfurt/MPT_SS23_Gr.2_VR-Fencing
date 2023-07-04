using UnityEngine;


[CreateAssetMenu(fileName = "TrainerAudioSO", menuName = "ScriptableObjects/TrainerAudioSO")]
public class TrainerAudioSO : ScriptableObject {

    [Header("Audio Clips")]
    public AudioClip[] audioClips;
}