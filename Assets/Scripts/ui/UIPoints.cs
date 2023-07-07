using UnityEngine;
using TMPro;


public class UIPoints : MonoBehaviour, IObserver {

    public TMP_Text pointsText;

    private Points _pointsSubject;


    private void Start() {
        _pointsSubject = Points.instance;
        _pointsSubject.AddObserver(this);
        pointsText.text = "Score: 0";
    }

    //
    // Observer
    public void OnNotify(int totalPoints) {
        pointsText.text = "Score: " + totalPoints.ToString();
    }


    // not important
    public void OnNotify(TrainingStateManager.nextStep n) {}
    public void OnNotify(TrainingStateManager.swordSide s) {}
}