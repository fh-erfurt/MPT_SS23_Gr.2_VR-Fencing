using UnityEngine;


public class posInAcceptedArea : MonoBehaviour {

    [Header("Default Colors")]
    public Color defaultColor         = new Color(1f, 1f, 1f, 0.1f);
    public Color defaultEmissionColor = new Color(0.54f, 0.54f, 0.54f);

    [Header("Correct Position Colors")]
    public Color selectionColor         = new Color(0f, 1f, 0f, 0.1f);
    public Color selectionEmissionColor = new Color(0f, 0.25f, 0f);

    private Material meshMaterial;

    private bool isHandInAcceptedPosition;
    private bool isBladeInAcceptedPosition;


    private void Start() {
        isHandInAcceptedPosition = false;
        isBladeInAcceptedPosition= false;

        meshMaterial = gameObject.GetComponent<Renderer>().material;
        SetDefaultColor();
    }


    //
    // Trigger
    private void OnTriggerEnter(Collider collider) {

        if (collider.gameObject.CompareTag("hand")) {
            Debug.Log("Hand entered accepted position!");
            isHandInAcceptedPosition = true;
        }

        if (collider.gameObject.CompareTag("Sword")) {
            Debug.Log("Sword entered accepted position!");
            isBladeInAcceptedPosition = true;
        }

        if (isSwordInAcceptedPosition()) {
            SetSelectionColor();
        }
    }


    private void OnTriggerExit(Collider collider) {

        if (collider.gameObject.CompareTag("hand")) {
            Debug.Log("Hand left accepted position!");
            isHandInAcceptedPosition = false;
            SetDefaultColor();
        }

        if (collider.gameObject.CompareTag("Sword")) {
            Debug.Log("Sword left accepted position!");
            isBladeInAcceptedPosition = false;
            SetDefaultColor();
        }
    }


    //
    // Position
    public bool isSwordInAcceptedPosition() {
        return isHandInAcceptedPosition && isBladeInAcceptedPosition;
    }


    public void resetIsSwordInAcceptedPosition() {
        isHandInAcceptedPosition = false;
        isBladeInAcceptedPosition= false;
    }


    private void OnDisable() {
        SetDefaultColor();
        resetIsSwordInAcceptedPosition();
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