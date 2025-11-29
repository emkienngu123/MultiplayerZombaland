using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 1. Add Photon Namespace

public class Ammo : MonoBehaviourPun // 2. Change to MonoBehaviourPun
{
    [SerializeField] AmmoSlot[] ammoSlots;

    [System.Serializable]
    private class AmmoSlot
    {
        public AmmoType ammoType;
        public int ammoAmount;
    }

    public int GetCurrentAmmo(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);

        // Safety Check: If we ask for an ammo type we don't have, return 0
        if (slot != null)
        {
            return slot.ammoAmount;
        }
        return 0;
    }

    public void ReduceCurrentAmmo(AmmoType ammoType)
    {
        // 3. Optimization: Only the local player cares about reducing ammo
        if (!photonView.IsMine) return;

        AmmoSlot slot = GetAmmoSlot(ammoType);

        // Safety Check: Prevent "NullReferenceException" if slot is missing
        if (slot != null)
        {
            slot.ammoAmount--;
        }
    }

    public void IncreaseCurrentAmmo(AmmoType ammoType, int ammoAmount)
    {
        // Only the local player picks up ammo
        if (!photonView.IsMine) return;

        AmmoSlot slot = GetAmmoSlot(ammoType);

        if (slot != null)
        {
            slot.ammoAmount += ammoAmount;
        }
    }

    private AmmoSlot GetAmmoSlot(AmmoType ammoType)
    {
        foreach (AmmoSlot slot in ammoSlots)
        {
            if (slot.ammoType == ammoType)
            {
                return slot;
            }
        }
        // If not found, return null (Handled in the functions above now)
        return null;
    }
}