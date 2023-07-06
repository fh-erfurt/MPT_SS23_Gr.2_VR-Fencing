using UnityEngine;


public class TrainingDeflectState : TrainingBaseState {

    // Training schedule
    private string[] trainingPlan = {   //ATTACK_R, ATTACK_R,
                                        ATTACK_L, ATTACK_L,
                                        ATTACK_R, ATTACK_L };
    private int trainingPlanIndex = 0;

    // State of the state
    private enum state { intro, training, end };
    private state currentState = state.intro;

    // Animation States
    private const string IDLE = "Idle";
    private const string ATTACK_R = "Attack_R";
    private const string ATTACK_L = "Attack_L";
    private const string ATTACK_SUCCESS_R = "Attack_R_Success";
    private const string ATTACK_SUCCESS_L = "Attack_L_Success";

    // Animation
    private string[] animationsGeneral = { IDLE };
    private string[] animationsAttack  = { ATTACK_R, ATTACK_L };
    private string[] animationsBlockSuccess = { ATTACK_SUCCESS_R, ATTACK_SUCCESS_L };
    private bool wasAnimationPlayed = false;
    private string currentAnimationState;

    // Audio
    private AudioManager audioManager;
    private AudioClip[] audioClipsGeneral;
    private AudioClip[] audioClipsAttack;
    private AudioClip[] audioClipsFailure;
    private AudioClip[] audioClipsCompliment;
    private bool wasAudioPlayed = false;

    // Trainer animator
    private Animator animator;

    // Sword
    private bool successfulBlock = false;



    public override void EnterState(TrainingStateManager training, GameObject nextStateSpheres, GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres, Animator trainerAnimator) {
        resetState();

        // hide spheres which change the trainer position
        training.HideSelectionSpheres();

        animator = trainerAnimator;

        // default animation state
        changeAnimationState(IDLE);
    }


    //
    // called once per frame from TrainingStateManager
    public override void UpdateState(TrainingStateManager training) {

        // wait until audio is done playing
        if (isAudioStillPlaying()) {
            return;
        }

        // wait until animation is done playing
        if (isAnimationStillPlaying()) {
            return;
        }

        // Intro
        if (currentState == state.intro) {
            playStartAudio();
            currentState = state.training;
            return;
        }

        // Training
        if (currentState == state.training) {
            executeTrainingPlan(training);
            return;
        }

        // End
        if (currentState == state.end) {
            training.SwitchState(training.EndState);
            return;
        }
    }


    //
    // Training Plan
    private void executeTrainingPlan(TrainingStateManager training) {

        short audioAndAnimationIndex = -1;

        // when last step in traininPlan was reached
        if (trainingPlanIndex == trainingPlan.Length) {
            currentState = state.end;
            return;
        }

        // set index to play the correct audio & animation for the trainer
        switch (trainingPlan[trainingPlanIndex]) {
            case ATTACK_R:
                audioAndAnimationIndex = 0;
                break;
            case ATTACK_L:
                audioAndAnimationIndex = 1;
                break;
        }


        // play audio once
        if (!wasAudioPlayed) {
            playSpecificAudio(audioClipsAttack[audioAndAnimationIndex]);
            wasAudioPlayed = true;
            // break into UpdateState() and wait until audio is finished
            return;
        };

        // play animation once
        if (!wasAnimationPlayed) {
            changeAnimationState(animationsAttack[audioAndAnimationIndex]);
            wasAnimationPlayed = true;
            // break into UpdateState() and wait until animation is finished
            return;
        };

        // reset for next trainer attack
        prepareNextAttack(training);
    }


    private void prepareNextAttack(TrainingStateManager training) {
        training.resetTrainerPosition();
        changeAnimationState(IDLE);
        trainingPlanIndex++;
        wasAudioPlayed = false;
        wasAnimationPlayed = false;
    }


    //
    // State functions
    private void resetState() {
        trainingPlanIndex = 0;
        wasAudioPlayed = false;
        wasAnimationPlayed = false;
        currentState = state.intro;
    }

    public override void SetAudios(AudioManager audioManager, TrainerAudioSO trainerAudioSO) {
        this.audioManager = audioManager;
        audioClipsGeneral = trainerAudioSO.audioClips;
        audioClipsAttack = trainerAudioSO.attackClips;
        audioClipsFailure = trainerAudioSO.failureClips;
        audioClipsCompliment = trainerAudioSO.complimentClips;
    }

    public override void SetNextStep(TrainingStateManager.nextStep nextStep) {}


    //
    // Audio
    private void playSpecificAudio(AudioClip audioClip) {
        audioManager.playClipAtTrainerPosition(audioClip);
    }

    private bool isAudioStillPlaying() {
        return audioManager.isAudioStillPlaying();
    }

    private void playStartAudio() {
        playSpecificAudio(audioClipsGeneral[0]);
    }


    //
    // Animation
    private void changeAnimationState(string newState) {
        // stop the same animation from interrupting itself
        if (currentAnimationState == newState) {
            return;
        }
        // play animation
        animator.Play(newState);
        // reassign the current state
        currentAnimationState = newState;
    }


    private bool isAnimationStillPlaying() {

        if (isCurrentStateIdle() && !animator.IsInTransition(0)) {
            return false;
        }

        return !isCurrentStateIdle();
    }


    private bool isCurrentStateIdle() {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(IDLE);
    }
}