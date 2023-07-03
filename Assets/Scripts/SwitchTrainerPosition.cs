using UnityEngine;


public class SwitchTrainerPosition : MonoBehaviour {

    [Header("Trainer-Model in Scene")]
    public Transform trainer;

    [Header("Position")]
    public float rotationYOffset = 0f;  // Main: y = 180
    private Transform trainerPosition;  // automatically assign parent at start

    [Header("Selection Sphere Timer")]
    public float requiredTimeInSphere = 1.5f;  // time the pointer has to stay in the sphere for an action to happen
    private float pointerInSphereTimer = 0f;
    private Transform selectionTimerSphere;  // automatically assign first child at start
    private float selectionTimerSphereScale = 0f;

    [Header("Selection Sphere Default Color")]
    public Color defaultColor         = new Color(1f, 1f, 1f, 0.1f);
    public Color defaultEmissionColor = new  Color(0.54f, 0.54f, 0.54f);

    [Header("Selection Sphere Selection Color")]
    public Color selectionColor         = new Color(0f, 1f, 0f, 0.1f);
    public Color selectionEmissionColor = new Color(0f, 0.55f, 0f);


    private Material meshMaterial;

    private bool positionWasSwitched = false;



    private void Start() {
        trainerPosition = transform.parent;

        selectionTimerSphere = transform.GetChild(0);

        meshMaterial = gameObject.GetComponent<Renderer>().material;
        SetDefaultColor();
    }


    // set trainer to the postition of the parent object of the sphere
    public void SetTrainerToPosition() {
        // only move trainer on x/z-plane
        trainer.position = new Vector3(trainerPosition.position.x, trainer.position.y, trainerPosition.position.z);
        trainer.rotation = Quaternion.Euler(0, rotationYOffset, 0);
    }


    //
    // Trigger
    private void OnTriggerEnter() {
        SetSelectionColor();
    }


    private void OnTriggerExit() {
        ResetTimer();
        positionWasSwitched = false;
        SetDefaultColor();
    }


    private void OnTriggerStay() {

        if (positionWasSwitched == true) {
                return;
        }

        pointerInSphereTimer += Time.deltaTime;

        IncreaseSelectionTimerSphereScale();

        // if sword is held long enough in the sphere -> move trainer to that position
        if (pointerInSphereTimer >= requiredTimeInSphere) {
            SetTrainerToPosition();
            positionWasSwitched = true;
            ResetTimer();
        }
    }


    //
    // Timer
    private void ResetTimer() {
        pointerInSphereTimer = 0f;
        selectionTimerSphere.localScale = new Vector3(0, 0, 0);
    }


    private void IncreaseSelectionTimerSphereScale() {
        selectionTimerSphereScale = pointerInSphereTimer / requiredTimeInSphere;
        selectionTimerSphere.localScale = new Vector3(selectionTimerSphereScale, selectionTimerSphereScale, selectionTimerSphereScale);
    }


    //
    // Colors
    private void SetDefaultColor() {
        meshMaterial.SetColor("_BaseColor", defaultColor);
        meshMaterial.SetColor("_EmissionColor", defaultEmissionColor * Mathf.LinearToGammaSpace(10f));
    }


    private void SetSelectionColor() {
        meshMaterial.SetColor("_BaseColor", selectionColor);
        meshMaterial.SetColor("_EmissionColor", selectionEmissionColor * Mathf.LinearToGammaSpace(10f));
    }
}
