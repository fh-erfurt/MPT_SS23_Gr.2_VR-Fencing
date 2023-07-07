using System.Collections;
using UnityEngine;
using TMPro;


public class TrainingStateManager : MonoBehaviour, IObserver {

    // Reference to active state in state-machine
    private TrainingBaseState currentState;
    // States
    public TrainingStartState       StartState       = new TrainingStartState();
    public TrainingInstructionState InstructionState = new TrainingInstructionState();
    public TrainingDeflectState     DeflectState     = new TrainingDeflectState();
    public TrainingEndState         EndState         = new TrainingEndState();


    [Header("Trainer Audios")]
    public TrainerAudioSO introAudiosSO;
    public TrainerAudioSO instructionAudiosSO;
    public TrainerAudioSO deflectingAudiosSO;
    public TrainerAudioSO endAudiosSO;

    [Header("Trainer")]
    public Transform trainer;
    public Animator trainerAnimator;
    private Vector3 trainerPositionMain;

    [Header("Selection Spheres")]
    public GameObject skipInstructionsSpheres;
    public GameObject nextStateSpheres;
    public GameObject trainerPositionSpheres;

    [Header("Table")]
    public GameObject table;

    [Header("UI")]
    public TMP_Text currentActionText;

    [Header("Manager")]
    public posManager posManagerRightBlock;
    public posManager posManagerLeftBlock;
    private AudioManager audioManager;


    // Observer-pattern
    [Header("Sphere Subjects")]
    [SerializeField] Subject _nextStateSphereSubject;
    [SerializeField] Subject _repeatStateSphereSubject;
    [SerializeField] Subject _skipInstructionSphereSubject;
    [SerializeField] Subject _dontSkipInstructionSphereSubject;


    // Enums
    public enum nextStep { not_set, next_state, repeat_state, skip_instructions };

    public enum swordSide { none, weak, strong };



    // Singleton
    public static TrainingStateManager instance;

    private void Awake() {
        // Singleton
        if (instance == null) {
            instance = this;
        }
    }


    private void Start() {
        // starting state
        currentState = StartState;
        // context to this script
        currentState.EnterState(this, nextStateSpheres, trainerPositionSpheres, skipInstructionsSpheres, trainerAnimator);

        // get main trainer position
        trainerPositionMain = trainerPositionSpheres.transform.Find("Trainer_Position_Main").transform.position;

        // disable selection spheres
        hideSelectionSpheres();

        audioManager = AudioManager.instance;

        // give each state their needed audios
        setStateAudios();

        // add itself to the subjects list of observers
        subscribeToSubjects();

        posManagerRightBlock.hideBlockPositions();
        posManagerLeftBlock.hideBlockPositions();
    }


    //
    // Update for active state
    private void Update() {
        // call UpdateState on the current state on every frame
        currentState.UpdateState(this);
    }



    //
    // Switch state-machine into a new state
    public void SwitchState(TrainingBaseState newState) {
        currentState = newState;
        newState.EnterState(this, nextStateSpheres, trainerPositionSpheres, skipInstructionsSpheres, trainerAnimator);
    }


    //
    // Setter
    private void subscribeToSubjects() {
        _nextStateSphereSubject.AddObserver(this);
        _repeatStateSphereSubject.AddObserver(this);
        _skipInstructionSphereSubject.AddObserver(this);
        _dontSkipInstructionSphereSubject.AddObserver(this);
    }

    private void setStateAudios() {
        StartState.SetAudios      (audioManager, introAudiosSO);
        InstructionState.SetAudios(audioManager, instructionAudiosSO);
        DeflectState.SetAudios    (audioManager, deflectingAudiosSO);
        EndState.SetAudios        (audioManager, endAudiosSO);
    }

    //
    // Selection Spheres
    public void hideSelectionSpheres() {
        nextStateSpheres.SetActive(false);
        trainerPositionSpheres.SetActive(false);
        skipInstructionsSpheres.SetActive(false);
    }


    //
    // Called from UI-Elements etc.
    public void skipInstructions() {
        SwitchState(DeflectState);
    }

    public void continueToNextState() {
        currentState.SetNextStep(TrainingStateManager.nextStep.next_state);
    }

    public void repeatState() {
        currentState.SetNextStep(TrainingStateManager.nextStep.repeat_state);
    }


    public void setCurrentAction(string action) {
        currentActionText.text = "Current: " + action;
    }


    //
    // Trainer
    public void resetTrainerPosition() {
        Vector3 destination = new Vector3(trainerPositionMain.x, trainer.position.y, trainerPositionMain.z);
        Vector3 origin      = new Vector3(trainer.position.x,    trainer.position.y, trainer.position.z);

        StartCoroutine(moveTrainer(destination, origin));
    }

    // smoothly move trainer to the position
    private IEnumerator moveTrainer(Vector3 destination, Vector3 origin) {
        float totalMovementTime = 0.2f; // amount of time for the move
        float currentMovementTime = 0f; // amount of time passed

        while (Vector3.Distance(trainer.position, destination) >= 0.01) {
            currentMovementTime += Time.deltaTime;
            trainer.position = Vector3.Lerp(origin, destination, currentMovementTime / totalMovementTime);
            yield return null;
        }
    }


    //
    // Observer
    public void OnNotify(TrainingStateManager.nextStep nextStep) {

        if (nextStep == TrainingStateManager.nextStep.next_state) {
            continueToNextState();
            return;
        }

        if (nextStep == TrainingStateManager.nextStep.repeat_state) {
            repeatState();
            return;
        }

        if (nextStep == TrainingStateManager.nextStep.skip_instructions) {
            skipInstructions();
            return;
        }
    }


    //
    // Sword
    public void hitDetected(TrainingStateManager.swordSide swordSide) {
        if (currentState == DeflectState) {
            DeflectState.detectHit(swordSide);
        }
    }


    //
    // Position manager
    public posManager getRightBlockPositionManager() {
        return posManagerRightBlock;
    }

    public posManager getLeftBlockPositionManager() {
        return posManagerLeftBlock;
    }


    //
    // Table
    public void hideTable() {
        table.SetActive(false);
    }


    // not important
    public void OnNotify(TrainingStateManager.swordSide s) {}
    public void OnNotify(int i) {}
}