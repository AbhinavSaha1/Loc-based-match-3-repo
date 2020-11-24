using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    moves,
    time
}
[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public GameObject movesLable;
    public GameObject timeLable;
    public Text counter;
    public EndGameRequirements requirements;
    private Board board;
    public int currentCounterValue;
    public float timerSeconds;
    public GameObject youWinPanel;
    public GameObject youLosePanel;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        SetGameType();
        SetupGame();
       
    }

    void SetGameType()
    {
        if(board.world!= null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    requirements = board.world.levels[board.level].endGameRequirements;
                }
            }
             
        }
    }
    void SetupGame()
    {
        currentCounterValue = requirements.counterValue;
        if (requirements.gameType== GameType.moves)
        {
            movesLable.SetActive(true);
            timeLable.SetActive(false);
        }
        if (requirements.gameType == GameType.time)
        {
            timerSeconds = 1;
            movesLable.SetActive(false);
            timeLable.SetActive(true);
        }
        counter.text = "" + currentCounterValue;
    }
    public void DecreaseCounterValue()
    {
        if(board.currentState!= GameState.pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;

            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }
            
    }

    public void LoseGame()
    {
        board.currentState = GameState.lose;
        youLosePanel.SetActive(true);
        Debug.Log("U lose");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
    }

    public void WinGame()
    {
        board.currentState = GameState.win;
        youWinPanel.SetActive(true);
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
    }
    // Update is called once per frame
    void Update()
    {
        if(requirements.gameType== GameType.time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if(timerSeconds<= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
