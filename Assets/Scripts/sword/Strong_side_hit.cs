using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strong_side_hit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("enemy_fence"))
        {
            print("Strong side got hit!");
        }
    }
}
