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

    [Header("UI")]
    public TMP_Text currentActionText;


    // Observer-pattern
    [Header("Sphere Subjects")]
    [SerializeField] Subject _nextStateSphereSubject;
    [SerializeField] Subject _repeatStateSphereSubject;
    [SerializeField] Subject _skipInstructionSphereSubject;
    [SerializeField] Subject _dontSkipInstructionSphereSubject;
    [Header("Sword Subjects")]
    [SerializeField] Subject _strongSide;
    [SerializeField] Subject _weakSide;


    // Manager
    private AudioManager audioManager;

    // Enums
    public enum nextStep { not_set, next_state, repeat_state, skip_instructions };

    public enum swordSide { weak, strong };



    private void Start() {
        // Starting state
        currentState = StartState;
        // Context to this script
        currentState.EnterState(this, nextStateSpheres, trainerPositionSpheres, skipInstructionsSpheres, trainerAnimator);

        // Get main trainer position
        trainerPositionMain = trainerPositionSpheres.transform.Find("Trainer_Position_Main").transform.position;

        // Disable selection spheres
        HideSelectionSpheres();

        audioManager = AudioManager.instance;

        // Give each state their needed audios
        StartState.SetAudios      (audioManager, introAudiosSO);
        InstructionState.SetAudios(audioManager, instructionAudiosSO);
        DeflectState.SetAudios    (audioManager, deflectingAudiosSO);
        EndState.SetAudios        (audioManager, endAudiosSO);

        // Add itself to the subjects list of observers
        _nextStateSphereSubject.AddObserver(this);
        _repeatStateSphereSubject.AddObserver(this);
        _skipInstructionSphereSubject.AddObserver(this);
        _dontSkipInstructionSphereSubject.AddObserver(this);
        _strongSide.AddObserver(this);
        _weakSide.AddObserver(this);
    }


    //
    // Update for each state
    private void Update() {
        // Call UpdateState on the current state on every frame
        currentState.UpdateState(this);
    }



    //
    // Switch to a new state
    public void SwitchState(TrainingBaseState newState) {
        currentState = newState;
        newState.EnterState(this, nextStateSpheres, trainerPositionSpheres, skipInstructionsSpheres, trainerAnimator);
    }


    //
    // Selection Spheres
    public void HideSelectionSpheres() {
        nextStateSpheres.SetActive(false);
        trainerPositionSpheres.SetActive(false);
        skipInstructionsSpheres.SetActive(false);
    }


    //
    // Called from UI-Elements etc.
    public void SkipInstructions() {
        SwitchState(DeflectState);
    }

    public void ContinueToNextState() {
        currentState.SetNextStep(TrainingStateManager.nextStep.next_state);
    }

    public void RepeatState() {
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
            ContinueToNextState();
            return;
        }

        if (nextStep == TrainingStateManager.nextStep.repeat_state) {
            RepeatState();
            return;
        }

        if (nextStep == TrainingStateManager.nextStep.skip_instructions) {
            SkipInstructions();
            return;
        }
    }

    // Sword
    public void OnNotify(TrainingStateManager.swordSide swordSide) {
        DeflectState.detectHit(swordSide);
    }

    // not important
    public void OnNotify(int i) {}
}