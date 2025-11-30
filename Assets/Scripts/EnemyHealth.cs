using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 1. Needed for Networking

// 2. Change to MonoBehaviourPun
public class EnemyHealth : MonoBehaviourPun
{
    [SerializeField] float hitPoints = 100f;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip hitSound;

    AudioSource audioSource;
    bool isDead = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool IsDead()
    {
        return isDead;
    }

    // 3. THE FIX: Add [PunRPC] so the Weapon script can call this
    [PunRPC]
    public void TakeDamage(float damage)
    {
        // This alerts the EnemyAI script (on the same object) to become provoked
        BroadcastMessage("OnDamageTaken");

        hitPoints -= damage;

        // Play sound on everyone's computer
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) { return; }

        isDead = true;

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        GetComponent<Animator>().SetTrigger("Die");

        // Optional: If you want to remove the body after 5 seconds
        // Destroy(gameObject, 5f); 
        // Note: For multiplayer, use a Coroutine + IsMine check to destroy network objects safely.
    }
}