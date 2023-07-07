using UnityEngine;


public class TrainingDeflectState : TrainingBaseState {

    // Training schedule
    private string[] trainingPlan = {   BLOCK_R, BLOCK_R,
                                        BLOCK_L, BLOCK_L,
                                        BLOCK_R, BLOCK_L };
    private int trainingPlanIndex = 0;

    // State of the state
    private enum state { intro, training, end };
    private state currentState = state.intro;

    // Animation States
    private const string IDLE = "Idle";
    private const string BLOCK_R = "Attack_L";
    private const string BLOCK_L = "Attack_R";
    private const string BLOCK_SUCCESS_R = "Block_R_Success";
    private const string BLOCK_SUCCESS_L = "Block_L_Success";

    // Animation
    private string[] animationsGeneral = { IDLE };
    private string[] animationsAttack  = { BLOCK_L, BLOCK_R };
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

    private TrainingStateManager.swordSide hitSide = TrainingStateManager.swordSide.none;

    private posManager currentPosManager;

    // Points
    private Points points;



    public override void EnterState(TrainingStateManager training, GameObject nextStateSpheres, GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres, Animator trainerAnimator) {
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

        // only accept sword hit-detection when sword is in the correct blocking spot
        if (hitWasDetectedOutsideOfPerfectPosition() && sufficientTimeOnAnimationHasPassed()) {
            hitSide = TrainingStateManager.swordSide.none;
            hitDetected = false;
        }

        if (currentPosManager && currentPosManager.perfectPosition() && hitSide != TrainingStateManager.swordSide.none) {
            successfulBlock = true;
        }

        if (successfulBlock && !pointsGained) {
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
                training.setCurrentAction("Block Left");
                break;
            case BLOCK_R:
                audioAndAnimationIndex = 1;
                currentPosManager = training.getRightBlockPositionManager();
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
            wasAnimationPlayed = true;
            currentPosManager.setBlockPositionsActive();
            changeAnimationState(animationsAttack[audioAndAnimationIndex]);
            // break into UpdateState() and wait until animation is finished
            return;
        };

        // reset for next trainer attack
        prepareNextAttack(training);
    }


    //
    // Attacks / Blocks
    private void successfullyBlocked() {

        pointsGained = true;

        // check which side was hit
        switch (hitSide) {
            case TrainingStateManager.swordSide.strong:
                points.AddPoints(100);
                break;
            case TrainingStateManager.swordSide.weak:
                points.AddPoints(50);
                break;
        }

        if (trainingPlan[trainingPlanIndex] == BLOCK_L) playSuccessAnimationRight();
        if (trainingPlan[trainingPlanIndex] == BLOCK_R) playSuccessAnimationLeft();
    }


    private void prepareNextAttack(TrainingStateManager training) {

        Debug.Log("<color=blue>prepare next attack</color>");
        training.resetTrainerPosition();


        changeAnimationState(IDLE);

        if (!successfulBlock) {
            repeatAttack();
            return;
        }

        playSuccessSound();

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


    private bool hitWasDetectedOutsideOfPerfectPosition() {
        return hitDetected && !currentPosManager.perfectPosition();
    }


    private bool sufficientTimeOnAnimationHasPassed() {
        // prevent too early hit detection when trainer is swinging back for an attack
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f;
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
        changeAnimationState(BLOCK_SUCCESS_L, true);
    }

    private void playSuccessAnimationRight() {
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

        hitSide = swordSide;
    }
}