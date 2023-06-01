using UnityEngine;
using UnityEngine.InputSystem;


public class AnimateHandOnInput : MonoBehaviour {

    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gribAnimationAction;

    public Animator handAnimator;


    private void Update() {

        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);
        Debug.Log("Trigger R: " + triggerValue);

        float gribValue = gribAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gribValue);
        Debug.Log("Grib R: " + gribValue);
    }
}
