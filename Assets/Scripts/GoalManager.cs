using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoals
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}
public class GoalManager : MonoBehaviour
{
    public BlankGoals[] levelGoals;
    public List<GoalPannel> currentGoals = new List<GoalPannel>();
    public GameObject goalPrefab;
    public GameObject goalGameParent;
    private Board board;
    private EndGameManager endGameManager;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        endGameManager = FindObjectOfType<EndGameManager>();
        GetGoals();
        SetupGoals();
    }

    void GetGoals()
    {
        if(board.world != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    levelGoals = board.world.levels[board.level].levelGoals;
                    for(int i=0; i< levelGoals.Length; i++)
                    {
                        levelGoals[i].numberCollected = 0;
                    }
                }
            }
             
        }
    }

    private void SetupGoals()
    {
        for(int i=0; i<levelGoals.Length; i++)
        {
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            GoalPannel panel = gameGoal.GetComponent<GoalPannel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }
    public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for(int i=0; i<levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            if(levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
            if(goalsCompleted>= levelGoals.Length)
            {
                Debug.Log("Good Job!");
                endGameManager.WinGame();
            }
        }
    }
    public void CompareGoals(string goalToCompare)
    {
        for(int i=0; i< levelGoals.Length; i++)
        {
            if(goalToCompare== levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }
}
