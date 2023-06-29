using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{
    private static float points;
    public int startPoints = 0;

    public static int lives;

    private void Start()
    {
        points = startPoints;
    }

    public static void ResetPoints()
    {
        points = 0;
    }

    public static float GetPoints()
    {
        return points;
    }

    public static void AddPoints(float amount)
    {
        points += amount;
    }

    public static void SubtractPoints(float amount)
    {
        points -= amount;
    }
}