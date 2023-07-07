using UnityEngine;


public class Points : Subject {

    private static int totalPoints;
    public int startPoints = 0;


    // Singleton
    public static Points instance;

    private void Awake() {
        // Singleton
        if (instance == null) {
            instance = this;
        }
    }


    private void Start() {
        totalPoints = startPoints;
    }


    public void ResetPoints() {
        totalPoints = 0;
        NotifyUIPointsObservers(totalPoints);
    }

    public int GetPoints() {
        return totalPoints;
    }


    public void AddPoints(int amount) {
        totalPoints += amount;
        Debug.Log("Points added |Total points: " + totalPoints);
        NotifyUIPointsObservers(totalPoints);
    }

    public void SubtractPoints(int amount) {
        totalPoints -= amount;

        if (totalPoints < 0) {
            totalPoints = 0;
        }
        Debug.Log("Points subtracted | Total points: " + totalPoints);
        NotifyUIPointsObservers(totalPoints);
    }
}