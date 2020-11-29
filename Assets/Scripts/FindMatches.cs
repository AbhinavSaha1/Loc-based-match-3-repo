﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.row));
        }
        if (dot2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
        }
        if (dot3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();

        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.column));
        }
        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
        }
        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
        }
        return currentDots;
    }

    private void AddMatchesTOList(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddMatchesTOList(dot1);
        AddMatchesTOList(dot2);
        AddMatchesTOList(dot3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        //yield return new WaitForSeconds(0.2f);
        yield return null;

        for(int i=0 ; i< board.width ; i++)
        {
            for(int j=0 ; j< board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                
                if(currentDot!= null)
                {
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    if (i > 0 && i < board.width -1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if(leftDot!= null && rightDot!= null)
                        {
                            Dot rightDotDot = rightDot.GetComponent<Dot>();
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            if (leftDot != null && rightDot != null)
                            {
                                if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                                {
                                    currentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot));
                                    currentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot));
                                    GetNearbyPieces(leftDot, currentDot, rightDot);
                                }
                            }
                        }   
                    }
                    if (j > 0 && j  < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j+1];
                        GameObject downtDot = board.allDots[i, j-1];

                        if(upDot!= null && downtDot!= null)
                        {
                            Dot downDotDot = downtDot.GetComponent<Dot>();
                            Dot upDotDot = upDot.GetComponent<Dot>();

                            if (upDot != null && downtDot != null)
                            {
                                if (upDot.tag == currentDot.tag && downtDot.tag == currentDot.tag)
                                {
                                    currentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot));
                                    currentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot));
                                    GetNearbyPieces(upDot, currentDot, downtDot);
                                }
                            }
                        }
                        
                    }

                   /* if((i > 0 && i < board.width - 1) && (j > 0 && j < board.height - 1))
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject upRightDot= board.allDots[i+1, j + 1];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if(upDot!= null && rightDot!= null && upRightDot!=null)
                        {
                            if(upDot.tag== currentDot.tag && rightDot.tag== currentDot.tag && upRightDot.tag== currentDot.tag)
                            {
                                upDot.GetComponent<Dot>().isMatched = true;
                                upRightDot.GetComponent<Dot>().isMatched = true;
                                rightDot.GetComponent<Dot>().isMatched = true;
                                currentDot.GetComponent<Dot>().isMatched = true;

                            }
                        }
                    }*/
                }
            }
        }
    }

    public void MatchPiecesOfColor(string color)
    {
        for(int i= 0; i< board.width; i++)
        {
            for(int j=0; j<board.height; j++)
            {
                if(board.allDots[i,j] != null)
                {
                    if(board.allDots[i,j].tag== color)
                    {
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i=0;i<board.height; i++ )
        {
            if(board.allDots[column, i]!= null)
            {
                Dot dot= board.allDots[column,i].GetComponent<Dot>();
                if(dot.isRowBomb)
                {
                    dots.Union(GetRowPieces(i).ToList());
                }
                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
            }
        }

        return dots;
    }

    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dot dot= board.allDots[i,row].GetComponent<Dot>();
                if(dot.isColumnBomb)
                {
                    dots.Union(GetColumnPieces(i).ToList());
                }
                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        }

        return dots;
    }

    public void CheckBombs(MatchType matchType)
    {
        if(board.currentDot!= null)
        {
            if(board.currentDot.isMatched && board.currentDot.tag== matchType.color)
            {
                board.currentDot.isMatched = false;
                /*int typeOfBomb = Random.Range(0, 100);
                if(typeOfBomb < 50)
                {
                    board.currentDot.MakeRowBomb();
                }
                else if(typeOfBomb >= 50)
                {
                    board.currentDot.MakeColumnBomb();
                }*/
                if((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                    ||(board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        board.currentDot.MakeRowBomb();
                    }
                else
                {
                    board.currentDot.MakeColumnBomb();
                }
            }
            else if(board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if(otherDot.isMatched && otherDot.tag == matchType.color)
                {
                    otherDot.isMatched = false;
                    /*int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50)
                    {
                        otherDot.MakeRowBomb();
                    }
                    else if (typeOfBomb >= 50)
                    {
                        otherDot.MakeColumnBomb();
                    }*/
                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                   || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }

        }
    }
}
