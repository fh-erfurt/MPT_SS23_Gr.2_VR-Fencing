using UnityEngine;


public class TrainingDeflectState : TrainingBaseState {

    // Training schedule
    private string[] trainingPlan = {   //BLOCK_R, BLOCK_L, BLOCK_M,
                                        //BLOCK_L, BLOCK_M, BLOCK_R,
                                        BLOCK_R, BLOCK_M, BLOCK_L };
    private int trainingPlanIndex = 0;

    // State of the state
    private enum state { intro, training, end };
    private state currentState = state.intro;

    // Animation States
    private const string IDLE = "Idle";
    private const string BLOCK_R = "Attack_L";
    private const string BLOCK_L = "Attack_R";
    private const string BLOCK_M = "Attack_M";
    private const string BLOCK_SUCCESS_R = "Block_R_Success";
    private const string BLOCK_SUCCESS_L = "Block_L_Success";
    private const string BLOCK_SUCCESS_M = "Block_M_Success";

    // Animation
    private string[] animationsGeneral = { IDLE };
    private string[] animationsAttack  = { BLOCK_L, BLOCK_R, BLOCK_M };
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
    private bool perfectBlock = false;
    private bool acceptedBlock = false;
    private bool pointsGained = false;
    private bool isBlockingWindowActive = false;

    private posManager currentPosManager;

    // Points
    private Points points;



    public override void EnterState(TrainingStateManager training, GameObject nextStateSpheres, GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres, Animator trainerAnimator) {

        if (animationsAttack.Length != audioClipsAttack.Length) {
            Debug.LogError("'animationsAttack' and 'audioClipsAttack' must have an equal amount of elements.");
        }

        resetState(training);

        // hide spheres which change the trainer position
        training.hideSelectionSpheres();

        training.hideTable();

        animator = trainerAnimator;

        // default animation state
        changeAnimationState(IDLE);

        if (points == null) {
            points = Points.instance;
        }
    }


    //
    // Called once per frame from TrainingStateManager
    public override void UpdateState(TrainingStateManager training) {

        // wait until audio is done playing
        if (isAudioStillPlaying()) {
            return;
        }

        // check if block was in the perfect zone or the accepted area
        if (isBlockingWindowActive && !acceptedBlock && isSwordInPerfectPosition()) {
            perfectBlock = true;
        }

        if (isBlockingWindowActive && !perfectBlock && isSwordInAcceptedArea()) {
            acceptedBlock = true;
        }

        // successful block if a perfect or accepted block was made
        if ((perfectBlock || acceptedBlock) && !pointsGained) {
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
            case BLOCK_L:
                audioAndAnimationIndex = 0;
                currentPosManager = training.getLeftBlockPositionManager();
                training.setCurrentAction($"Block Left ({trainingPlanIndex+1}/{trainingPlan.Length})"); // UI
                break;
            case BLOCK_R:
                audioAndAnimationIndex = 1;
                currentPosManager = training.getRightBlockPositionManager();
                training.setCurrentAction($"Block Right ({trainingPlanIndex+1}/{trainingPlan.Length})"); // UI
                break;
            case BLOCK_M:
                audioAndAnimationIndex = 2;
                currentPosManager = training.getMiddleBlockPositionManager();
                training.setCurrentAction($"Block Middle ({trainingPlanIndex+1}/{trainingPlan.Length})"); // UI
                break;
        }

        // play audio once
        if (!wasAudioPlayed) {
            wasAudioPlayed = true;
            playSpecificAudio(audioClipsAttack[audioAndAnimationIndex]);
            return; // break into UpdateState() and wait until audio is finished
        };

        // play animation once
        if (!wasAnimationPlayed) {
            wasAnimationPlayed = true;
            currentPosManager.setBlockPositionsActive();
            changeAnimationState(animationsAttack[audioAndAnimationIndex]);
            return; // break into UpdateState() and wait until animation is finished
        };

        // reset for next trainer attack
        prepareNextAttack(training);
    }


    //
    // Successful block
    private void successfullyBlocked() {

        pointsGained = true;

        if (perfectBlock)  points.AddPoints(100);
        if (acceptedBlock) points.AddPoints(50);

        if (trainingPlan[trainingPlanIndex] == BLOCK_L) playSuccessAnimationRight();
        if (trainingPlan[trainingPlanIndex] == BLOCK_R) playSuccessAnimationLeft();
        if (trainingPlan[trainingPlanIndex] == BLOCK_M) playSuccessAnimationMiddle();
    }


    //
    // Next attack
    private void prepareNextAttack(TrainingStateManager training) {

        training.resetTrainerPosition();

        changeAnimationState(IDLE);

        if (!perfectBlock && !acceptedBlock) {
            repeatAttack();
            return;
        }

        playSuccessSound();

        training.setTrainerSwordColorBlack();

        currentPosManager.hideBlockPositions();

        trainingPlanIndex++;

        currentPosManager = null;

        resetChecks();
    }

    private void repeatAttack() {
        points.SubtractPoints(10);
        playFailSound();
        resetChecks();
    }


    //
    // Sword
    private bool isSwordInPerfectPosition() {
        return currentPosManager && currentPosManager.isSwordInPerfectPosition();
    }

    private bool isSwordInAcceptedArea() {
        return currentPosManager && currentPosManager.isSwordInAcceptedArea();
    }

    public void setIsBlockingWindowActive(bool isActive) {
        isBlockingWindowActive = isActive;
    }


    //
    // State functions
    private void resetState(TrainingStateManager training) {
        trainingPlanIndex = 0;

        currentState = state.intro;

        currentPosManager = null;

        training.getRightBlockPositionManager().hideBlockPositions();
        training.getLeftBlockPositionManager().hideBlockPositions();

        resetChecks();
    }

    private void resetChecks() {
        wasAudioPlayed = false;
        wasAnimationPlayed = false;

        perfectBlock = false;
        acceptedBlock = false;
        pointsGained = false;
        isBlockingWindowActive = false;
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

    private void playStartAudio() {
        playSpecificAudio(audioClipsGeneral[0]);
    }

    private void playSuccessSound() {
        playSpecificAudio(audioClipsSuccess[Random.Range(0, audioClipsSuccess.Length)]);
    }

    private void playFailSound() {
        playSpecificAudio(audioClipsFailure[0]);
    }

    private bool isAudioStillPlaying() {
        return audioManager.isAudioStillPlaying();
    }


    //
    // Animation State
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
    // Animation Success
    private void playSuccessAnimationLeft() {
        changeAnimationState(BLOCK_SUCCESS_L, true);
    }

    private void playSuccessAnimationRight() {
        changeAnimationState(BLOCK_SUCCESS_R, true);
    }

    private void playSuccessAnimationMiddle() {
        changeAnimationState(BLOCK_SUCCESS_M, true);
    }
}