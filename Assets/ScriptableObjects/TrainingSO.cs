using UnityEngine;


[CreateAssetMenu(fileName = "TrainingSO", menuName = "ScriptableObjects/TrainingSO")]
public class TrainingSO : ScriptableObject {

    [Header("Training Type")]
    public MainManager.trainingType trainingType = MainManager.trainingType.training_1;

    [Header("Training Difficulty")]
    public MainManager.difficulty trainingDifficulty = MainManager.difficulty.easy;

    public string trainingName {
        get {
            switch (trainingType) {
                case MainManager.trainingType.training_1:
                    return "Deflecting Training";
                case MainManager.trainingType.training_2:
                    return "Attacking Training";
                case MainManager.trainingType.training_3:
                    return "Training 3 (empty)";
                default:
                    return "Deflecting Training";
            }
        }
    }

}
