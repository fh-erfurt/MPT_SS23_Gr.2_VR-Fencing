using UnityEngine;


public class Weak_side_hit : Subject {

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("enemy_fence")) {
            print("Weak side got hit!");
            NotifyObservers(TrainingStateManager.swordSide.weak);
        }
    }
}