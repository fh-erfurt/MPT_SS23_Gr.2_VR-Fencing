using UnityEngine;


public class TrainingAttackState : TrainingBaseState {

    // Training schedule
    private string[] trainingPlan = {   ATTACK_R, ATTACK_L, ATTACK_M,
                                        ATTACK_L, ATTACK_M, ATTACK_R,
                                        ATTACK_R, ATTACK_M, ATTACK_L };
    private int trainingPlanIndex = 0;

    // State of the state
    private enum state { intro, training, end };
    private state currentState = state.intro;

    // Animation States
    private const string IDLE = "Idle";
    private const string ATTACK_R = "Attack_L";
    private const string ATTACK_L = "Attack_R";
    private const string ATTACK_M = "Attack_M";
    private const string INVITATION = "Invitation";

    // Animation
    private string[] animationsGeneral = { IDLE };
    private string animationInvitation  = INVITATION;
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
    // private bool perfectBlock = false;
    // private bool acceptedBlock = false;
    private bool acceptedAttack = false;
    private bool pointsGained = false;
    private bool isAttackingWindowActive = false;

    private posManager currentPosManager;

    // Points
    private Points points;



    public override void EnterState(TrainingStateManager training, GameObject nextStateSpheres, GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres, Animator trainerAnimator) {

        // if (animationsAttack.Length != audioClipsAttack.Length) {
        //     Debug.LogError("'animationsAttack' and 'audioClipsAttack' must have an equal amount of elements.");
        // }

        resetState(training);

        // hide spheres which change the trainer position
        training.hideSelectionSpheres();

        training.hideTable();

        animator = trainerAnimator;

        // default animation state
        changeAnimationState(IDLE);

        training.resetTrainerPosition();

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

        // check if attack was in the perfect zone
        if (isSwordInPerfectPosition()) {
            acceptedAttack = true;
        }

        // successful block if a perfect or accepted block was made
        if (acceptedAttack && !pointsGained) {
            successfullyAttacked();
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
            case ATTACK_L:
                audioAndAnimationIndex = 0;
                currentPosManager = training.getLeftAttackPositionManager();
                training.setCurrentAction($"Attack Left ({trainingPlanIndex+1}/{trainingPlan.Length})"); // UI
                break;
            case ATTACK_R:
                audioAndAnimationIndex = 1;
                currentPosManager = training.getRightAttackPositionManager();
                training.setCurrentAction($"Attack Right ({trainingPlanIndex+1}/{trainingPlan.Length})"); // UI
                break;
            case ATTACK_M:
                audioAndAnimationIndex = 2;
                currentPosManager = training.getMiddleAttackPositionManager();
                training.setCurrentAction($"Attack Middle ({trainingPlanIndex+1}/{trainingPlan.Length})"); // UI
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
            changeAnimationState(animationInvitation);
            return; // break into UpdateState() and wait until animation is finished
        };

        // reset for next trainer attack
        prepareNextAttack(training);
    }


    //
    // Successful attack
    private void successfullyAttacked() {

        pointsGained = true;

        points.AddPoints(100);
    }


    //
    // Next attack
    private void prepareNextAttack(TrainingStateManager training) {

        // training.resetTrainerPosition();

        changeAnimationState(IDLE);

        training.setTrainerSwordColorBlack();

        if (!acceptedAttack) {
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


    //
    // Sword
    private bool isSwordInPerfectPosition() {
        return currentPosManager && currentPosManager.isSwordInPerfectPosition();
    }


    public void setAttackingWindowActive() {
        isAttackingWindowActive = true;
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

        // perfectBlock = false;
        // acceptedBlock = false;
        acceptedAttack = false;
        pointsGained = false;
        isAttackingWindowActive = false;
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
}