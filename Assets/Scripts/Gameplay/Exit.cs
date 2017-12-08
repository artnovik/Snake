using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public GameObject winText;

    void OnTriggerEnter(Collider other)
    {
        // .. We collider with the player (wall's collision layer only collides with the player's head)
        if (GameManager.Instance.gameState == GameState.Playing)
        {
            Time.timeScale = 0;
            winText.SetActive(true);
            StartCoroutine(Restart(2f));
        }
    }

    IEnumerator Restart(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameManager.Instance.GameCommencingEvent.Invoke();
    }
}
