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
                    return "Training 1";
                case MainManager.trainingType.training_2:
                    return "Training 2";
                case MainManager.trainingType.training_3:
                    return "Training 3";
                default:
                    return "Training 1";
            }
        }
    }

}
