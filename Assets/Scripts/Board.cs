using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}
public enum TileKind
{
    Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    [Header("Scriptable object stuff")]
    public World world;
    public int level;

    public GameState currentState = GameState.move;
    [Header("Board Dimensions")]
    public int width;
    public int height;
    public int offset;
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;

    [Header("Layout")]
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    private BackgroundTIle[,] breakableTile;
    public GameObject[] dots;
    public GameObject destroyEffect;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;
    public int basePieceValue= 20;
    public int streakValue= 1;
    public ScoreManager scoreManager;
    public SoundManager soundManager;
    private GoalManager goalManager;
    public float refillDelay= 0.5f;
    public int[] scoreGoals;
    // Start is called before the first frame update

    private void Awake()
    {
        if(world!= null)
        {
            if(world.levels[level]!= null)
            {
                if(level < world.levels.Length)
                {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    dots = world.levels[level].dots;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                }
                
            }
        }
    }
    void Start()
    {
        soundManager= FindObjectOfType<SoundManager>();
        scoreManager= FindObjectOfType<ScoreManager>();
        goalManager = FindObjectOfType<GoalManager>();
        breakableTile = new BackgroundTIle[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        Setup();
        currentState = GameState.pause;
    }

    private void GenerateBlankSpaces()
    {
        for(int i=0; i<boardLayout.Length; i++)
        {
            if(boardLayout[i].tileKind== TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }
    private void GenerateBreakabletTile()
    {
        for(int i=0; i<boardLayout.Length; i++)
        {
            if(boardLayout[i].tileKind== TileKind.Breakable)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x ,boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTile[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTIle>();
             }
        }
    }

    private void Setup()
    {
        GenerateBlankSpaces();
        GenerateBreakabletTile();
        for (int i=0; i< width; i++)
        {
            for (int j = 0; j < height; j++) 
            {
                if(!blankSpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    Vector2 tilePosition= new Vector2(i,j);
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + i + "," + j + ")";
                    int dotsToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotsToUse]) && maxIterations < 100)
                    {
                        dotsToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;
                    GameObject dot = Instantiate(dots[dotsToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "(" + i + "," + j + ")";
                    allDots[i, j] = dot;
                }
               
            }
        }
            
    }
 
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allDots[column-1, row]!= null && allDots[column-2,row]!= null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }

            if (allDots[column, row-1] != null && allDots[column, row-2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
                
        }
        else if(column <=1 || row <= 1)
        {
            if(row > 1)
            {
                if (allDots[column, row-1] != null && allDots[column, row-2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }      
               
            }
            if(column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
                     
            }
        }
        return false;
    }

    private void CheckToMakeBombs()
    {
        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {
            findMatches.CheckBombs();
        }
        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        {
            if(currentDot!= null)
            {
                if(currentDot.isMatched)
                {
                    if(!currentDot.isColorBomb)
                    {
                        currentDot.isMatched = false;
                        currentDot.MakeColorBomb();
                    }
                }    
            }
            else if(currentDot.otherDot!= null)
            {
                Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched)
                {
                    if (!otherDot.isColorBomb)
                    {
                        otherDot.isMatched = false;
                        otherDot.MakeColorBomb();
                    }
                }
            }
        }
    }    

    private void DestroymatchesAt(int column, int row)
    {
        if(allDots[column, row].GetComponent<Dot>().isMatched)
        {
            if(findMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }
            
            if(breakableTile[column,row]!= null)
            {
                breakableTile[column, row].TakeDamage(1);
                if(breakableTile[column, row].healthpoints >= 0)
                {
                    breakableTile[column, row] = null;
                }
            }

            if(goalManager!= null)
            {
                goalManager.CompareGoals(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }
            if (soundManager!= null)
            {
                soundManager.PlayRandomDestroyNoise();
            }
            GameObject particle= Instantiate(destroyEffect, allDots[column,row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;
        }
    }
    public void DestroyMatches()
    {
        for (int i=0; i<width; i++)
        {
            for(int j=0; j< height; j++)
            {
                if(allDots[i,j]!= null)
                {
                    DestroymatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo2());
    }
    private IEnumerator DecreaseRowCo2()
    {
        for(int i= 0; i< width; i++)
        {
            for(int j= 0; j< height; j++)
            {
                if(!blankSpaces[i,j] && allDots[i, j]== null)
                {
                    for(int k= j+1; k<height; k++)
                    {
                        if(allDots[i,k]!= null)
                        {
                            allDots[i, k].GetComponent<Dot>().row = j;
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay *0.5f);
        StartCoroutine(FillBoardCo());
    }
    private IEnumerator DecreaseRowCo()
    {
        int nullCount=0;
        for(int i=0; i<width; i++)
        {
            for(int j=0; j<height; j++)
            {
                if(allDots[i,j] == null)
                {
                    nullCount++;
                }
                else if(nullCount> 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(refillDelay *0.5f);

        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i=0; i<width; i++)
        {
            for(int j=0; j<height; j++)
            {
                if(allDots[i,j] == null && !blankSpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j+ offset);
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations= 0;
                    while(MatchesAt(i,j, dots[dotToUse]) && maxIterations <100)
                    {
                        maxIterations++;
                        dotToUse= Random.Range(0, dots.Length);
                    }
                    maxIterations= 0;
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if(allDots[i,j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);

        while(MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
            yield return new WaitForSeconds(2*refillDelay);
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(refillDelay);
        if(IsDeadLocked())
        {
            ShuffleBoard();
            Debug.Log("Dead Locked");
        }
        currentState = GameState.move;
        streakValue= 1;
    }

    private void SwitchPieces(int column,int row,Vector2 direction)
    {
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        allDots[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for(int i=0; i<width; i++)
        {
            for(int j=0; j< height; j++)
            {
                if(i< width-2)
                {
                    if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                    {
                        if (allDots[i + 1, j].tag == allDots[i, j].tag
                            && allDots[i + 2, j].tag == allDots[i, j].tag)
                        {
                            return true;
                        }
                    }
                }
               
                if(j < height-2)
                {
                    if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                    {
                        if (allDots[i, j + 1].tag == allDots[i, j].tag
                            && allDots[i, j + 2].tag == allDots[i, j].tag)
                        {
                            return true;
                        }
                    }
                } 
            }
        }
        return false;
    }
    private bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if(CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }
    private bool IsDeadLocked()
    {
        for(int i=0; i< width; i++)
        {
            for(int j=0; j< height; j++)
            {
                if(allDots[i,j]!= null)
                {
                    if(i< width-1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if(j< height-1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIterations = 0;
                    piece.column = i;
                    piece.row = j;
                    allDots[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        if(IsDeadLocked())
        {
            ShuffleBoard();
        }
    }
}
