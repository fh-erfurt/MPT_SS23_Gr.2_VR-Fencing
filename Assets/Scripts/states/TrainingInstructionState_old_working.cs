using UnityEngine;


public class TrainingInstructionState_old_working : TrainingBaseState {

    // Audio
    private AudioManager audioManager;
    private AudioClip[] audioClips;

    private bool wasAudioPlayed = false;

    private int numberOfAudioClips;
    private int currentAudio = 0;

    // Animation
    private string[] animationTriggers = {"Greetings_Idle", "Show_Deflect_R_O", "Show_Deflect_L_O", "Greetings_Idle" };
    private string[] animationNames    = {"Greetings_Idle", "Verteidigung R o", "Verteidigung L o", "Greetings_Idle" };
    private int currentAnimation = 0;
    private bool wasAnimationTriggered = false;

    // Selection spheres
    private GameObject nextStateSpheres;
    private GameObject trainerPositionSpheres;

    // Trainer animator
    private Animator animator;

    // Next step
    private TrainingStateManager.nextStep nextStep = TrainingStateManager.nextStep.not_set;

    // State stuff
    private bool readyForNextInstruction = true;



    // called once when entering the state
    public override void EnterState(TrainingStateManager training,
                                    GameObject nextStateSpheres,
                                    GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres,
                                    Animator trainerAnimator) {
        resetState();

        training.HideSelectionSpheres();
        this.nextStateSpheres = nextStateSpheres;
        this.trainerPositionSpheres = trainerPositionSpheres;
        trainerPositionSpheres.SetActive(true);

        animator = trainerAnimator;
    }


    // called once per frame from TrainingStateManager
    public override void UpdateState(TrainingStateManager training) {

        // when the last audio-clip was played only check for next step
        if (isLastAudioClipPlayed()) {
            if (!isAudioStillPlaying() && !isAnimationStillPlaying(currentAnimation-1)) {
                nextStateSpheres.SetActive(true);
                checkNextState(training);
            }
            return;
        }


        if (readyForNextInstruction) {
            readyForNextInstruction = false;
            playAudio();
            triggerAnimation();
        }

        // check if an animation was triggered that is not the default "idle"-state
        if (wasAnimationTriggered && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            return;
        }

        // check if the animation is done playing
        if(isAnimationStillPlaying(currentAnimation)) {
            wasAnimationTriggered = false;
            return;
        }

        // check if the audio is done playing
        if(isAudioStillPlaying()) {
            return;
        }

        prepareNextInstruction();
    }


    private void prepareNextInstruction() {
        currentAnimation++;
        currentAudio++;
        readyForNextInstruction = true;
    }


    //
    // State functions
    private void resetState() {
        currentAudio = 0;
        wasAudioPlayed = false;
        currentAnimation = 0;
        wasAnimationTriggered = false;
        nextStep = TrainingStateManager.nextStep.not_set;
    }

    public override void SetAudios(AudioManager audioManager, TrainerAudioSO trainerAudioSO) {
        this.audioManager = audioManager;
        audioClips = trainerAudioSO.audioClips;
        numberOfAudioClips = audioClips.Length;
    }

    public override void SetNextStep(TrainingStateManager.nextStep nextStep) {
        this.nextStep = nextStep;
    }


    //
    // Check for next state
    private void checkNextState(TrainingStateManager training) {
        // continue to training
        if (nextStep == TrainingStateManager.nextStep.next_state) {
            training.SwitchState(training.DeflectState);
            trainerPositionSpheres.transform.Find("Trainer_Position_Main").transform.GetChild(0).GetComponent<SwitchTrainerPosition>().SetTrainerToPosition();
        }
        // repeat instructions
        if (nextStep == TrainingStateManager.nextStep.repeat_state) {
            training.SwitchState(training.InstructionState);
        }
    }


    //
    // Audio
    private void playAudio() {
        audioManager.playClipAtTrainerPosition(audioClips[currentAudio]);
        wasAudioPlayed = true;
    }

    private bool isAudioStillPlaying() {
        return audioManager.isAudioStillPlaying();
    }

    private bool isLastAudioClipPlayed() {
        return currentAudio == numberOfAudioClips;
    }


    //
    // Animation
    private void triggerAnimation() {
        animator.SetTrigger(animationTriggers[currentAnimation]);
        wasAnimationTriggered = true;
    }

    private bool isAnimationStillPlaying(int currentAnimation) {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationNames[currentAnimation]);
    }

    private bool isCurrentStateIdle() {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
    }
}