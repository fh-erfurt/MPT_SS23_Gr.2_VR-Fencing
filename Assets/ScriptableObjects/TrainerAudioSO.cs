using UnityEngine;


[CreateAssetMenu(fileName = "TrainerAudioSO", menuName = "ScriptableObjects/TrainerAudioSO")]
public class TrainerAudioSO : ScriptableObject {

    [Header("General")]
    public AudioClip[] audioClips;

    [Header("(for training) Attack")]
    public AudioClip[] attackClips;

    [Header("(for training) Failure")]
    public AudioClip[] failureClips;

    [Header("(for training) Compliment")]
    public AudioClip[] complimentClips;
}