using UnityEngine;


public class Strong_side_hit : Subject {

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("enemy_fence")) {
            print("Strong side got hit!");
            NotifyObservers(TrainingStateManager.swordSide.strong);
        }
    }
}