using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour {

    [Header("Name of the Training-Scene")]
    public string trainingSceneName = "Training";


    private MainManager mainManager;


    private void Start() {
        mainManager = MainManager.instance;
    }


    public void loadScene() {
        SceneManager.LoadScene(trainingSceneName);
    }


    public void SetDifficultyEasy()   => mainManager.selectedDifficulty = MainManager.difficulty.easy;
    public void SetDifficultyMedium() => mainManager.selectedDifficulty = MainManager.difficulty.medium;
    public void SetDifficultyHard()   => mainManager.selectedDifficulty = MainManager.difficulty.hard;

    public void SetTraining1() => mainManager.selectedTraining = MainManager.trainingType.training_1;
    public void SetTraining2() => mainManager.selectedTraining = MainManager.trainingType.training_2;
    public void SetTraining3() => mainManager.selectedTraining = MainManager.trainingType.training_3;
}
