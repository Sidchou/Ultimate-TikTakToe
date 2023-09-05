using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine; 

public class AI : MonoBehaviour
{
    public bool aiEnabled = false;
    public bool aiTurn = true;
    public GameObject aiMask;
    
    float bestScore;
    int bestMove;

    List<int> _playable;
    List<int> _circles;
    List<int> _cross;

    string st;
    string dp = "";
    public int GetBestScore()
    {
        
        bestScore = -Mathf.Infinity;
        
        _circles = new List<int>(GameManager.Instance.circles);
        _cross = new List<int>(GameManager.Instance.cross);
        _playable = Enumerable.Range(0, 9).Except(_circles).Except(_cross).ToList();

        if (_playable.Count == 0)
        {
            GameManager.Instance.CheckBigBoard();
        } 
        for (int i = 0; i < _playable.Count; i++)
        {
            int playing = _playable[i];
            float cacheScore = Minimax(_playable, playing, 0, true);
 
            if (cacheScore > bestScore)
            {
                bestMove = _playable[i];
                bestScore = cacheScore;
            }
            st += " " + _playable[i] + " " + cacheScore + " ";

        }
        st += "bm " + bestMove;
        Debug.Log(st);
        st = " ";
       // Debug.Log(dp);

        // Minimax(-1, -1, true);
        return bestMove;
    }
 
    float Minimax(List<int> playable, int board, int depth, bool isMaxing)
    {
        //limit depth or tie
        //if (depth > 3 )
        //{
        //   // dp += board + " " + depth + " | ";
        //    return 0;
        //}

        //ai m  ai cur change
        // T T   O O +
        // T F   O X -
        // F F   X X +
        // F T   X O -

        List<int> cashedList = isMaxing ? _circles : _cross;
        if (depth == 0)
            cashedList.Add(board);
        if (GameManager.Instance.CheckVictory(cashedList))
        { 
            if (isMaxing == aiTurn)
                return 1-depth*0.1f;
            else
                return -(1- depth * 0.1f);
        }
        else if (_circles.Count + _cross.Count == 9)
        {
            //dp += board + " " + depth + ": ___ | ";
            return 0;
        }
         

        List<int> cashedPlayable = new List<int>(playable);
        cashedPlayable.Remove(board); 

        float localMinmax = isMaxing ? -Mathf.Infinity : Mathf.Infinity;

        for (int i = 0; i < cashedPlayable.Count; i++)
        {
            int playing = cashedPlayable[i];
            cashedList.Add(playing); 
            float score = Minimax(cashedPlayable, playing, depth + 1, !isMaxing);
            cashedList.Remove(playing); 

            if (isMaxing)
                localMinmax = Mathf.Max(localMinmax, score);
            else
                localMinmax = Mathf.Min(localMinmax, score); 
        }
        if (depth == 0)
            cashedList.Remove(board);

        return localMinmax;
    }
}


