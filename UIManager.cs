using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviourPunCallbacks {

    public Button createButton;
    public Button joinButton;
    public TMP_InputField nameIF;

    public GameObject mainMenu;
    public GameObject createMenu;
    public GameObject joinMenu;
    public GameObject lobbyMenu;

    public GameObject startButton;

    void Start() {
        ToggleRooms(PhotonNetwork.InRoom);
    }

    void Update()
    {
        EnableStartButton();
        ToggleButtons();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        // show error message
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        // show error message
    }

    public override void OnJoinedRoom() {
        // joined a room successfully
        ToggleRooms(true);
    }

    public override void OnDisconnected(DisconnectCause cause) {
        ToggleRooms(false);
    }

    private void ToggleRooms(bool isConnected) {
        lobbyMenu.SetActive(isConnected);
        mainMenu.SetActive(!isConnected);
        createMenu.SetActive(false);
        joinMenu.SetActive(false);
    }

    private void ToggleButtons() {
        if (PhotonNetwork.IsConnected) {
            createButton.interactable = true;
            joinButton.interactable = true;
            nameIF.interactable = true;
        }
        else {
            createButton.interactable = false;
            joinButton.interactable = false;
            nameIF.interactable = false;
        }
    }

    private void EnableStartButton() {
        if(!startButton.activeSelf && PhotonNetwork.IsMasterClient)
            startButton.SetActive(true);
        if(!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount < 2) 
            startButton.SetActive(false);
    }
}
