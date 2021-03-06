﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public int score;
    public Text scoreText;
    private Board board;
    public Image scoreBar;


    void Start()
    {
        board= FindObjectOfType<Board>();
    }
    void Update()
    {
        scoreText.text = ""+ score;
    }
    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        if(board!= null && scoreBar!= null)
        {
            int length= board.scoreGoals.Length;
            scoreBar.fillAmount= (float)score/(float)board.scoreGoals[length-1];
        }
    }
   
}
