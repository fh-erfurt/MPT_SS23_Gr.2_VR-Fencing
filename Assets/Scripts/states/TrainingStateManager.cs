using UnityEngine;


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

    private AudioManager audioManager;

    [Header("Skip Instructions Spheres")]
    public GameObject skipInstructionsSpheres;

    [Header("Next State Spheres")]
    public GameObject nextStateSpheres;

    [Header("Trainer Position Spheres")]
    public GameObject trainerPositionSpheres;


    // Observer-pattern
    [Header("Subject of Observer")]
    [SerializeField] Subject _nextStateSphereSubject;
    [SerializeField] Subject _repeatStateSphereSubject;
    [SerializeField] Subject _skipInstructionSphereSubject;
    [SerializeField] Subject _dontSkipInstructionSphereSubject;


    public enum nextStep { not_set, next_state, repeat_state, skip_instructions };



    private void Start() {
        // Starting state
        currentState = StartState;
        // Context to this script
        currentState.EnterState(this, nextStateSpheres, trainerPositionSpheres, skipInstructionsSpheres);

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
    }


    private void Update() {
        // Call UpdateState on the current state on every frame
        currentState.UpdateState(this);
    }


    public void SwitchState(TrainingBaseState newState) {
        currentState = newState;
        newState.EnterState(this, nextStateSpheres, trainerPositionSpheres, skipInstructionsSpheres);
    }


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
}