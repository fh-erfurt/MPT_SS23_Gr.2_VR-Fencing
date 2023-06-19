using UnityEngine;
using TMPro;


public class SceneInitiator : MonoBehaviour {

    [Header("UI Texts")]
    public TMP_Text currentTraining;
    public TMP_Text trainingScore;
    public TMP_Text trainingTime;

    [Header("Training Types")]
    public TrainingSO[] trainingTypesSO;


    // Defaults
    string trainingScoreDefault = "00000";
    string trainingTimeDefault = "15:00";

    private MainManager mainManager;

    private void Start() {
        mainManager = MainManager.instance;

        currentTraining.text = getTrainingName() + " (" + mainManager.selectedDifficulty.ToString() + ")";
        trainingScore.text = trainingScoreDefault;
        trainingTime.text = trainingTimeDefault;
    }


    private string getTrainingName() {
        foreach (TrainingSO training in trainingTypesSO) {
            if (training.trainingType == mainManager.selectedTraining) {
                return training.name;
            }
        }
        return "Training name doesn't exist";
    }
}
