using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class posOfSword : MonoBehaviour
{
    public bool swordOnPosition;

    // Start is called before the first frame update
    void Start()
    {
        swordOnPosition = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sword"))
        {
            Debug.Log("Sword entered right position!");
            swordOnPosition = true;
        }
    }
}
