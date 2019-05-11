using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The class that should display the the all next blocks and the saved block
public class ObjectList : MonoBehaviour
{
    private Spawner Spawner;
    private GameObject saveObject;


    // Start is called before the first frame update
    void Start()
    {
        Spawner = FindObjectOfType<Spawner>();
    }

    //Update the visualisation of all next blocks
    public void UpdateQueue()
    {
        float posY = transform.position.y - 4;

        //Go through all next objects and arrange them correctly
        foreach(GameObject gameObject in Spawner.myQueue)
        {
            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            gameObject.transform.position = new Vector3(transform.position.x, posY, 0f);
            posY -= 3;
            gameObject.SetActive(true);
        }
    }

    //Function to save one Object
    public void SaveObject(GameObject gameObject)
    {
        if (saveObject == null)
        {
            saveObject = gameObject;
            saveObject.transform.position = new Vector3(transform.position.x, transform.position.y -1.5f, 0f);
            saveObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Spawner.spawnNext();

            UpdateQueue();

        }
    }

    //Function to get the currently saved object
    public void GetSaveObject()
    {
        if (saveObject != null)
        {
            Queue<GameObject> myQueue = new Queue<GameObject>();
            myQueue.Enqueue(Spawner.currentActicObject);

            //Set all spwan infos correctly
            Spawner.currentActicObject = saveObject;
            saveObject.transform.position = Spawner.transform.position;
            saveObject.transform.localScale = new Vector3(1f, 1f, 1f);

            foreach(GameObject current in Spawner.myQueue)
            {
                myQueue.Enqueue(current);
            }
            Spawner.myQueue = myQueue;
            UpdateQueue();
            saveObject = null;
        }
    }
}
