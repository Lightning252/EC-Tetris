using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EndMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Get the text fields and add the score/level
        Transform childScore = transform.Find("Score");
        Transform childLevel = transform.Find("Level");

        childScore.GetComponent<Text>().text = "Score: " + Score.score;
        childLevel.GetComponent<Text>().text = "Level: " + Score.level;
    }

    public void QuitGame()
    {
        //Don't work in Editor
        Application.Quit();
    }
}
