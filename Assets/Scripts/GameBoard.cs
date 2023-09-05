using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
 
public class GameBoard : MonoBehaviour
{
    [Header("Info")]
    public int boardID = -1;

    public bool scored = false;

    [Header("normal")]
    [SerializeField]
    public Image normalButtonImage;
    [SerializeField] 
    public Button normalButton;
    [SerializeField] 
    public AudioClip normalCircleClip;
    [SerializeField] 
    public AudioClip normalCrossClip;
    [SerializeField] 
    public AudioSource normalAudioSource;

    [Header("Cache")]
    private List<PlayableButton> buttons;
    [SerializeField]
    private List<int> circles;
    [SerializeField]
    private List<int> cross;
    [SerializeField]
    public Animator animator;

    private void Awake()
    {
        boardID = gameObject.name[gameObject.name.LastIndexOf("(") + 1];
        boardID -= (int)'0';
    }
    private void OnEnable()
    {
        GameManager.OnStartBoard += StartGame; 

        GameManager.OnResetBoard += ResetBoard;
        PlayableButton.OnPlay += PlayPiece;

        normalButtonImage = GetComponent<Image>();
        normalButton = GetComponent<Button>();

        buttons = GetComponentsInChildren<PlayableButton>().ToList();
    }
    private void OnDisable()
    {
        GameManager.OnResetBoard -= ResetBoard;
        PlayableButton.OnPlay -= PlayPiece;
     }
    #region Ultimate

    public void PlayPiece(PlayableButton button)
    { 
        if (button.boardID == boardID)
        {
            CheckBoard(button);
           // Debug.Log("board "+ boardID + " scored? " + scored);
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
    //Check Local Victory
    void CheckBoard(PlayableButton button)
    {
        if (GameManager.Instance.turns)
        {
            circles.Add(button.playableID);
            if (GameManager.Instance.CheckVictory(circles))
            {
                PlayPiece(true);
            }
        }
        else
        {
            cross.Add(button.playableID);
            if (GameManager.Instance.CheckVictory(cross))
            {
                PlayPiece(false);
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


    #endregion

    #region Normal

    public void PlayNormal()
    {
        if (!scored)
        {
            normalButtonImage.enabled = false;
            normalButton.enabled = false;
            PlayPiece(GameManager.Instance.turns);
            GameManager.Instance.CheckBigBoard();

            GameManager.Instance.DisplayTurn();
        }
    }
    public void PlayAiNormal()
    {
        if (!scored)
        {
            normalButton.onClick.Invoke();
        }
    }
    #endregion


    #region Manager

    private void PlayPiece(bool circle)
    {
        animator.ResetTrigger("Reset");

        scored = true;

        if (circle)
        {
            GameManager.Instance.circles.Add(boardID);
            PlayCircle();
            animator.SetTrigger("Circle");
        }
        else
        {
            GameManager.Instance.cross.Add(boardID);
            PlayCross();
            animator.SetTrigger("Cross");
        }
    }

    public void PlayCircle()
    {
        animator.ResetTrigger("Reset");
        animator.SetTrigger("Circle");
        normalAudioSource.clip = normalCircleClip;
        normalAudioSource.Play();
    }
    public void PlayCross()
    {
        animator.ResetTrigger("Reset");
        animator.SetTrigger("Cross");
        normalAudioSource.clip = normalCrossClip;
        normalAudioSource.Play();
    }

    public void ResetBoard()
    {
        animator.ResetTrigger("Circle");
        animator.ResetTrigger("Cross");
        scored = false;
        circles.Clear();
        cross.Clear();
        animator.SetTrigger("Reset");
        SetButtons(true);

        StartGame();
    }

    void StartGame()
    {
        switch (GameManager.Instance.currentMode)
        {   
            case GameManager.GameMode.Ultimate:
                StartMode(true);
                break;
            case GameManager.GameMode.Normal:
                StartMode(false);
                break;
            case GameManager.GameMode.Limit:
                StartMode(false);
                break;
            default:
                break;
        }
    }
    void StartMode(bool ultimate)
    {
        foreach (var b in buttons)
        {
            b.gameObject.SetActive(ultimate);
        }
        normalButtonImage.enabled = !ultimate;
        normalButton.enabled = !ultimate;
    }
    #endregion
}
