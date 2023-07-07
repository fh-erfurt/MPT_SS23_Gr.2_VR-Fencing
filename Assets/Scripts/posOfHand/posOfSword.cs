using UnityEngine;


public class posOfSword : MonoBehaviour {

    [Header("Default Colors")]
    public Color defaultColor         = new Color(1f, 1f, 1f, 0.1f);
    public Color defaultEmissionColor = new Color(0.54f, 0.54f, 0.54f);

    [Header("Correct Position Colors")]
    public Color selectionColor         = new Color(0f, 1f, 0f, 0.1f);
    public Color selectionEmissionColor = new Color(0f, 0.25f, 0f);

    private Material meshMaterial;

    private bool isSwordOnCorrectPosition;


    private void Start() {
        isSwordOnCorrectPosition = false;

        meshMaterial = gameObject.GetComponent<Renderer>().material;
        SetDefaultColor();
    }


    private void OnTriggerEnter(Collider collider) {

        if (collider.gameObject.CompareTag("Sword")) {
            Debug.Log("Sword entered correct position!");
            isSwordOnCorrectPosition = true;
            SetSelectionColor();
        }
    }


    private void OnTriggerExit(Collider collider) {

        if (collider.gameObject.CompareTag("Sword")) {
            Debug.Log("Sword left correct position!");
            isSwordOnCorrectPosition = false;
            SetDefaultColor();
        }
    }


    public bool IsSwordOnCorrectPosition() {
        return isSwordOnCorrectPosition;
    }


    public void resetIsHandOnCorrectPosition() {
        isSwordOnCorrectPosition = false;
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
        isSwordOnCorrectPosition = false;
    }
}