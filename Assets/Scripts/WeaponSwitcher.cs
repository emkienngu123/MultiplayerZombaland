using UnityEngine;
using Photon.Pun;

public class WeaponSwitcher : MonoBehaviourPun
{
    [Header("References")]
    // DRAG the "Weapons" GameObject here in the Inspector!
    public Transform weaponContainer;

    [SerializeField] int currentWeapon = 0;
    [SerializeField] AudioClip weaponSwitchSound;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Safety Check
        if (weaponContainer == null)
        {
            Debug.LogError("You forgot to assign the 'Weapons' object to the WeaponSwitcher!");
            return;
        }

        SetWeaponActive(currentWeapon);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        int previousWeapon = currentWeapon;
        ProcessKeyInput();
        ProcessScrollWheel();

        if (previousWeapon != currentWeapon)
        {
            SetWeaponActive(currentWeapon);
            // Sync weapon switch
            photonView.RPC("RPC_SetWeapon", RpcTarget.AllBuffered, currentWeapon);
        }
    }

    // ... (Keep ProcessScrollWheel and ProcessKeyInput logic same as before) ...
    void ProcessScrollWheel()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (currentWeapon >= weaponContainer.childCount - 1) currentWeapon = 0;
            else currentWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (currentWeapon <= 0) currentWeapon = weaponContainer.childCount - 1;
            else currentWeapon--;
        }
    }

    void ProcessKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentWeapon = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentWeapon = 2;
    }

    [PunRPC]
    void RPC_SetWeapon(int _weaponIndex)
    {
        currentWeapon = _weaponIndex;
        SetWeaponActive(_weaponIndex);
    }

    void SetWeaponActive(int _index)
    {
        int weaponIndex = 0;

        // CHANGED: We loop through 'weaponContainer', not 'transform'
        foreach (Transform weapon in weaponContainer)
        {
            if (weaponIndex == _index) weapon.gameObject.SetActive(true);
            else weapon.gameObject.SetActive(false);
            weaponIndex++;
        }
    }

    // --- THE FIX FOR YOUR WEAPON FX ---

    [PunRPC]
    public void RPC_FireWeaponEffect(int weaponIndex)
    {
        // 1. Look inside the WEAPON CONTAINER to find the correct gun
        if (weaponContainer != null && weaponIndex < weaponContainer.childCount)
        {
            Transform gunTransform = weaponContainer.GetChild(weaponIndex);

            // 2. Found the gun! Now tell it to flash.
            Weapon gunScript = gunTransform.GetComponent<Weapon>();
            if (gunScript != null)
            {
                gunScript.PlayEffectsOnly();
            }
        }
    }
}