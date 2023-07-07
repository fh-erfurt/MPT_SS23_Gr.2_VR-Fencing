using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class manages the training; it spawns the needed blocks to the given action
// Furthermore this script will handle the calculations needed to recognize the rotation of the collision

public class BlockTraining : MonoBehaviour
{

    private GameObject currentBlockingBlock;

    bool isActivated;

    public GameObject blockingPrefab_LU;
    public GameObject blockingPrefab_RU;


    //private string[] blockingTraining = { "training_blockLD", "training_blockRD", "training_blockLU", "training_blockRU" };


    void Start()
    {
        blockingPrefab_LU.SetActive(false);
        blockingPrefab_RU.SetActive(false);
       // isActivated = false;
        startBlockingTraining();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("blockingBlockLU"))
        {
            Debug.Log("box" + collision.gameObject.name + "was hit!");
            blockingPrefab_LU.SetActive(false);
            
        } else if(collision.gameObject.CompareTag("blockingBlockRU"))
        {
            Debug.Log("box" + collision.gameObject.name + "was hit!");
            blockingPrefab_RU.SetActive(false);
            
        }
    }

    void startBlockingTraining()
    {

        training_blockLU();
        training_blockRU();
        training_blockLU();
        training_blockRU();

    }


    void training_blockLU()
    {
            blockingPrefab_LU.SetActive(true);
            //currentBlockingBlock = blockingPrefab_LU;
        }
    

    void training_blockRU()
    {
        blockingPrefab_RU.SetActive(true);
        //currentBlockingBlock = blockingPrefab_RU;
    }


 

    //TODO: Die Blöcke spawnen noch nicht ganz an den richtigen Stellen; umschreiben oder richtig basteln. kA wodran das aktuell liegt --> evt Fehler wegen World-Position und 
/*GameObject SpawnBlock(GameObject prefab, Vector3 position)
{
    GameObject newBlock = Instantiate(prefab, position, Quaternion.identity);
    Vector3 prefabPosition = prefab.transform.position;
    float xOffset = -0.5f;
    float yOffset =  1.0f;
    newBlock.transform.position = new Vector3(prefabPosition.x + xOffset, prefabPosition.y + yOffset, 13.694f);
    return newBlock;*/
}


