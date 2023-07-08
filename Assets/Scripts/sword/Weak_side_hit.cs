using UnityEngine;


public class Weak_side_hit : MonoBehaviour {

    private TrainingStateManager trainingStateManager;

    private void Start() {
        if (trainingStateManager == null) {
            trainingStateManager = TrainingStateManager.instance;
        }
    }


    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("enemy_fence")) {
            print("Weak side got hit!");
            // trainingStateManager.hitDetected(TrainingStateManager.swordSide.weak);
        }
    }
}