using UnityEngine;
using Photon.Pun; // 1. Add Photon Namespace

public class FootstepsCreator : MonoBehaviourPun // 2. Change to MonoBehaviourPun
{
    public GameObject footstep;

    void Start()
    {
        footstep.SetActive(false);
    }

    void Update()
    {
        // 3. Gatekeeper: Only run Input checks for MY player
        if (!photonView.IsMine) return;

        // 4. Combine inputs into one check
        // "Is any movement key currently being held down?"
        bool isMoving = Input.GetKey(KeyCode.W) ||
                        Input.GetKey(KeyCode.S) ||
                        Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.D);

        if (isMoving)
        {
            // Only enable if it's not already on (optimization)
            if (!footstep.activeSelf)
                footstep.SetActive(true);
        }
        else
        {
            if (footstep.activeSelf)
                footstep.SetActive(false);
        }
    }
}