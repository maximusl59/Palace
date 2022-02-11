using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class Matchmaking : MonoBehaviourPunCallbacks {

    [SerializeField]
    private byte maxPlayers = 4;

    void Start() {
        if(PhotonNetwork.LocalPlayer.NickName == "")
            UpdateUsername("");
    }

    public void UpdateUsername(string name) {
        if (name.IsNullOrEmpty())
            PhotonNetwork.LocalPlayer.NickName = "Player " + Random.Range(1000, 10000);
        else
            PhotonNetwork.LocalPlayer.NickName = name;
    }

    public void OnCreatePressed() {
        TMP_InputField inputField = GameObject.Find("RoomNameIF").GetComponent<TMP_InputField>();
        string roomName = inputField.text;
        CreateRoom(roomName);
    }

    private void CreateRoom(string roomName) {
        if (roomName.IsNullOrEmpty())
            roomName = Random.Range(10000, 100000).ToString();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.PublishUserId = true;
        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }

    public void OnJoinPressed() {
        TMP_InputField inputField = GameObject.Find("RoomNameIF").GetComponent<TMP_InputField>();
        string roomName = inputField.text;
        JoinRoom(roomName);
    }

    private void JoinRoom(string roomName) {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnBackPressed() {
        PhotonNetwork.Disconnect();
    }

    public void OnStartPressed() {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(1);
    }
}
