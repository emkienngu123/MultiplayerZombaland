using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MissionWaypoint : MonoBehaviourPun
{
    private Image img;
    private Text meter;
    private Transform target;

    // Cached camera
    private Camera myOwnCamera;

    public Vector3 offset;

    private void Awake()
    {
        // 1. CRITICAL: Identify if this Camera belongs to the Local Player.
        if (!photonView.IsMine)
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        // 2. GET THE CAMERA
        if (myOwnCamera == null)
        {
            myOwnCamera = GetComponent<Camera>();
        }

        if (myOwnCamera == null)
        {
            Debug.LogError("MissionWaypoint is attached to an object without a Camera component!");
        }

        // 3. Find UI based on Hierarchy: Waypoint -> Img -> Text
        GameObject wayPointRoot = GameObject.FindWithTag("Waypoint");

        if (wayPointRoot != null)
        {
            // A. Find the Image (Child of Waypoint)
            img = wayPointRoot.GetComponentInChildren<Image>();

            if (img != null)
            {
                // B. Find the Text (Child of Image)
                meter = img.GetComponentInChildren<Text>();

                // Force visible
                img.enabled = true;
                if (meter != null) meter.enabled = true;
            }
            else
            {
                Debug.LogError("Found 'Waypoint' tag, but it has no Image component in children!");
            }
        }
        else
        {
            Debug.LogError("Could not find object with tag 'Waypoint'.");
        }

        // 4. Find Target
        GameObject targetObj = GameObject.FindWithTag("WayPointTarget");
        if (targetObj != null)
        {
            target = targetObj.transform;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        // Safety Check
        if (img == null || target == null || meter == null || myOwnCamera == null) return;

        // --- CALCULATION ---

        float minX = img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        // 5. USE THE SPECIFIC CAMERA
        Vector2 pos = myOwnCamera.WorldToScreenPoint(target.position + offset);

        // Check if target is behind the player
        if (Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
                pos.x = maxX;
            else
                pos.x = minX;
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        // THE FIX: Move the Image (The Middle Child). 
        // Because "Text" is a child of "Img", moving "Img" will automatically move the Text with it.
        img.transform.position = pos;

        // Update text
        float dist = Vector3.Distance(target.position, transform.position);
        meter.text = ((int)dist).ToString() + "m";
    }
}