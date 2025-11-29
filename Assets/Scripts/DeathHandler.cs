using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 1. Needed for Networking

// 2. Change MonoBehaviour to MonoBehaviourPun
public class DeathHandler : MonoBehaviourPun
{
    private Canvas gameOverCanvas;
    private Canvas youWonCanvas;

    private void Start()
    {
        // 3. Safety Check: If this script belongs to another player, turn it off.
        // We don't want to accidentally trigger Game Over if a friend dies.
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        // Make sure these TAGS exist in Unity!
        // It is safer to check for null to prevent crashes
        GameObject gameOverObj = GameObject.FindWithTag("GameOverUI");
        GameObject youWonObj = GameObject.FindWithTag("YouWonUI");

        if (gameOverObj != null)
        {
            gameOverCanvas = gameOverObj.GetComponent<Canvas>();
            gameOverCanvas.enabled = false;
        }

        if (youWonObj != null)
        {
            youWonCanvas = youWonObj.GetComponent<Canvas>();
            youWonCanvas.enabled = false;
        }
    }

    public void HandleDeath()
    {
        // Double check IsMine just in case
        if (!photonView.IsMine) return;

        gameOverCanvas.enabled = true;

        // 4. CRITICAL FIX: Do NOT use Time.timeScale = 0;

        // 5. Disable THIS player's ability to shoot
        // Use GetComponent() so we don't disable other players' weapons
        if (GetComponent<WeaponSwitcher>() != null)
            GetComponent<WeaponSwitcher>().enabled = false;

        // 6. Disable Movement (Assuming you have a movement script)
        // if(GetComponent<PlayerMovement>() != null)
        //    GetComponent<PlayerMovement>().enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EndGame()
    {
        if (!photonView.IsMine) return;

        youWonCanvas.enabled = true;

        // Same here: Disable controls, do not freeze time
        if (GetComponent<WeaponSwitcher>() != null)
            GetComponent<WeaponSwitcher>().enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}