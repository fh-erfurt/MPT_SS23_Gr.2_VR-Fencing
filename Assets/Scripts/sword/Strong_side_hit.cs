using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strong_side_hit : MonoBehaviour
{
    private float pointGain;
    public float points = 100;

    private void Start()
    {
        pointGain = points;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy_fence"))
        {
            print("Strong side got hit!");
            Points.AddPoints(pointGain);
            print("You gained:" + pointGain + " points");
        }
    }
}