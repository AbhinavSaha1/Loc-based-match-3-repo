using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameStartCo());
    }

    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1f);
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
    }
}
