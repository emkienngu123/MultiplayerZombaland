using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; // 1. Add Photon Namespace

// 2. Change MonoBehaviour to MonoBehaviourPun
public class MissionWaypoint : MonoBehaviourPun
{
    private Image img;
    private Text meter;
    private Transform target;
    public Vector3 offset;

    private void Start()
    {
        // 3. THE FIX: If this script is on a Remote Player (not me), turn it off.
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        // 4. Find the UI Container automatically
        GameObject wayPointObj = GameObject.FindWithTag("Waypoint");

        if (wayPointObj != null)
        {
            img = wayPointObj.GetComponentInChildren<Image>();
            meter = wayPointObj.GetComponentInChildren<Text>();
        }
        else
        {
            Debug.LogError("Could not find object with tag 'WayPoint'. Make sure your UI Panel has this tag!");
        }

        // 5. Find the Target
        GameObject targetObj = GameObject.FindWithTag("WayPointTarget");
        if (targetObj != null)
        {
            target = targetObj.transform;
        }
        else
        {
            Debug.LogWarning("No target found with tag 'WayPointTarget'.");
        }
    }

    private void Update()
    {
        // Safety Check: If we haven't found the UI or Target yet, don't run math
        if (img == null || target == null || meter == null) return;

        // --- Your Original Math (Preserved) ---

        float minX = img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(target.position + offset);

        // Check if target is behind the player
        if (Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        img.transform.position = pos;

        // Update text
        meter.text = ((int)Vector3.Distance(target.position, transform.position)).ToString() + "m";
    }
}