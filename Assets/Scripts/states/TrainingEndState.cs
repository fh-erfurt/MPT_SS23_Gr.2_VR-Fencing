using UnityEngine;
using UnityEngine.SceneManagement;


public class TrainingEndState : TrainingBaseState {

    // Audio
    private AudioManager audioManager;
    private AudioClip[] audioClips;

    private bool wasAudioPlayed = false;

    // Timer
    private float delayBeforeAudioStarts = 2f;
    private float currentTimer = 0f;

    // Selection spheres
    private GameObject nextStateSpheres;

    // Next step
    private TrainingStateManager.nextStep nextStep = TrainingStateManager.nextStep.not_set;

    // Level-Select scene name
    private string levelSelectName = "SelectTraining";



    public override void EnterState(TrainingStateManager training,
                                    GameObject nextStateSpheres,
                                    GameObject trainerPositionSpheres,
                                    GameObject skipInstructionSpheres) {
        resetState();
        training.HideSelectionSpheres();
        this.nextStateSpheres = nextStateSpheres;
    }


    public override void UpdateState(TrainingStateManager training) {

        // to level select
        if (nextStep == TrainingStateManager.nextStep.next_state) {
            SceneManager.LoadScene(levelSelectName);
        }

        // repeat training
        if (nextStep == TrainingStateManager.nextStep.repeat_state) {
            training.SwitchState(training.StartState);
        }

        if (wasAudioPlayed) {
            return;
        }


        if (currentTimer <= delayBeforeAudioStarts) {
            currentTimer += Time.deltaTime;
            return;
        }

        if (!wasAudioPlayed) {
            audioManager.playClipAtTrainerPosition(audioClips[0]);
            wasAudioPlayed = true;
        }

        nextStateSpheres.SetActive(true);
    }


    public override void SetAudios(AudioManager audioManager, TrainerAudioSO trainerAudioSO) {
        this.audioManager = audioManager;
        audioClips = trainerAudioSO.audioClips;
    }


    public override void SetNextStep(TrainingStateManager.nextStep nextStep) {
        this.nextStep = nextStep;
    }



    private void resetState() {
        currentTimer = 0f;
        wasAudioPlayed = false;
        nextStep = TrainingStateManager.nextStep.not_set;
    }
}