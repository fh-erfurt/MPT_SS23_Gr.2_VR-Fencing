using UnityEngine;


public class posManager : MonoBehaviour {

    [Header("Parent of all positions")]
    public GameObject positions;

    [Header("Perfect Position")]
    public posOfSword swordPosition;
    public posOfHand handPosition;

    [Header("Accepted Position")]
    public posInAcceptedArea acceptedArea;


    public bool isSwordInPerfectPosition() {
        return swordPosition.IsSwordInPerfectPosition() && handPosition.IsHandInPerfectPosition();
    }


    public bool isSwordInAcceptedArea() {
        return acceptedArea.isSwordInAcceptedPosition();
    }


    public void resetCorrectStates() {
        swordPosition.resetIsSwordInPerfectPosition();
        handPosition.resetIsHandInPerfectPosition();
        acceptedArea.resetIsSwordInAcceptedPosition();
    }


    public void setBlockPositionsActive() {
        positions.SetActive(true);
    }

    public void hideBlockPositions() {
        positions.SetActive(false);
    }
}