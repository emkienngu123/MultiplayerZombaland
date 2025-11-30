using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 1. Add Photon Namespace

// 2. Change to MonoBehaviourPun to use RPCs
public class AmmoPickup : MonoBehaviourPun
{
    [SerializeField] int ammoAmount = 5;
    [SerializeField] AmmoType ammoType;
    [SerializeField] AudioClip pickupSound;

    // We don't use Start() for AudioSource anymore because 
    // the object gets destroyed instantly. We will use PlayClipAtPoint.

    private void OnTriggerEnter(Collider other)
    {
        // 3. Check if the thing colliding is a Player
        if (other.gameObject.CompareTag("Player"))
        {
            // 4. Identify WHICH player touched it
            PhotonView playerPV = other.GetComponent<PhotonView>();

            // 5. SAFETY CHECK: "IsMine"
            // Only run this logic if *I* am the player touching the box.
            // This prevents the enemy's computer from running this code for me.
            if (playerPV != null && playerPV.IsMine)
            {
                // 6. Give ammo to the SPECIFIC player that touched it
                Ammo playerAmmo = other.GetComponent<Ammo>();

                if (playerAmmo != null)
                {
                    playerAmmo.IncreaseCurrentAmmo(ammoType, ammoAmount);

                    // 7. Tell everyone to destroy this box
                    // "AllBuffered" means if someone joins late, the box is gone for them too.
                    photonView.RPC("RPC_DestroyPickup", RpcTarget.AllBuffered);
                }
            }
        }
    }

    [PunRPC]
    void RPC_DestroyPickup()
    {
        // Play sound at the location (Creating a temporary audio object)
        // This is better than AudioSource because the object is about to die.
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Destroy the object on everyone's screen
        Destroy(gameObject);
    }
}