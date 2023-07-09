using UnityEngine;


public class MainManager : MonoBehaviour {

    // enums
    public enum difficulty { easy, medium, hard };

    public enum trainingType { training_1, training_2, training_3 };


    public difficulty selectedDifficulty { get; set; } = difficulty.easy;
    public trainingType selectedTraining { get; set; } = trainingType.training_1;

    // Singleton
    public static MainManager instance;


    private void Awake() {
        // only let one instance exist at a time
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        // keep MainManager alive through scene-loading
        DontDestroyOnLoad(gameObject);
    }
}