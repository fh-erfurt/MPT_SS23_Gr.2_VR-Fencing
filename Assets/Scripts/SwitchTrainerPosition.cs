using UnityEngine;


public class SwitchTrainerPosition : MonoBehaviour {

    [Header("Trainer-Model in Scene")]
    public Transform trainer;

    [Header("Positions")]
    public Transform mainPosition;
    public Transform inPlayerPosition;
    public Transform nextToPlayerPosition;


    // set trainer to the main-postition
    public void SetMain() {
        // only move on x/z-plane
        trainer.position = new Vector3(mainPosition.position.x, trainer.position.y, mainPosition.position.z);
        trainer.rotation = Quaternion.Euler(0, 180, 0);
    }

    // set trainer to the player-postition
    public void SetInPlayer() {
        // only move on x/z-plane
        trainer.position = new Vector3(inPlayerPosition.position.x, trainer.position.y, inPlayerPosition.position.z);
        trainer.rotation = Quaternion.Euler(0, 0, 0);
    }

    // set trainer next to the player
    public void SetNextToPlayer() {
        // only move on x/z-plane
        trainer.position = new Vector3(nextToPlayerPosition.position.x, trainer.position.y, nextToPlayerPosition.position.z);
        trainer.rotation = Quaternion.Euler(0, 0, 0);
    }
}
