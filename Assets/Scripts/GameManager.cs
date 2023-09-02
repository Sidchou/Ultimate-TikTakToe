using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    static private GameManager _instance;
    static public GameManager Instance
    { get { return _instance; } }

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

    public static Action OnResetBoard;
    void Awake()
    {
        _instance = this;
    }
    void OnEnable()
    {
        PlayableButton.OnPlay += PlayPiece;
        gameBoards = FindObjectsByType<GameBoard> (FindObjectsSortMode.None).ToList();

    }

    public void PlayPiece(PlayableButton button)
    {
        // Debug.Log(button.gameObject.name);

        if (turns)
        {
            button.PlayCircle();
            turnCircle.enabled = false;
            turnCross.enabled = true;
        }
        else
        {
            button.PlayCross();
            turnCircle.enabled = true;
            turnCross.enabled = false;
        }
        Invoke("ChangeTurns",0.01f);
    }
    public void ChangeTurns()
    {
        turns = !turns;
    }
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
        lines[lineID].SetTrigger("Reset");
        turns = false;
        resultContainer.SetActive(false);
        circles.Clear();
        cross.Clear();
        lines[lineID].ResetTrigger("Reset");

    }
}
