using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
 
public class GameBoard : MonoBehaviour
{
    public int boardID = -1;

    public bool scored = false; 

    private List<PlayableButton> buttons;
    [SerializeField]
    private List<int> circles;
    [SerializeField]
    private List<int> cross;
    [SerializeField]
    Animator animator;

    private void Awake()
    {
        boardID = gameObject.name[gameObject.name.LastIndexOf("(") + 1];
        boardID -= (int)'0';
    }
    private void OnEnable()
    {
        GameManager.OnResetBoard += ResetBoard;
        PlayableButton.OnPlay += PlayPiece;
        buttons = GetComponentsInChildren<PlayableButton>().ToList();
    }
    private void OnDisable()
    {
        GameManager.OnResetBoard -= ResetBoard;
        PlayableButton.OnPlay -= PlayPiece;
     }


    public void PlayPiece(PlayableButton button)
    {

        if (button.boardID == boardID)
        {
            CheckBoard(button);
            Debug.Log("board "+ boardID + " scored? " + scored);


        }
        if (button.playableID == boardID)
        {
            if (!scored)
            {
                GameManager.Instance.SetBoard(true, button.playableID);
            }
            else
            {
                GameManager.Instance.SetBoard(true);

            }
        }
    }

    void CheckBoard(PlayableButton button)
    {
        if (GameManager.Instance.turns)
        {
            circles.Add(button.playableID);
            if (GameManager.Instance.CheckVictory(circles))
            {
                scored = true;
                 GameManager.Instance.circles.Add(boardID);
                animator.SetTrigger("Circle");
            }
        }
        else
        {
            cross.Add(button.playableID);
            if (GameManager.Instance.CheckVictory(cross))
            {
                scored = true;
                 GameManager.Instance.cross.Add(boardID);
                animator.SetTrigger("Cross");
            }
        }
        if (circles.Count + cross.Count == 9)
        {
            scored = true;
            GameManager.Instance.tie.Add(boardID);
        }
        GameManager.Instance.CheckBigBoard();
    }
    public void SetButtons(bool set)
    {
        foreach (var b in buttons)
        {
            if (b.played)
            {
                b.button.interactable = false;
            }
            b.button.interactable = set;
        }
    }
    public void ResetBoard()
    {
        scored = false;
        circles.Clear();
        cross.Clear();
        animator.SetTrigger("Reset");
        SetButtons(true);
        //animator.ResetTrigger("Reset");
    }

}
