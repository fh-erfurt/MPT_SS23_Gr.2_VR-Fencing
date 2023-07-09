using UnityEngine;


public class TrainerAnimationEvents : MonoBehaviour {

    private TrainingStateManager trainingStateManager;

    private MainManager mainManager;


    private void Start() {
        if (trainingStateManager == null) {
            trainingStateManager = TrainingStateManager.instance;
        }

        if (mainManager == null) {
            mainManager = MainManager.instance;
        }
    }


    //
    // Blocking
    public void enableBlockingWindow() {
        if (mainManager.selectedTraining == MainManager.trainingType.training_1) {
            trainingStateManager.getDeflectState().setIsBlockingWindowActive(true);
            trainingStateManager.setTrainerSwordColorGreen();
        }
    }

    public void disableBlockingWindow() {
        if (mainManager.selectedTraining == MainManager.trainingType.training_1) {
            trainingStateManager.getDeflectState().setIsBlockingWindowActive(false);
            trainingStateManager.setTrainerSwordColorBlack();
        }
    }


        // Attacking
    public void enableAttackingWindow() {
        if (mainManager.selectedTraining == MainManager.trainingType.training_2) {
            trainingStateManager.getAttackState().setAttackingWindowActive();
            trainingStateManager.setTrainerSwordColorGreen();
        }
    }
}
