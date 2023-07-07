using UnityEngine;


public class posOfHand : MonoBehaviour {

    [Header("Default Colors")]
    public Color defaultColor         = new Color(1f, 1f, 1f, 0.1f);
    public Color defaultEmissionColor = new Color(0.54f, 0.54f, 0.54f);

    [Header("Correct Position Colors")]
    public Color selectionColor         = new Color(0f, 1f, 0f, 0.1f);
    public Color selectionEmissionColor = new Color(0f, 0.25f, 0f);

    private Material meshMaterial;


    private bool isHandOnCorrectPosition;


    private void Start() {
        isHandOnCorrectPosition = false;

        meshMaterial = gameObject.GetComponent<Renderer>().material;
        SetDefaultColor();
    }


    //
    // Trigger
    private void OnTriggerEnter(Collider collider) {

        if (collider.gameObject.CompareTag("hand")) {
            Debug.Log("Hand entered correct position!");
            isHandOnCorrectPosition = true;
            SetSelectionColor();
        }
    }


    private void OnTriggerExit(Collider collider) {

        if (collider.gameObject.CompareTag("hand")) {
            Debug.Log("Hand left correct position!");
            isHandOnCorrectPosition = false;
            SetDefaultColor();
        }
    }


    //
    // Position
    public bool IsHandOnCorrectPosition() {
        return isHandOnCorrectPosition;
    }


    public void resetIsHandOnCorrectPosition() {
        isHandOnCorrectPosition = false;
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


    private void OnDisable() {
        SetDefaultColor();
        isHandOnCorrectPosition = false;
    }
}