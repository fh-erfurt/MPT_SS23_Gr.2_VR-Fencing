using UnityEngine;


public class TrainingInstructionState : TrainingBaseState {

    // Audio
    private AudioManager audioManager;
    private AudioClip[] audioClips;

    private bool wasAudioPlayed = false;

    private int numberOfAudioClips;
    private int currentClip = 0;

    // Timer
    private float delayBetweenAudios = 2f;
    private float currentTimer = 0f;

    // Selection spheres
    private GameObject nextStateSpheres;
    private GameObject trainerPositionSpheres;

    // Next step
    private TrainingStateManager.nextStep nextStep = TrainingStateManager.nextStep.not_set;



    public override void EnterState(TrainingStateManager training,
                                    GameObject nextStateSpheres,
                                    GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres) {
        resetState();
        training.HideSelectionSpheres();
        this.nextStateSpheres = nextStateSpheres;
        this.trainerPositionSpheres = trainerPositionSpheres;
        trainerPositionSpheres.SetActive(true);
    }


    public override void UpdateState(TrainingStateManager training) {

        if (nextStep == TrainingStateManager.nextStep.next_state) {
            training.SwitchState(training.DeflectState);
            trainerPositionSpheres.transform.Find("Trainer_Position_Main").transform.GetChild(0).GetComponent<SwitchTrainerPosition>().SetTrainerToPosition();
        }

        if (nextStep == TrainingStateManager.nextStep.repeat_state) {
            training.SwitchState(training.InstructionState);
        }

        // when the last audio-clip was played only check for next step
        if (isLastClipPlayed()) {
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

        if (isLastClipPlayed()) {
            nextStateSpheres.SetActive(true);
        }
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