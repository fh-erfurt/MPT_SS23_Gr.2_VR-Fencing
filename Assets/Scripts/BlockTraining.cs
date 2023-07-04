using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class manages the training; it spawns the needed blocks to the given action
// Furthermore this script will handle the calculations needed to recognize the rotation of the collision

public class BlockTraining : MonoBehaviour
{

    private GameObject currentBlockingBlock;

    public GameObject blockingPrefab_LU;
    public GameObject blockingPrefab_LD;
    public GameObject blockingPrefab_RU;
    public GameObject blockingPrefab_RD;


    private string[] blockingTraining = { "training_blockLD", "training_blockRD", "training_blockLU", "training_blockRU" };


    void Start()
    {
        startBlockingTraining();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("blockingBlock"))
        {
            Debug.Log("box" + collision.gameObject.name + "was hit!");
            if (collision.gameObject == currentBlockingBlock)
            {
                Destroy(currentBlockingBlock);
                startBlockingTraining();
            }
        }
    }

    void startBlockingTraining()
    {
        int randomIndex = Random.Range(0, blockingTraining.Length);
        string functionName = blockingTraining[randomIndex];
        Invoke(functionName, 0f);
    }

    void training_blockLD()
    {
        GameObject newBlock = SpawnBlock(blockingPrefab_LD, Vector3.zero);
        currentBlockingBlock = newBlock;
    }

    void training_blockRD()
    {
        GameObject newBlock = SpawnBlock(blockingPrefab_RD, Vector3.zero);
        currentBlockingBlock = newBlock;
    }

    void training_blockLU()
    {
        GameObject newBlock = SpawnBlock(blockingPrefab_LU, Vector3.zero);
        currentBlockingBlock = newBlock;
    }

    void training_blockRU()
    {
        GameObject newBlock = SpawnBlock(blockingPrefab_RU, Vector3.zero);
        currentBlockingBlock = newBlock;
    }


    //TODO: Die Blöcke spawnen noch nicht ganz an den richtigen STellen; umschreiben doer richtig basteln. kA wodran das aktuell liegt
    GameObject SpawnBlock(GameObject prefab, Vector3 position)
    {
        GameObject newBlock = Instantiate(prefab, position, Quaternion.identity);
        Vector3 prefabPosition = prefab.transform.position;
        float xOffset = -0.5f;
        float yOffset =  1.0f;
        newBlock.transform.position = new Vector3(prefabPosition.x + xOffset, prefabPosition.y + yOffset, 13.694f);
        return newBlock;
    }
}

