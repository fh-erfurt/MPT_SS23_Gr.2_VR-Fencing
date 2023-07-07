public interface IObserver {
    // subject uses this function to communicate with the observer
    public void OnNotify(TrainingStateManager.nextStep nextStep);

    // points ui
    public void OnNotify(int totalPoints);
}