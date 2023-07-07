using UnityEngine;
using TMPro;


public class SceneInitiator : MonoBehaviour {

    [Header("UI Texts")]
    public TMP_Text currentTraining;

    [Header("Training Types")]
    public TrainingSO[] trainingTypesSO;


    private MainManager mainManager;


    private void Start() {
        mainManager = MainManager.instance;

        currentTraining.text = getTrainingName() + " (" + mainManager.selectedDifficulty.ToString() + ")";
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
