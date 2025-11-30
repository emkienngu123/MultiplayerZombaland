using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun; // 1. Add Photon Namespace

public class Notification : MonoBehaviour
{
    // We remove [SerializeField] because we find these automatically to prevent crashes
    private TextMeshProUGUI notificationText;
    private Animator notificationAnim;

    [Header("Data")]
    [SerializeField] NotifcationScriptable notification;

    private void Start()
    {
        // 2. AUTO-FIND UI
        // You tagged the "Notification Canvas" as "NotificationUI"
        GameObject uiRoot = GameObject.FindWithTag("NotificationUI");

        if (uiRoot != null)
        {
            // --- STRATEGY: SEARCH INSIDE THE TAGGED OBJECT ---

            // 1. Try to find the "Main Panel" inside the Canvas
            Transform mainPanel = uiRoot.transform.Find("Main Panel");

            // If we found "Main Panel", look inside it. 
            // If not, maybe the user tagged the Main Panel directly? So we use uiRoot.
            Transform targetPanel = (mainPanel != null) ? mainPanel : uiRoot.transform;

            // 2. Get Animator (It should be on the Main Panel)
            notificationAnim = targetPanel.GetComponent<Animator>();
            if (notificationAnim == null)
            {
                // Fallback: Just look for ANY animator inside the tagged object
                notificationAnim = uiRoot.GetComponentInChildren<Animator>();
            }

            // 3. Get Text (It should be on "Objective" child of Main Panel)
            Transform objectiveNode = targetPanel.Find("Objective");
            if (objectiveNode != null)
            {
                notificationText = objectiveNode.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                // Fallback: Just look for ANY text inside the tagged object
                notificationText = uiRoot.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
        else
        {
            Debug.LogError("Notification UI not found! Please tag your Canvas as 'NotificationUI'.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 3. IS MINE CHECK
            // Only show message if *I* am the one walking into the zone
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                StartCoroutine(EnableNotification());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && notification.removeAfterExit)
        {
            // Same check for exit
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                RemoveNotification();
            }
        }
    }

    IEnumerator EnableNotification()
    {
        // Safety check
        if (notificationAnim != null && notificationText != null)
        {
            notificationText.text = notification.notificationMessage;
            notificationAnim.Play("Fade In");

            if (notification.disableAfterTimer)
            {
                yield return new WaitForSeconds(notification.disableTimer);
                RemoveNotification();
            }
        }
    }

    private void RemoveNotification()
    {
        if (notificationAnim != null)
        {
            notificationAnim.Play("Fade Out");
        }
    }
}