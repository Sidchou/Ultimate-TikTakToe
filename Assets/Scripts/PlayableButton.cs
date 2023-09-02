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
        boardID = board.boardID;

    }


    public void PlayButton()
    {
        if (!played)
        {
            OnPlay?.Invoke(this);
            played = true;
            
        }
    }

    public void PlayCircle()
    {
        animator.SetTrigger("Circle");
        audioSource.clip = circleClip;
        audioSource.Play();
    }
    public void PlayCross()
    {
        animator.SetTrigger("Cross");
        audioSource.clip = crossClip;
        audioSource.Play();
    }
    public void ResetBoard()
    {
        animator.SetTrigger("Reset");
        played = false;

    }
}

