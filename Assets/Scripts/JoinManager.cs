using UnityEngine;
using TMPro; // Needed for the Input Field
using Photon.Pun;
using Photon.Realtime;

public class JoinManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public TMP_InputField joinInput; // Drag your new Input Field here


    [Header("UI Panels")]
    public GameObject joinRoomPanel; // Your Main Menu (the one with Host/Join buttons)
    public GameObject roomPanel; // The new panel you just created

    // Link this to the Join Button
    public void JoinSpecificRoom()
    {
        string roomName = joinInput.text;

        // check if they actually typed something
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Room Name is empty!");
            return;
        }

        Debug.Log("Attempting to join room: " + roomName);
        PhotonNetwork.NickName = "Player_" + Random.Range(100, 999);
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room Successfully!");

        // OPEN YOUR ROOM UI HERE
        // Example:
        joinRoomPanel.SetActive(false);
        roomPanel.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join room: " + message);
        // This usually happens if the room doesn't exist or is full
    }
}