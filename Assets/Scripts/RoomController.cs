using UnityEngine;
using UnityEngine.UI;
using TMPro; // Needed for Text Mesh Pro
using Photon.Pun;
using Photon.Realtime;

public class RoomController : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    public GameObject menuPanel; // Your Main Menu (the one with Host/Join buttons)
    public GameObject roomPanel; // The new panel you just created

    [Header("Room UI Elements")]
    public TMP_Text roomNameText;
    public TMP_Text playerListText;
    public GameObject startGameButton;

    // 1. This runs automatically when you create OR join a room
    public override void OnJoinedRoom()
    {
        // Switch UI
        menuPanel.SetActive(false);
        roomPanel.SetActive(true);

        // Update Room Name
        roomNameText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        PhotonNetwork.AutomaticallySyncScene = true;

        // Update the list of players
        UpdatePlayerList();

        // Only the Host (Master Client) should see the Start Game button
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // 2. This runs when ANOTHER player joins your room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    // 3. This runs when a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    // 4. Helper function to update the text list of players
    void UpdatePlayerList()
    {
        string playerList = "Players:\n";

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            playerList += p.NickName + "\n";
        }

        playerListText.text = playerList;

        // Ensure only host sees start button (in case host left and you became host)
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // 5. Linked to the "Start Game" Button
    public void StartGame()
    {
        // Only the host can start the game
        
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameOnline"); // Change "GameScene" to your actual scene name
            Debug.Log("Game Started by Host");
        }
    }

    // 6. Linked to the "Leave" Button
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // Go back to Main Menu
        roomPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}