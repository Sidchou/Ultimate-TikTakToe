using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayableButton : MonoBehaviour
{
    [Header("button info")]
    public int boardID = -1;

    public int playableID = -1;

    public bool played = false;

    [Header("reference")]
    Animator animator;
    GameBoard board;
    public AudioClip circleClip;
    public AudioClip crossClip;
    public AudioSource audioSource;

    static public Action<PlayableButton> OnPlay;
    public Button button;

    private void OnEnable()
    {
        GameManager.OnResetBoard += ResetBoard;

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError(playableID + " animator is null!");
        }
        button = gameObject.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError(playableID + " button is null!");
        }
        playableID = gameObject.name[gameObject.name.LastIndexOf("(") + 1];
        playableID -= (int)'0';
        board = GetComponentInParent<GameBoard>();
        if (board == null)
        {
            Debug.LogError(playableID + " GameBoard is null!");
        }
        boardID = board.boardID;

    }
    private void Start()
    {
        //not sure why sometimes board id wont set on enable
        if (boardID == -1)
        {
            boardID = board.boardID;
        }
    }


    public void PlayButton()
    {
        if (!played && !GameManager.Instance.hasResult)
        {
            played = true;
            button.interactable = false;
            OnPlay?.Invoke(this);
            
        }
    }

    public void PlayCircle()
    {
        animator.ResetTrigger("Reset");
        animator.SetTrigger("Circle");
        audioSource.clip = circleClip;
        audioSource.Play();
    }
    public void PlayCross()
    {
        animator.ResetTrigger("Reset");
        animator.SetTrigger("Cross");
        audioSource.clip = crossClip;
        audioSource.Play();
    }
    public void ResetBoard()
    {
        animator.ResetTrigger("Circle");
        animator.ResetTrigger("Cross");
        animator.SetTrigger("Reset");
        played = false;

    }

}

