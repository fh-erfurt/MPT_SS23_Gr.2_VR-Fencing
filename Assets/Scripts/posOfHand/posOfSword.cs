using UnityEngine;


public class posOfSword : MonoBehaviour {

    [Header("Default Colors")]
    public Color defaultColor         = new Color(1f, 1f, 1f, 0.1f);
    public Color defaultEmissionColor = new Color(0.54f, 0.54f, 0.54f);

    [Header("Correct Position Colors")]
    public Color selectionColor         = new Color(0f, 1f, 0f, 0.1f);
    public Color selectionEmissionColor = new Color(0f, 0.25f, 0f);

    private Material meshMaterial;

    private bool isSwordInPerfectPosition;


    private void Start() {
        isSwordInPerfectPosition = false;

        meshMaterial = gameObject.GetComponent<Renderer>().material;
        setDefaultColor();
    }


    private void OnTriggerEnter(Collider collider) {

        if (collider.gameObject.CompareTag("Sword")) {
            Debug.Log("Sword entered perfect position!");
            isSwordInPerfectPosition = true;
            setSelectionColor();
        }
    }


    private void OnTriggerExit(Collider collider) {

        if (collider.gameObject.CompareTag("Sword")) {
            Debug.Log("Sword left perfect position!");
            isSwordInPerfectPosition = false;
            setDefaultColor();
        }
    }


    public bool IsSwordInPerfectPosition() {
        return isSwordInPerfectPosition;
    }


    public void resetIsSwordInPerfectPosition() {
        isSwordInPerfectPosition = false;
    }


    //
    // Colors
    private void setDefaultColor() {
        meshMaterial.SetColor("_Color", defaultColor);
        meshMaterial.SetColor("_EmissionColor", defaultEmissionColor);
    }


    private void setSelectionColor() {
        meshMaterial.SetColor("_Color", selectionColor);
        meshMaterial.SetColor("_EmissionColor", selectionEmissionColor * Mathf.LinearToGammaSpace(10f));
    }


    private void OnDisable() {
        setDefaultColor();
        isSwordInPerfectPosition = false;
    }
}