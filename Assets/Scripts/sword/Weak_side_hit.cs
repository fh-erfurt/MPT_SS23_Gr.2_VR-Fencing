using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weak_side_hit : Subject
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
            print("Weak side got hit!");
            Points.AddPoints(pointGain);
            print("You gained:" + pointGain + " points");
            NotifyObservers();
        }
    }
}