using UnityEngine;


public class HideGameobjectOnStart : MonoBehaviour {

    private void Awake() {
        this.gameObject.SetActive(false);
    }
}