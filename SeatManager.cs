using Photon.Pun;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    public GameObject[] seats;
    public PlayArea playArea;

    private bool assignedSeats;

    void Start() {
        assignedSeats = false;
    }

    void Update()
    {
        CheckAssignSeats();
    }

    private void CheckAssignSeats() {
        if (GameObject.FindGameObjectsWithTag("Player").Length != PhotonNetwork.PlayerList.Length || assignedSeats)
            return;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            AssignSeatsTwoPlayer();
        else
            AssignSeats();

        SeatsAssignedEvent.OnOnSeatsAssigned();
        assignedSeats = true;
    }

    private void AssignSeats() {
        PhotonView[] players = FindObjectsOfType<PhotonView>();
        Photon.Realtime.Player player = PhotonNetwork.LocalPlayer;

        for(int i = 0; i < players.Length; i++) {
            AssignSeatsHelper(players, i, player);
            player = player.GetNext();
        }
    }

    private void AssignSeatsTwoPlayer() {
        PhotonView[] players = FindObjectsOfType<PhotonView>();
        Photon.Realtime.Player player = PhotonNetwork.LocalPlayer;

        AssignSeatsHelper(players, 0, player);

        player = player.GetNext();

        AssignSeatsHelper(players, 2, player);
    }

    private void AssignSeatsHelper(PhotonView[] players, int seatNo, Photon.Realtime.Player player) {
        seats[seatNo].SetActive(true);
        foreach (PhotonView p in players) {
            if (p.ControllerActorNr == player.ActorNumber) {
                Player playerScript = p.gameObject.GetComponent<Player>();
                p.gameObject.transform.parent = seats[seatNo].transform;
                playerScript.cardLayoutManager = seats[seatNo].GetComponentInChildren<CardLayoutManager>();
                playerScript.playArea = playArea;
                playerScript.cardLayoutManager.nameText.text = player.NickName;
                if(seatNo == 0) {
                    seats[seatNo].GetComponentInChildren<UIHandler>().player = playerScript;
                    playerScript.uiHandler = seats[seatNo].GetComponentInChildren<UIHandler>();
                }
                break;
            }
        }
    }
}
