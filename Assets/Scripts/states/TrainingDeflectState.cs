using UnityEngine;


public class TrainingDeflectState : TrainingBaseState {

    // Audio
    private AudioManager audioManager;
    private AudioClip[] audioClips;

    private bool wasAudioPlayed = false;

    private int numberOfAudioClips;
    private int currentClip = 0;

    // Timer
    private float delayBetweenAudios = 3f;
    private float currentTimer = 0f;

    // Next step
    private TrainingStateManager.nextStep nextStep = TrainingStateManager.nextStep.not_set;



    public override void EnterState(TrainingStateManager training,
                                    GameObject nextStateSpheres,
                                    GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres) {
        resetState();
        training.HideSelectionSpheres();
    }


    public override void UpdateState(TrainingStateManager training) {

        // when the last audio-clip was played finish training
        if (isLastClipPlayed()) {
            training.SwitchState(training.EndState);
            return;
        }


        if (currentTimer <= delayBetweenAudios) {
            currentTimer += Time.deltaTime;
            return;
        }

        if (!wasAudioPlayed) {
            audioManager.playClipAtTrainerPosition(audioClips[currentClip]);
            wasAudioPlayed = true;
        }

        if (audioManager.isAudioStillPlaying()) {
            return;
        }

        currentClip++;
        currentTimer = 0f;
        wasAudioPlayed = false;
    }


    public override void SetAudios(AudioManager audioManager, TrainerAudioSO trainerAudioSO) {
        this.audioManager = audioManager;
        audioClips = trainerAudioSO.audioClips;
        numberOfAudioClips = audioClips.Length;
    }


    public override void SetNextStep(TrainingStateManager.nextStep nextStep) {
        this.nextStep = nextStep;
    }



    private void resetState() {
        currentTimer = 0f;
        currentClip = 0;
        wasAudioPlayed = false;
        nextStep = TrainingStateManager.nextStep.not_set;
    }


    private bool isLastClipPlayed() {
        return currentClip == numberOfAudioClips;
    }
}
