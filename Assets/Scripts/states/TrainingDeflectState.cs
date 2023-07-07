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
    private const string BLOCK_SUCCESS_R = "Block_R_Success";
    private const string BLOCK_SUCCESS_L = "Block_L_Success";

    // Animation
    private string[] animationsGeneral = { IDLE };
    private string[] animationsAttack  = { ATTACK_R, ATTACK_L };
    private bool wasAnimationPlayed = false;
    private string currentAnimationState;

    // Audio
    private AudioManager audioManager;
    private AudioClip[] audioClipsGeneral;
    private AudioClip[] audioClipsAttack;
    private AudioClip[] audioClipsFailure;
    private AudioClip[] audioClipsSuccess;
    private bool wasAudioPlayed = false;

    // Trainer animator
    private Animator animator;

    // Sword
    private bool hitDetected = false;
    private bool successfulBlock = false;
    private bool pointsGained = false;
    
    private TrainingStateManager.swordSide hitSide;


    // Points
    private Points points;



    public override void EnterState(TrainingStateManager training, GameObject nextStateSpheres, GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres, Animator trainerAnimator) {
        resetState();

        // hide spheres which change the trainer position
        training.HideSelectionSpheres();

        training.hideTable();

        animator = trainerAnimator;

        // default animation state
        changeAnimationState(IDLE);

        if (points == null) {
            points = Points.instance;
        }
    }


    //
    // called once per frame from TrainingStateManager
    public override void UpdateState(TrainingStateManager training) {

        // wait until audio is done playing
        if (isAudioStillPlaying()) {
            return;
        }

        // TODO: if sword in block then set successfulBlock = true
        if (successfulBlock && !pointsGained) {
            Debug.Log("<color=yellow>hit in update detected</color>");
            successfullyBlocked();
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

        short audioAndAnimationIndex = 0;

        // when last step in traininPlan was reached
        if (trainingPlanIndex == trainingPlan.Length) {
            currentState = state.end;
            return;
        }

        // set index to play the correct audio & animation for the trainer
        switch (trainingPlan[trainingPlanIndex]) {
            case ATTACK_R:
                audioAndAnimationIndex = 0;
                training.setCurrentAction("Block Left");
                break;
            case ATTACK_L:
                audioAndAnimationIndex = 1;
                training.setCurrentAction("Block Right");
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


    //
    // Attacks / Blocks
    private void successfullyBlocked() {

        // check which side was hit
        switch (hitSide) {
            case TrainingStateManager.swordSide.strong:
                points.AddPoints(100);
                break;
            case TrainingStateManager.swordSide.weak:
                points.AddPoints(50);
                break;
        }

        pointsGained = true;

        if (trainingPlan[trainingPlanIndex] == ATTACK_R) playSuccessAnimationRight();
        if (trainingPlan[trainingPlanIndex] == ATTACK_L) playSuccessAnimationLeft();

        playSuccessSound();
    }


    private void prepareNextAttack(TrainingStateManager training) {

        Debug.Log("<color=blue>prepare next attack</color>");
        training.resetTrainerPosition();

        changeAnimationState(IDLE);

        if (!successfulBlock) {
            repeatAttack();
            return;
        }

        trainingPlanIndex++;

        resetChecks();
    }


    private void repeatAttack() {
        points.SubtractPoints(10);
        playFailSound();
        resetChecks();
    }


    //
    // State functions
    private void resetState() {
        trainingPlanIndex = 0;

        currentState = state.intro;

        resetChecks();
    }

    private void resetChecks() {
        wasAudioPlayed = false;
        wasAnimationPlayed = false;

        hitDetected = false;
        successfulBlock = false;
        pointsGained = false;
    }

    public override void SetAudios(AudioManager audioManager, TrainerAudioSO trainerAudioSO) {
        this.audioManager = audioManager;
        audioClipsGeneral = trainerAudioSO.audioClips;
        audioClipsAttack = trainerAudioSO.attackClips;
        audioClipsFailure = trainerAudioSO.failureClips;
        audioClipsSuccess = trainerAudioSO.complimentClips;
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

    private void playSuccessSound() {
        playSpecificAudio(audioClipsSuccess[Random.Range(0, audioClipsSuccess.Length-1)]);
    }

    private void playFailSound() {
        playSpecificAudio(audioClipsFailure[0]);
    }


    //
    // Animation
    private void changeAnimationState(string newState, bool normalizedTime = false) {
        // stop the same animation from interrupting itself
        if (currentAnimationState == newState) {
            return;
        }
        // play animation
        if (normalizedTime) animator.Play(newState, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        else                animator.Play(newState);
        // reassign the current state
        currentAnimationState = newState;
    }


    private void playSuccessAnimationLeft() {
        Debug.Log("<color=green>playSuccessAnimation</color>");
        changeAnimationState(BLOCK_SUCCESS_L, true);
    }

    private void playSuccessAnimationRight() {
        Debug.Log("<color=green>playSuccessAnimation</color>");
        changeAnimationState(BLOCK_SUCCESS_R, true);
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


    //
    // Sword Hits
    public void detectHit(TrainingStateManager.swordSide swordSide) {

        // allow only one hit-detection per attack
        if (hitDetected) {
            return;
        }

        hitDetected = true;

        // TODO: remove
        successfulBlock = true;

        hitSide = swordSide;
    }
}