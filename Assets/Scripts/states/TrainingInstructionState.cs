using System;
using UnityEngine;


public class TrainingInstructionState : TrainingBaseState {

    // Animation States
    private const string IDLE = "Idle";

    private const string DEFLECT_R = "Deflect_R_TOP";
    private const string DEFLECT_L = "Deflect_L_TOP";
    private const string DEFLECT_M = "Deflect_M";

    private const string ATTACK_R = "Attack_R";
    private const string ATTACK_L = "Attack_L";
    private const string ATTACK_M = "Attack_M";

    // Animation
    private string[] animationOrder;
    private int currentAnimation = 0;
    private bool wasAnimationPlayed = false;
    private string currentAnimationState;

    // Audio
    private AudioManager audioManager;
    private AudioClip[] audioClips;

    private int numberOfAudioClips;
    private int currentAudio = 0;

    // Selection spheres
    private GameObject nextStateSpheres;
    private GameObject trainerPositionSpheres;

    // Trainer animator
    private Animator animator;

    private MainManager mainManager;

    // Next step
    private TrainingStateManager.nextStep nextStep = TrainingStateManager.nextStep.not_set;

    private bool readyForNextInstruction = true;



    // called once when entering the state
    public override void EnterState(TrainingStateManager training,
                                    GameObject nextStateSpheres,
                                    GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres,
                                    Animator trainerAnimator) {


        if (mainManager == null) {
            mainManager = MainManager.instance;
        }

        setAnimationOrder();

        if (animationOrder.Length != audioClips.Length) {
            Debug.LogError("'animationOrder' and 'audioClips' must have an equal amount of elements.");
        }

        resetState();

        training.hideTable();

        training.setCurrentAction("Instruction");

        training.hideSelectionSpheres();
        this.nextStateSpheres = nextStateSpheres;
        this.trainerPositionSpheres = trainerPositionSpheres;
        // show spheres to change trainer position
        trainerPositionSpheres.SetActive(true);

        animator = trainerAnimator;

        // default animation state
        ChangeAnimationState(IDLE);
    }


    //
    // called once per frame from TrainingStateManager
    public override void UpdateState(TrainingStateManager training) {

        // when the last audio-clip was played only check for next step
        if (isLastAudioClipPlayed()) {
            // wait for audio and animation to finish
            if (!isAudioStillPlaying() && !isAnimationStillPlaying()) {
                training.setNextStateSphereText("Point to\n continue to Training");
                training.setRepeatStateSphereText("Point to\nrepeat Instructions");
                nextStateSpheres.SetActive(true);
                checkNextState(training);
            }
            return;
        }

        // when animation is done play next audio
        if (readyForNextInstruction) {
            readyForNextInstruction = false;
            playAudio();
        }

        // wait until audio is done playing
        if (isAudioStillPlaying()) {
            return;
        }

        // play animation after audio is finished
        if (!wasAnimationPlayed) {
            switch (mainManager.selectedTraining) {
                case MainManager.trainingType.training_1:
                    playDeflectAnimation();
                    break;
                case MainManager.trainingType.training_2:
                    playAttackAnimation();
                    break;
            }
            return;
        }

        // wait until animation is done playing
        if (isAnimationStillPlaying()) {
            return;
        }

        prepareNextInstruction(training);
    }



    private void prepareNextInstruction(TrainingStateManager training) {
        // training.resetTrainerPosition();
        currentAnimation++;
        currentAudio++;
        readyForNextInstruction = true;
        wasAnimationPlayed = false;
    }


    //
    // State functions
    private void resetState() {
        currentAudio = 0;
        currentAnimation = 0;
        wasAnimationPlayed = false;
        nextStep = TrainingStateManager.nextStep.not_set;
    }


    private void setAnimationOrder() {
        switch (mainManager.selectedTraining) {
            case MainManager.trainingType.training_1:
                animationOrder = new String[] { IDLE, DEFLECT_R, DEFLECT_L, DEFLECT_M, IDLE };
                break;
            case MainManager.trainingType.training_2:
                animationOrder = new String[] { IDLE, ATTACK_R, ATTACK_L, ATTACK_M, IDLE };
                break;
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


    //
    // Check for next state
    private void checkNextState(TrainingStateManager training) {
        // continue to training
        if (nextStep == TrainingStateManager.nextStep.next_state) {
            switch (mainManager.selectedTraining) {
                case MainManager.trainingType.training_1:
                    training.SwitchState(training.DeflectState);
                    break;
                case MainManager.trainingType.training_2:
                    training.SwitchState(training.AttackState);
                    break;
        }
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
    }

    private bool isAudioStillPlaying() {
        return audioManager.isAudioStillPlaying();
    }

    private bool isLastAudioClipPlayed() {
        return currentAudio == numberOfAudioClips;
    }


    //
    // Animation
    private void playDeflectAnimation() {
        switch (currentAnimation) {
            case 1:
                ChangeAnimationState(DEFLECT_R);
                break;
            case 2:
                ChangeAnimationState(DEFLECT_L);
                break;
            case 3:
                ChangeAnimationState(DEFLECT_M);
                break;
            default:
                ChangeAnimationState(IDLE);
                break;
        }
        wasAnimationPlayed = true;
    }

    private void playAttackAnimation() {
        switch (currentAnimation) {
            case 1:
                ChangeAnimationState(ATTACK_R);
                break;
            case 2:
                ChangeAnimationState(ATTACK_L);
                break;
            case 3:
                ChangeAnimationState(ATTACK_M);
                break;
            default:
                ChangeAnimationState(IDLE);
                break;
        }
        wasAnimationPlayed = true;
    }


    private void ChangeAnimationState(string newState) {
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