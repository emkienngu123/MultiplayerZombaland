using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Weapon : MonoBehaviourPun
{
    [Header("Camera")]
    [SerializeField] Camera FPCamera;

    [Header("Stats")]
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;
    [SerializeField] float timeBetweenShots = 0.5f;

    [Header("Visuals")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;

    [Header("Ammo System")]
    [SerializeField] Ammo ammoSlot;
    [SerializeField] AmmoType ammoType;
    [SerializeField] TextMeshProUGUI ammoText;

    [Header("Audio")]
    [SerializeField] AudioClip gunSound;
    [SerializeField] AudioClip emptyGunSound;

    AudioSource audioSource;
    bool canShoot = true;

    private void OnEnable()
    {
        canShoot = true;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 3. UI CONNECT: Only find UI if this is MY gun
        if (photonView.IsMine)
        {
            GameObject textObj = GameObject.FindWithTag("AmmoDisplayUI");
            if (textObj != null)
            {
                ammoText = textObj.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }

    void Update()
    {
        // 4. INPUT BLOCK: Only I can shoot my gun
        if (!photonView.IsMine) return;

        DisplayAmmo();

        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            StartCoroutine(Shoot());
        }
    }

    void DisplayAmmo()
    {
        if (ammoText != null)
        {
            int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);
            ammoText.text = currentAmmo.ToString();
        }
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        if (ammoSlot.GetCurrentAmmo(ammoType) > 0)
        {
            // A. LOCAL VISUALS (Instant feedback for you)
            PlayEffectsOnly();

            // B. NETWORK VISUALS (Tell Parent to relay the message)
            // We call "RPC_FireWeaponEffect" which MUST exist on your WeaponSwitcher (Parent)
            // We pass 'transform.GetSiblingIndex()' so it knows if this is Gun 0, 1, or 2
            photonView.RPC("RPC_FireWeaponEffect", RpcTarget.Others, transform.GetSiblingIndex());

            // C. LOGIC
            ProcessRaycast();
            ammoSlot.ReduceCurrentAmmo(ammoType);
        }
        else
        {
            PlayEmptyGunSound();
        }

        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    // --- RENAMED & PUBLIC ---
    // This is NOT an RPC anymore. The Parent (WeaponSwitcher) calls this manually.
    public void PlayEffectsOnly()
    {
        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource && gunSound) audioSource.PlayOneShot(gunSound);
    }

    void PlayEmptyGunSound()
    {
        if (audioSource && emptyGunSound) audioSource.PlayOneShot(emptyGunSound);
    }

    void ProcessRaycast()
    {
        RaycastHit hit;

        Vector3 origin = (FPCamera != null) ? FPCamera.transform.position : transform.position;
        Vector3 direction = (FPCamera != null) ? FPCamera.transform.forward : transform.forward;

        if (Physics.Raycast(origin, direction, out hit, range))
        {
            CreateHitImpact(hit);

            // 5. DAMAGE LOGIC (Multiplayer)
            PhotonView targetPV = hit.transform.GetComponent<PhotonView>();

            if (targetPV != null)
            {
                // Call TakeDamage on the HIT object's computer
                targetPV.RPC("TakeDamage", RpcTarget.All, damage);
            }
            else
            {
                // Fallback for single player AI
                EnemyHealth simpleTarget = hit.transform.GetComponent<EnemyHealth>();
                if (simpleTarget != null)
                {
                    simpleTarget.TakeDamage(damage);
                }
            }
        }
    }

    private void CreateHitImpact(RaycastHit hit)
    {
        if (hitEffect != null)
        {
            GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 0.1f);
        }
    }
}