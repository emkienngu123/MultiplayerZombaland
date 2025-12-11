using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using Photon.Pun;

public class GameChat : MonoBehaviour
{

    public TextMeshProUGUI chatText;
    public TMP_InputField chatInput;

    private bool isInputFieldToggled;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y) && !isInputFieldToggled)
        {
            isInputFieldToggled = true;
            chatInput.Select();
            chatInput.ActivateInputField();
            Debug.Log("Toggled on");
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isInputFieldToggled)
        {
            isInputFieldToggled = false;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("Toggled off");
        }

        //Sending a message
        if ((Input.GetKeyDown(KeyCode.Return) | Input.GetKeyDown(KeyCode.KeypadEnter)) && isInputFieldToggled && !chatInput.text.IsNullOrEmpty())
        {
            string messageToSend = $"{PhotonNetwork.LocalPlayer.NickName} : {chatInput.text}";
            GetComponent<PhotonView>().RPC("SendChatMessage", RpcTarget.AllBuffered, messageToSend);
            chatInput.text = "";
            isInputFieldToggled = false;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("Message Sent");
        }
    }

    [PunRPC]
    public void SendChatMessage(string _message)
    {
        chatText.text = chatText.text + "\n" + _message;
    }
}
