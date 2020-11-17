using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameState
{
    wait,
    move
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
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offset;
    public GameObject tilePrefab;
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    public GameObject[] dots;
    public GameObject destroyEffect;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;
    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        Setup();
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

    private void Setup()
    {
        GenerateBlankSpaces();
        for(int i=0; i< width; i++)
        {
            for (int j = 0; j < height; j++) 
            {
                if(!blankSpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + i + "," + j + ")";
                    int dotsToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotsToUse]) && maxIterations < 100)
                    {
                        dotsToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }
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
            CheckToMakeBombs();
            GameObject particle= Instantiate(destroyEffect, allDots[column,row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            Destroy(allDots[column, row]);
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
        yield return new WaitForSeconds(.4f);
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
        yield return new WaitForSeconds(.4f);

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
        yield return new WaitForSeconds(0.5f);

        while(MatchesOnBoard())
        {
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentDot = null;

        yield return new WaitForSeconds(.4f);
        currentState = GameState.move;
    }
}
