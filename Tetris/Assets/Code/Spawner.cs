using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class should spawn new Objects
public class Spawner : MonoBehaviour
{
    //All Blocks
    public GameObject[] groups;

    //All next objects in the spawn row
    public Queue<GameObject> myQueue = new Queue<GameObject> ();

    //The current activ falling block
    public GameObject currentActicObject;

    public void spawnNext()
    {
        //The List should always contain at least 3 object + one activ object
        for (int indx = myQueue.Count; indx < 4; indx++)
        {
            int i = Random.Range(0, groups.Length);

            GameObject newObject = Instantiate(groups[i], transform.position, Quaternion.identity);
            newObject.SetActive(false);
            myQueue.Enqueue(newObject);
        }
        GameObject nextObject = myQueue.Dequeue();
        nextObject.SetActive(true);
        nextObject.transform.position = transform.position;
        nextObject.transform.rotation = Quaternion.identity;
        nextObject.transform.localScale = new Vector3(1f,1f,1f);
        currentActicObject = nextObject;

    }
    // Start is called before the first frame update
    void Start()
    {
        spawnNext();
        FindObjectOfType<ObjectList>().UpdateQueue();
    }
}
