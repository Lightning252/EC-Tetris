using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//This class should display the score
public class Score : MonoBehaviour
{
    //The Score and the level
    static public int score = 0;
    static public int level = 0;

    //All txt fields
    private Text scoreText;
    private Text levelText;
    private Text lightText;
    private Text timeText;


    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Transform childScore = canvas.transform.Find("ScoreText");
            Transform childLevel = canvas.transform.Find("LevelText");
            Transform childLight = canvas.transform.Find("LightText");
            Transform childTime = canvas.transform.Find("TimeText");

            scoreText = childScore.GetComponent<Text>();
            levelText = childLevel.GetComponent<Text>();
            lightText = childLight.GetComponent<Text>();
            timeText = childTime.GetComponent<Text>();

            scoreText.text = "Score: " + score;
            levelText.text = "Level: " + level;
            lightText.text = "" + (300 * (Score.level + 1));
            timeText.text = "" + (1000 * (Score.level + 1));


        }
    }

    //Function to add a level
    public void AddLevel(int level)
    {
        Score.level += level;
        levelText.text = "Level: " + Score.level;
        lightText.text = "" + (300 * (Score.level + 1));
        timeText.text = "" + (1000 * (Score.level + 1));
    }

    //function to add a score
    public void AddScore(int score)
    {
        Score.score += score;
        scoreText.text = "Score: " + Score.score;
    }
}
