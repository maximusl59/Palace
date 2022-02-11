using UnityEngine;
using Photon.Pun;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject content;
    public GameObject nameTextPrefab;
    public TMP_Text roomName;

    public override void OnEnable() {
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
    }

    void FixedUpdate() {
        UpdateNamesList();
    }

    private void UpdateNamesList() {

        foreach(Transform child in content.transform) {
            Destroy(child.gameObject);
        }

        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            GameObject clone = Instantiate(nameTextPrefab, content.transform);
            clone.GetComponent<TMP_Text>().text = player.NickName;
        }
    }
}
