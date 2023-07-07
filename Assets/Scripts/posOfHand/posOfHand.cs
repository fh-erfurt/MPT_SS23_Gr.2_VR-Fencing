using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class posOfHand : MonoBehaviour
{
    public bool handOnPosition;

    // Start is called before the first frame update
    void Start()
    {
        handOnPosition = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("hand"))
        {
            Debug.Log("Hand entered right position!");
            handOnPosition = true;
        }
    }
}