using UnityEngine;


public class posManager : MonoBehaviour {

    public GameObject positions;

    public posOfSword swordPosition;
    public posOfHand handPosition;


    private void Update() {

        // if (swordPosition.IsSwordOnCorrectPosition() && handPosition.IsHandOnCorrectPosition()) {
        //     Debug.Log("right position for blocking!");
        // }
    }


    public bool perfectPosition() {
        return swordPosition.IsSwordOnCorrectPosition() && handPosition.IsHandOnCorrectPosition();
    }


    public void resetCorrectStates() {
        swordPosition.resetIsHandOnCorrectPosition();
        handPosition.resetIsHandOnCorrectPosition();
    }


    public void setBlockPositionsActive() {
        positions.SetActive(true);
    }

    public void hideBlockPositions() {
        positions.SetActive(false);
    }
}