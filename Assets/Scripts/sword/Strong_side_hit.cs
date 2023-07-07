using UnityEngine;


public class Strong_side_hit : Subject {

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("enemy_fence")) {
            print("Strong side got hit!");
            NotifySwordObservers(TrainingStateManager.swordSide.strong);
        }
    }
}