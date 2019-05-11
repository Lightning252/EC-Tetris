using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

//Main Class for the interaction. It handels all keys
public class Group : MonoBehaviour
{
    //All audio clips
    public AudioClip lightning;
    public AudioClip timeStop;
    public AudioClip timeStart;
    public AudioClip levelUp;
    public AudioClip clearLine;

    private AudioSource audioSource;

    //All needed game objects 
    private Spawner spawn;
    private ObjectList objectList;
    private Score dispalyScore;

    //List with all blocks that are already placed
    private List<GameObject> allObjects;
    private GameObject lastObject;

    //The fall time
    private float downTime = 1f;
    private float oldDownTime = 0f;

    //Last block fall time
    private float lastFall = 0;

    //Timer for the next level up
    private float diffTime = 0;

    //Counter for the slow motion ability. After 5 blocks back to normal time
    private int dropCounter = 0;
    private int slowCounter = 0;

    //Counter for the Score (grid spaces counter for the down key needed for the Score)
    private int softDropCount = 0;

    //Checks if the currently active object has a valid grid position
    private bool isValidGridPos()
    {
        foreach (Transform child in spawn.currentActicObject.transform)
        {
            Vector2 vector = Grid.roundVec2(child.position);

            if (!Grid.isInsideBorder(vector))
            {
                return false;
            }

            if (Grid.grid[(int)vector.x, (int)vector.y] != null &&
                Grid.grid[(int)vector.x, (int)vector.y].parent != spawn.currentActicObject.transform)
            {
                return false;
            }

        }
        return true;
    }


    //update the grid delete and add the new position to the grid
    private void updateGrid()
    {
        for (int counterHeight = 0; counterHeight < Grid.height; counterHeight++)
        {
            for (int counterWight = 0; counterWight < Grid.witdh; counterWight++)
            {
                if (Grid.grid[counterWight, counterHeight] != null && 
                    Grid.grid[counterWight, counterHeight].parent == spawn.currentActicObject.transform)
                {
                    Grid.grid[counterWight, counterHeight] = null;
                }
                        
                   
            }
                
        }
           

        foreach (Transform child in spawn.currentActicObject.transform)
        {
            Vector2 vector = Grid.roundVec2(child.position);
            Grid.grid[(int)vector.x, (int)vector.y] = child;
        }
    }

    //Fuction to delete one block from the grid
    private void deleteBlock(GameObject deleteObject)
    {
        foreach (Transform child in deleteObject.transform)
        {
            Vector2 vector = Grid.roundVec2(child.position);
            Grid.grid[(int)vector.x, (int)vector.y] = null;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //Get all Objects
        spawn = gameObject.GetComponent<Spawner>();
        objectList = FindObjectOfType<ObjectList>();
        dispalyScore = FindObjectOfType<Score>();
        allObjects = new List<GameObject>();
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.volume = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if we have valid objects
        if (spawn == null || spawn.currentActicObject == null) return;

        //Key press event for the left movement
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            spawn.currentActicObject.transform.position += new Vector3(-1, 0, 0);

            if (isValidGridPos())
            {
                updateGrid();
            }
            else
            {
                spawn.currentActicObject.transform.position += new Vector3(1, 0, 0);
            }
                
        }
        //Key press event for the right movement
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            spawn.currentActicObject.transform.position += new Vector3(1, 0, 0);


            if (isValidGridPos())
            {
                updateGrid();
            }
            else
            {
                spawn.currentActicObject.transform.position += new Vector3(-1, 0, 0);
            }
        }
        //Key press event for the rotation
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            spawn.currentActicObject.transform.Rotate(0, 0, -90);

            if (isValidGridPos())
            {
                updateGrid();
            }
            else
            {
                spawn.currentActicObject.transform.position += new Vector3(0, 0, 90);
            }
        }
        //Key press event for the down movement
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            softDropCount++;
            CalculateDown();
        }
        //Time event for the down movement
        else if (Time.time - lastFall >= downTime)
        {
            CalculateDown();

            lastFall = Time.time;
        }

        //Special key event for the slow motion
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //Slow Time
            int needScore = 1000 * (Score.level + 1);
            if (Score.level > 0 && Score.score > needScore)
            {
                Score.score -= needScore;
                slowCounter = dropCounter;
                oldDownTime = downTime;
                downTime = 1f;
                diffTime = 0f;

                audioSource.clip = timeStop;
                audioSource.Play();
            }

        }
        //Special key for the delete last block event
        else if (Input.GetKeyDown(KeyCode.E))
        {
            int needScore = 300 * (Score.level + 1);
            if (lastObject != null && Score.score > needScore)
            {
                deleteBlock(lastObject);
                allObjects.Remove(lastObject);
                Destroy(lastObject);
                lastObject = null;
                Score.score -= needScore;
                diffTime = 0f;

                audioSource.clip = lightning;
                audioSource.Play();
            }
        }

        //Save the current block
        if (Input.GetKeyDown(KeyCode.W))
        {
            deleteBlock(spawn.currentActicObject);
            objectList.SaveObject(spawn.currentActicObject);
        }
        //Get the saved block
        else if (Input.GetKeyDown(KeyCode.S))
        {
            deleteBlock(spawn.currentActicObject);
            objectList.GetSaveObject();
        }
        //Checks if the current active object has a valid grid position
        else if (!isValidGridPos())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log("GAME OVER");
        }
        diffTime += Time.deltaTime;
    }

    //Fuction to calculate the down movement of the block
    private void CalculateDown()
    {
        spawn.currentActicObject.transform.position += new Vector3(0, -1, 0);

        //Checks for a valid grid position
        if (isValidGridPos())
        {
            updateGrid();
        }
        else
        {
            //Set the variabels
            allObjects.Add(spawn.currentActicObject);
            lastObject = spawn.currentActicObject;
            spawn.currentActicObject.transform.position += new Vector3(0, 1, 0);

            int currentScore;
            int deleteRows = Grid.deleteAllFullRows();

            //If rows are deleted play a sound and delete also all emty blocks
            if (deleteRows > 0)
            {
                deleteBlocks();

                audioSource.clip = clearLine;
                audioSource.Play();
            }

            //Calculate the score
            switch (deleteRows)
            {
                case 1:
                    currentScore = 40 * (Score.level + 1);
                    break;
                case 2:
                    currentScore = 100 * (Score.level + 1);
                    break;
                case 3:
                    currentScore = 300 * (Score.level + 1);
                    break;
                case 4:
                    currentScore = 1200 * (Score.level + 1);
                    break;

                default:
                    currentScore = 0;
                    break;
            }

            //Add to the score the number of down pressed grid spaces
            if (softDropCount > 0)
            {
                currentScore += softDropCount - 1;
            }
            softDropCount = 0;

            //Display the score
            dispalyScore.AddScore(currentScore);

            //Spawn next block
            spawn.spawnNext();

            dropCounter++;

            //Update the list with the next objects
            objectList.UpdateQueue();

            //Ends the Slow Motion after 5 blocks
            if (slowCounter > 0 && dropCounter > slowCounter + 5)
            {
                print(slowCounter.ToString());
                slowCounter = 0;
                downTime = oldDownTime;
                oldDownTime = 0f;
                diffTime = 0f;

                audioSource.clip = timeStart;
                audioSource.Play();
            }

            //Change the level and makes the game harder
            if (diffTime > 20f && downTime > 0.2f && slowCounter == 0)
            {
                print(diffTime.ToString());
                downTime -= 0.1f;
                diffTime = 0f;
                dispalyScore.AddLevel(1);

                audioSource.clip = levelUp;
                audioSource.Play();
            }

            if (!isValidGridPos())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                Debug.Log("GAME OVER");
            }

            foreach (Transform child in spawn.currentActicObject.transform)
            {
                Vector2 vector = Grid.roundVec2(child.position);
                Grid.grid[(int)vector.x, (int)vector.y] = child;
            }

        }
    }

    //Delete all emty game objects
    private void deleteBlocks()
    {
        List<GameObject> currentList = new List<GameObject>();
        foreach (GameObject currentObject in allObjects)
        {
            if (currentObject.transform.childCount == 0)
            {
                Destroy(currentObject);
            }
            else
            {
                currentList.Add(currentObject);
            }
        }
        allObjects = currentList;
    }
}
