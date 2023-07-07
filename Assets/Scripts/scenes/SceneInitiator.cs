using UnityEngine;
using TMPro;


public class SceneInitiator : MonoBehaviour {

    [Header("UI Texts")]
    public TMP_Text currentTraining;

    [Header("Training Types")]
    public TrainingSO[] trainingTypesSO;

    [Header("Trainer Animator")]
    public Animator animator;

    private MainManager mainManager;


    private void Start() {
        if (mainManager == null) {
            mainManager = MainManager.instance;
        }

        // speed of training is determined by the selected difficulty
        switch (mainManager.selectedDifficulty) {
            case MainManager.difficulty.easy:
                animator.speed = 1f;
                break;
            case MainManager.difficulty.medium:
                animator.speed = 1.5f;
                break;
            case MainManager.difficulty.hard:
                animator.speed = 2f;
                break;
        }

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
