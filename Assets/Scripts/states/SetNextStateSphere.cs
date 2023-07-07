using UnityEngine;


public class SetNextStateSphere : Subject {

    [Header("Next Step on Activation")]
    [SerializeField] TrainingStateManager.nextStep nextStepOnActivation;

    [Header("Selection Sphere Timer")]
    public float requiredTimeInSphere = 1f;  // time the pointer has to stay in the sphere for an action to happen
    private float pointerInSphereTimer = 0f;
    private Transform selectionTimerSphere;  // automatically assign first child at start
    private float selectionTimerSphereScale = 0f;

    [Header("Selection Sphere Default Color")]
    public Color defaultColor         = new Color(1f, 1f, 1f, 0.1f);
    public Color defaultEmissionColor = new Color(0.54f, 0.54f, 0.54f);

    [Header("Selection Sphere Selection Color")]
    public Color selectionColor         = new Color(0f, 1f, 0f, 0.1f);
    public Color selectionEmissionColor = new Color(0f, 0.55f, 0f);

    private Material meshMaterial;



    private void Start() {
        selectionTimerSphere = transform.GetChild(0);

        meshMaterial = gameObject.GetComponent<Renderer>().material;
        SetDefaultColor();
    }


    //
    // Trigger
    private void OnTriggerEnter() {
        SetSelectionColor();
    }


    private void OnTriggerExit() {
        ResetTimer();
        SetDefaultColor();
    }


    private void OnTriggerStay() {

        pointerInSphereTimer += Time.deltaTime;

        IncreaseSelectionTimerSphereScale();

        // if sword is held long enough in the sphere -> trigger next step
        if (pointerInSphereTimer >= requiredTimeInSphere) {
            NotifyObservers(nextStepOnActivation);
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
        selectionTimerSphereScale = Mathf.Clamp(pointerInSphereTimer / requiredTimeInSphere, 0, 0.999f);
        selectionTimerSphere.localScale = new Vector3(selectionTimerSphereScale, selectionTimerSphereScale, selectionTimerSphereScale);
    }


    //
    // Colors
    private void SetDefaultColor() {
        meshMaterial.SetColor("_Color", defaultColor);
        meshMaterial.SetColor("_EmissionColor", defaultEmissionColor);
    }


    private void SetSelectionColor() {
        meshMaterial.SetColor("_Color", selectionColor);
        meshMaterial.SetColor("_EmissionColor", selectionEmissionColor * Mathf.LinearToGammaSpace(10f));
    }
}