using UnityEngine;


public class TrainerAnimationEvents : MonoBehaviour {

    private TrainingStateManager trainingStateManager;


    private void Start() {
        if (trainingStateManager == null) {
            trainingStateManager = TrainingStateManager.instance;
        }
    }


    public void enableBlockingWindow() {
        trainingStateManager.getDeflectState().setIsBlockingWindowActive(true);
        trainingStateManager.setTrainerSwordColorGreen();
    }


    public void disableBlockingWindow() {
        trainingStateManager.getDeflectState().setIsBlockingWindowActive(false);
        trainingStateManager.setTrainerSwordColorBlack();
    }
}
