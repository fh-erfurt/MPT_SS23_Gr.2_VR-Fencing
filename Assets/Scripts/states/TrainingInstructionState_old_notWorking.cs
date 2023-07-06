using UnityEngine;


public class TrainingInstructionState_old_notWorking : TrainingBaseState {

    // Audio
    private AudioManager audioManager;
    private AudioClip[] audioClips;

    private bool wasAudioPlayed = false;

    private int numberOfAudioClips;
    private int currentAudio = 0;

    // Animation
    private string[] animationTriggers = { "Show_Deflect_R_O", "Show_Deflect_L_O" };
    private string[] animationNames = { "Verteidigung R o", "Verteidigung L o" };
    private int currentAnimation = 0;
    private bool wasAnimationTriggered = false;
    private bool wasAnimationPlayed = false;
    private bool idlePlayed = false;

    // Selection spheres
    private GameObject nextStateSpheres;
    private GameObject trainerPositionSpheres;

    // Trainer animator
    private Animator animator;

    // Next step
    private TrainingStateManager.nextStep nextStep = TrainingStateManager.nextStep.not_set;



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

        // play introduction audio
        //playAudio();
    }

    // called once per frame from TrainingStateManager
    public override void UpdateState(TrainingStateManager training) {

        // when the last audio-clip was played only check for next step
        if (isLastAudioClipPlayed()) {
            checkNextState(training);
            return;
        }

        if (!wasAudioPlayed) {
            // playAudio();
            // return;
        }

        if (audioManager.isAudioStillPlaying()) {
            return;
        }

        if (!wasAnimationTriggered) {
            triggerAnimation();
            return;
        }

        if (isAnimationStillPlaying()) {
            return;
        }

        wasAudioPlayed = false;
        wasAnimationTriggered = false;
        // temp
        currentAudio++;

        Debug.Log("End of Update");

        if (isLastAudioClipPlayed()) {
            nextStateSpheres.SetActive(true);
            wasAnimationTriggered = true;
            wasAnimationPlayed = false;
        }
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

    //
    // Check for next state
    private void checkNextState(TrainingStateManager training) {
        if (nextStep == TrainingStateManager.nextStep.next_state) {
            training.SwitchState(training.DeflectState);
            trainerPositionSpheres.transform.Find("Trainer_Position_Main").transform.GetChild(0).GetComponent<SwitchTrainerPosition>().SetTrainerToPosition();
        }

        if (nextStep == TrainingStateManager.nextStep.repeat_state) {
            training.SwitchState(training.InstructionState);
        }
    }

    //
    // Audio
    private void playAudio() {
        audioManager.playClipAtTrainerPosition(audioClips[currentAnimation]);
        wasAudioPlayed = true;
        currentAudio++;
    }

    public override void SetNextStep(TrainingStateManager.nextStep nextStep) {
        this.nextStep = nextStep;
    }

    private bool isLastAudioClipPlayed() {
        return currentAudio == numberOfAudioClips;
    }

    //
    // Animation
    private void triggerAnimation() {
        Debug.Log("Play Animation");
        animator.SetTrigger(animationTriggers[currentAnimation]);
        wasAnimationTriggered = true;
        currentAnimation++;
    }

    private bool isAnimationStillPlaying() {
        // if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
        //     if (idlePlayed) return false;
        //     if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f){
        //         idlePlayed = true;
        //     }

        //     return true;
        // }

        // check if in idle
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !wasAnimationPlayed) {
            return true;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f && animator.GetCurrentAnimatorStateInfo(0).IsName(animationNames[currentAnimation-1])) { // || animator.IsInTransition(0)) {
            wasAnimationPlayed = true;
            return true;
        }
        else {
            return false;
        }
    }
}