using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureEventProcessor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGestureCompleted(GestureCompletionData gestureCompletionData)
    {
        if (gestureCompletionData.gestureID < 0) 
        {
            string errorMessage = GestureRecognition.getErrorMessage(gestureCompletionData.gestureID);
        }
        if (gestureCompletionData.similarity >= 0.5)
        {
            Debug.Log("Geste: " + gestureCompletionData.gestureName);
        }
    }
}
