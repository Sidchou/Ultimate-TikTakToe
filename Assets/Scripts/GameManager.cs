using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    static private GameManager _instance;
    static public GameManager Instance
    { get { return _instance; } }

    public enum GameMode
    {
        Ultimate,
        Normal,
        Limit
    }
    [Tooltip("Ultimate, Normal, Limit")]
    public GameMode currentMode;
    [SerializeField]
    Image turnCircle;
    [SerializeField]
    Image turnCross;

    //start with cross
    public bool turns = false;
    [SerializeField]
    public List<int> circles;
    [SerializeField]
    public List<int> cross;
    [SerializeField]
    public List<int> tie;

    List<GameBoard> gameBoards;

    [SerializeField]
    public List<Animator> lines;
    [SerializeField]
    int lineID = -1;
    [SerializeField]
    GameObject resultContainer;
    [SerializeField]
    TMP_Text result;

    public UnityEvent OnStartGame;
    public static Action OnStartBoard;

    public static Action OnResetBoard;

    void Awake()
    {
        _instance = this;
    }
    void OnEnable()
    {
        PlayableButton.OnPlay += PlayPiece;
        gameBoards = FindObjectsByType<GameBoard>(FindObjectsSortMode.None).ToList();
        gameBoards =  gameBoards.OrderBy(e => e.name).ToList();
    }

    private void OnDisable()
    {
        PlayableButton.OnPlay -= PlayPiece;
    }

    //General
    //Change UI Display Turn
    public void DisplayTurn()
    {
        
        if (turns)
        {
            turnCircle.enabled = false;
            turnCross.enabled = true;
        }
        else
        {
            turnCircle.enabled = true;
            turnCross.enabled = false;
        }
        if (currentMode == GameMode.Limit)
        {
            Invoke("LimitedPlay", 0.1f);

            Invoke("ChangeTurns", 0.2f);
        }
        else
        {
            Invoke("ChangeTurns", 0.01f);
        }
    }
    //Change player
    public void ChangeTurns()
    {
        turns = !turns;
    }

    #region Ultimate
    //play piece (Ultimate)
    public void PlayPiece(PlayableButton button)
    {
        if (turns)
            button.PlayCircle();
        else
            button.PlayCross();

        DisplayTurn();
    }
     // check buttons on board
    [ContextMenu("setboard")]
    public void SetBoard(bool toSet)
    {
        foreach (var b in gameBoards)
        {
            if (GameManager.Instance.circles.Contains(b.boardID) || GameManager.Instance.cross.Contains(b.boardID) || GameManager.Instance.tie.Contains(b.boardID))
            {
                b.SetButtons(false);
            }
            else
            {
                b.SetButtons(toSet);
            }
        }
    }
    public void SetBoard(bool toSet, int id)
    {
        foreach (var b in gameBoards)
        {
            if (GameManager.Instance.circles.Contains(b.boardID) || GameManager.Instance.cross.Contains(b.boardID) || GameManager.Instance.tie.Contains(b.boardID))
            {
                b.SetButtons(false);
            }
            else
            {
                if (id == b.boardID)
                {
                    b.SetButtons(toSet);
                }
                else
                {
                    b.SetButtons(!toSet);

                }
            }
        }
    }
    #endregion

    #region Limit
    void LimitedPlay()
    {

        if (turns)
        {
            if (circles.Count > 3)
            {
                gameBoards[circles[0]].animator.ResetTrigger("Circle");
                LimitBoard(circles[0]);

                circles.RemoveAt(0);
            }
        }
        else
        {
            if (cross.Count > 3)
            {
                gameBoards[cross[0]].animator.ResetTrigger("Cross");
                LimitBoard(cross[0]);

                cross.RemoveAt(0);
            }
        }
    }
    void LimitBoard(int i)
    {
        gameBoards[i].scored = false;
        gameBoards[i].normalButtonImage.enabled = true;
        gameBoards[i].normalButton.enabled = true;

        gameBoards[i].animator.SetTrigger("Reset");

    }

    #endregion
    //General
    #region GameManaging
    public bool CheckVictory(List<int> checkList)
    {
        //rows
        for (int i = 0; i <= 6; i += 3)
        {
            if (checkList.Contains(i))
            {
                if (checkList.Contains(i + 1) && checkList.Contains(i + 2))
                {
                    if (i == 0)
                        lineID = 3;
                    else if (i == 3)
                        lineID = 4;
                    else if (i == 6)
                        lineID = 5;
                    return true;
                }
            }
        }
        //cols
        for (int i = 0; i <= 2; i += 1)
        {
            if (checkList.Contains(i))
            {
                if (checkList.Contains(i + 3) && checkList.Contains(i + 6))
                {
                    if (i == 0)
                        lineID = 0;
                    else if (i == 1)
                        lineID = 1;
                    else if (i == 2)
                        lineID = 2;
                    return true;
                }
            }

        }
        //cross
        if (checkList.Contains(4))
        {
            if ((checkList.Contains(0) && checkList.Contains(8)) || (checkList.Contains(2) && checkList.Contains(6)))
            {
                if (checkList.Contains(0))
                    lineID = 6;
                else if (checkList.Contains(2))
                    lineID = 7;
                return true;
            }
        }
        return false;
    }
    
    public void CheckBigBoard()
    {
        if (GameManager.Instance.CheckVictory(circles))
        {
            ShowResult("Circle Win");
        }
        if (GameManager.Instance.CheckVictory(cross))
        {
            ShowResult("Cross Win");
        }
        if (circles.Count + cross.Count + tie.Count >= 9)
        {
            ShowResult("Tie");
        }
    }

    public void ShowResult(string txt)
    {
        CancelInvoke("LimitedPlay");
        lines[lineID].SetTrigger("Draw");
        Invoke("ShowResult", 0.5f);
        result.text = txt;
    }
    public void ShowResult()
    {
        resultContainer.SetActive(true);
    }


    public void ResetBoard()
    {
        OnResetBoard?.Invoke();
        turns = false;
        resultContainer.SetActive(false);
        circles.Clear();
        cross.Clear();

        foreach (var l in lines)
        {
            l.SetTrigger("Reset");
        }
        lineID = -1;
    }
    public void StartGame(int mode)
    {
        currentMode = (GameMode) mode;
        OnStartGame?.Invoke();
        OnStartBoard?.Invoke();
    }
    #endregion
}
