using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class posManager : MonoBehaviour
{
    public posOfSword posOfSword_var;
    public posOfHand posOfHand_var;

    // Start is called before the first frame update
    void Start()
    { 

    }

    // Update is called once per frame
    void Update()
    {
        if (posOfSword_var.swordOnPosition == true && posOfHand_var.handOnPosition == true)
        {
            Debug.Log("right position for blocking!");
        }
    }
}
