using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 1. Add Photon Namespace

public class DamageDisplayer : MonoBehaviourPun // 2. Inherit from MonoBehaviourPun
{
    // We remove [SerializeField] because we must find it with code
    private Canvas impactCanvas;
    [SerializeField] float impactTime = 0.3f;

    void Start()
    {
        // 3. IsMine Check:
        // If this script belongs to another player (a "Ghost"), 
        // turn it off so it can't flash red on OUR screen.
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        // 4. Find the Canvas in the scene automatically
        // Make sure you created a Tag called "ImpactUI" in Unity and assigned it to your Canvas!
        GameObject canvasObj = GameObject.FindWithTag("ImpactUI");

        if (canvasObj != null)
        {
            impactCanvas = canvasObj.GetComponent<Canvas>();
            impactCanvas.enabled = false;
        }
        else
        {
            Debug.LogError("Impact Canvas Not Found! Did you tag it 'ImpactUI'?");
        }
    }

    public void ShowDamageImpact()
    {
        // Double safety check
        if (impactCanvas != null && photonView.IsMine)
        {
            StartCoroutine(ShowSplatter());
        }
    }

    IEnumerator ShowSplatter()
    {
        impactCanvas.enabled = true;
        yield return new WaitForSeconds(impactTime);
        impactCanvas.enabled = false;
    }
}