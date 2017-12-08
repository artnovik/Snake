using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlePanel : MonoBehaviour
{
    private Animator anim;       // cach animator component

    private int slideOutHash;    // cash animator parameters
    private int slideInHash;     // cash animator parameters

    [SerializeField]
    private HUD hudScript;       // reference to the HUD script to slide the head in

    [SerializeField]
    private GameObject player;   // activate the player when play button is pressed

    [SerializeField]
    private GameObject startPoint;

    void Awake()
    {
        anim = GetComponent<Animator>();

        slideOutHash = Animator.StringToHash("SlideOut");
        slideInHash = Animator.StringToHash("SlideIn");
    }

    void Start()
    {
        GameManager.Instance.GameOverEvent.AddListener(OnGameOver);
    }

    /// <summary>
    /// when snake image is clicked, slide out and start game
    /// </summary>
    public void OnPlay()
    {
        // .. Check if we are already playing to prevent spamming the button
        if (GameManager.Instance.gameState == GameState.Playing)
            return;

        // .. Change game state to playing
        GameManager.Instance.gameState = GameState.Playing;

        // .. Closing gates
        if (GameManager.Instance.gateState == GateState.Opened || GameManager.Instance.gateState == GateState.ExitOpened)
        {
            GameManager.Instance.gateState = GateState.Closed;
            ScoreManager.Instance.gateOpened.SetActive(false);
            ScoreManager.Instance.gateClosed.SetActive(true);
            GameManager.Instance.gateState = GateState.ExitClosed;
            ScoreManager.Instance.exit.SetActive(false);
            ScoreManager.Instance.exitWall.SetActive(true);
        }
        
        // .. Play menu music (it's the same menu and ingame music)
        SoundManager.Instance.PlayMenuMusic();

        // .. Slide out
        anim.SetTrigger(slideOutHash);

        // .. Slide in the HUD
        StartCoroutine(SlideInHUD(.5f));

        // .. Activate player
        
        StartCoroutine(ActivatePlayer(.85f));
    }

    IEnumerator SlideInHUD(float delay, bool slideIn = true)
    {
        yield return new WaitForSeconds(delay);

        if (slideIn)
            hudScript.SlideIn();
        else
            hudScript.SlideOut();
    }

    IEnumerator ActivatePlayer(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameManager.Instance.GameCommencingEvent.Invoke();   // Game is commencing here 
        
        player.SetActive(true);
        player.transform.position = new Vector3(player.transform.position.x - 2f, startPoint.transform.position.y, startPoint.transform.position.z);
    }

    /// <summary>
    /// Reset game when player loses
    /// </summary>
    private void OnGameOver()
    {
        StartCoroutine(ResetGameAfter(1f));
    }

    IEnumerator ResetGameAfter(float dur)
    {
        yield return new WaitForSeconds(dur);

        // .. Invoke the reset event after dur
        GameManager.Instance.ResetEvent.Invoke();

        OnReturn();
    }

    /// <summary>
    /// Slide In the main menu animation and hide the HUD
    /// </summary>
    private void OnReturn()
    {
        // .. Slide In main menu UI
        anim.SetTrigger(slideInHash);

        // .. Slide out the HUD
        StartCoroutine(SlideInHUD(0f, false));

        // .. Deactivate the player
        player.SetActive(false);
    }
}
