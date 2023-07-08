using UnityEngine;


public class TrainingStartState : TrainingBaseState {

    // Audio
    private AudioManager audioManager;
    private AudioClip[] audioClips;

    private bool wasAudioPlayed = false;

    // Timer
    private float delayBeforeAudioStarts = 0f;
    private float currentTimer = 0f;

    // Selection spheres
    private GameObject skipInstructionSpheres;

    // Trainer animator
    private Animator animator;

    // Next step
    private TrainingStateManager.nextStep nextStep = TrainingStateManager.nextStep.not_set;

    // Points
    private Points points;


    public override void EnterState(TrainingStateManager training,
                                    GameObject nextStateSpheres,
                                    GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres,
                                    Animator trainerAnimator) {
        resetState();

        training.setTotalScoreCanvasInactive();

        if (points == null) {
            points = Points.instance;
        }
        // reset points on start
        points.ResetPoints();

        training.hideSelectionSpheres();

        training.setCurrentAction("Introduction");

        this.skipInstructionSpheres = skipInstructionSpheres;

        animator = trainerAnimator;
    }


    public override void UpdateState(TrainingStateManager training) {

        if (nextStep == TrainingStateManager.nextStep.next_state) {
            training.SwitchState(training.InstructionState);
            return;
        }

        if (skipInstructionSpheres.activeSelf) {
            return;
        }


        if (currentTimer <= delayBeforeAudioStarts) {
            currentTimer += Time.deltaTime;
            return;
        }

        if (!wasAudioPlayed) {
            audioManager.playClipAtTrainerPosition(audioClips[0]);
            wasAudioPlayed = true;
        }

        if (audioManager.isAudioStillPlaying()) {
            return;
        }

        skipInstructionSpheres.SetActive(true);
    }


    public override void SetAudios(AudioManager audioManager, TrainerAudioSO trainerAudioSO) {
        this.audioManager = audioManager;
        audioClips = trainerAudioSO.audioClips;
    }


    public override void SetNextStep(TrainingStateManager.nextStep nextStep) {
        this.nextStep = nextStep;
    }



    private void resetState() {
        currentTimer = 0f;
        wasAudioPlayed = false;
        nextStep = TrainingStateManager.nextStep.not_set;
    }
}