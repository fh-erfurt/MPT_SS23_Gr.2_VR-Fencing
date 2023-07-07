using UnityEngine;


public class Billboard : MonoBehaviour {

    private static Transform tCam = null;


    private void Update () {

        if(!tCam) {
            if(!Camera.main) {
                return;
            }

            tCam = Camera.main.transform;
        }

        transform.LookAt(tCam.position, Vector3.up);
    }
}
