using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPun
{
    [SerializeField] float hitPoints = 400f;
    private TextMeshProUGUI healthBar;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip hitSound;
    

    AudioSource audioSource;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        GameObject textObj = GameObject.FindWithTag("HealthUI");
        if (textObj != null)
        {
            healthBar = textObj.GetComponentInChildren<TextMeshProUGUI>();
        } else
        {
            Debug.LogError("HealthText object not found in the scene.");
        }
            audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        DisplayHealth();
    }

    public void DisplayHealth()
    {
        float currentHealth = hitPoints / 4;
        healthBar.text = currentHealth.ToString();
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        
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
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        GetComponent<DeathHandler>().HandleDeath();
    }


    public void GiveHealth()
    {
        hitPoints = 400f;
    }

}
