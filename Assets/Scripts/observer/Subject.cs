using System.Collections.Generic;
using UnityEngine;


public abstract class Subject : MonoBehaviour {

    // collection of all observers of this subject
    private List<IObserver> _observers = new List<IObserver>();


    public void AddObserver(IObserver observer) {
        _observers.Add(observer);
    }


    public void RemoveObserver(IObserver observer) {
        _observers.Remove(observer);
    }


    protected void NotifyObservers(TrainingStateManager.nextStep nextStep) {
        _observers.ForEach((_observer) => {
            _observer.OnNotify(nextStep);
        });
    }

    // for sword
    protected void NotifyObservers(TrainingStateManager.swordSide swordSide) {
        _observers.ForEach((_observer) => {
            _observer.OnNotify(swordSide);
        });
    }
}