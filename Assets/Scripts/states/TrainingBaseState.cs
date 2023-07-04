using UnityEngine;


public abstract class TrainingBaseState {

    public abstract void EnterState(TrainingStateManager training,
                                    GameObject nextStateSpheres,
                                    GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres);

    public abstract void UpdateState(TrainingStateManager training);

    public abstract void SetAudios(AudioManager audioManager, TrainerAudioSO trainerAudioSO);

    public abstract void SetNextStep(TrainingStateManager.nextStep nextStep);
}