using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks {

    void Start()
    {
        StartPhoton();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        StartPhoton();
    }

    void StartPhoton() {
        PhotonNetwork.AutomaticallySyncScene = true;
        if(!PhotonNetwork.InRoom)
            PhotonNetwork.ConnectUsingSettings();
    }
}
