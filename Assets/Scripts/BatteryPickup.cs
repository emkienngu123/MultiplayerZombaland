using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 1. Add Photon Namespace

public class BatteryPickup : MonoBehaviourPun // 2. Inherit from MonoBehaviourPun
{
    [Header("Settings")]
    [SerializeField] float restoreAngle = 90f;
    [SerializeField] float addIntensity = 1f;
    [SerializeField] AudioClip batteryPickupSound;

    // We remove the cached variables (player, audioSource) because 
    // we need to find them dynamically when the collision happens.

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 3. Identify WHICH player touched it
            PhotonView playerPV = other.GetComponent<PhotonView>();

            // 4. SAFETY CHECK: Only run logic if *I* am the one touching it
            if (playerPV != null && playerPV.IsMine)
            {
                // 5. Apply effects to the specific player that touched us

                // Fix Flashlight
                Flashlight flashlight = other.GetComponentInChildren<Flashlight>();
                if (flashlight != null)
                {
                    flashlight.RestoreLightAngle(restoreAngle);
                    flashlight.AddLightIntensity(addIntensity);
                }

                // Fix Health (Replaced FindObjectOfType with GetComponent)
                PlayerHealth pHealth = other.GetComponent<PlayerHealth>();
                // Debug.Log("Player pHealth ", pHealth);

                if (pHealth != null)
                {
                    pHealth.GiveHealth();
                }

                // 6. Destroy the object for everyone
                photonView.RPC("RPC_DestroyBattery", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void RPC_DestroyBattery()
    {
        Debug.Log("Battery Picked Up");

        // Play sound at location (since object is about to vanish)
        if (batteryPickupSound != null)
        {
            AudioSource.PlayClipAtPoint(batteryPickupSound, transform.position);
        }

        // Destroy locally
        Destroy(gameObject);
    }
}