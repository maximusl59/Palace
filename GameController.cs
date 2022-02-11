using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private List<string> cardStr = new List<string> { "CA", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "C10" ,"CJ", "CQ", "CK",
                                 "DA", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D10" ,"DJ", "DQ", "DK",
                                 "SA", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S9", "S10" ,"SJ", "SQ", "SK",
                                 "HA", "H2", "H3", "H4", "H5", "H6", "H7", "H8", "H9", "H10" ,"HJ", "HQ", "HK"};
    /*
     * C = Clubs
     * D = Diamonds
     * S = Spades
     * H = Hearts
     */

    private List<Photon.Realtime.Player> players; 
    
    private Photon.Realtime.Player currentPlayer;

    private static int START_HAND = 9;

    private int readiedPlayers;

    public GameObject playerPrefab;

    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, transform.position, Quaternion.identity); 
    }

    private void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        SeatsAssignedEvent.OnSeatsAssigned += OnSeatsAssigned;
    }

    private void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        SeatsAssignedEvent.OnSeatsAssigned -= OnSeatsAssigned;
    }

    private void OnSeatsAssigned() {
        if (PhotonNetwork.IsMasterClient)
            StartGame();
    }

    private void OnEvent(EventData photonEvent) {
        if (!PhotonNetwork.IsMasterClient)
            return;

        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.PlayCardEventCode) {
            object[] data = (object[])photonEvent.CustomData;
            string[] cardsPlayed = (string[])data[0];
            int cardsInHand = (int)data[1];
            if (cardStr.Count > 0 && cardsInHand < 3)
                SendCards(currentPlayer.ActorNumber, DrawCards(3 - cardsInHand));
            if (Toolbox.GetCardValue(cardsPlayed[0]) == 8)
                Skip(cardsPlayed.Length);
            GetNextPlayer();
            YourTurn(currentPlayer.ActorNumber);
        } else if (eventCode == EventCodes.ReadyEvent) {
            readiedPlayers++;
            if (readiedPlayers >= PhotonNetwork.PlayerList.Length)
                YourTurn(currentPlayer.ActorNumber);
        } else if (eventCode == EventCodes.PickUpEvent) {
            GetNextPlayer();
            YourTurn(currentPlayer.ActorNumber);
        } else if (eventCode == EventCodes.BurnEvent) {
            object[] data = (object[])photonEvent.CustomData;
            Photon.Realtime.Player player = (Photon.Realtime.Player)data[0];
            currentPlayer = player;
            YourTurn(currentPlayer.ActorNumber);
        } else if (eventCode == EventCodes.FinishedPlayingEvent) {
            GetNextPlayer();
            YourTurn(currentPlayer.ActorNumber);
        } else if (eventCode == EventCodes.RemovePlayerEvent) {
            object[] data = (object[])photonEvent.CustomData;
            Photon.Realtime.Player player = (Photon.Realtime.Player)data[0];
            GetNextPlayer();
            RemovePlayer(player);
            if (players.Count == 1)
                EndGame();
            YourTurn(currentPlayer.ActorNumber);
        }
    }

    private void StartGame() {
        Photon.Realtime.Player[] playerList = PhotonNetwork.PlayerList;
        for (int i = 0; i < playerList.Length; i++) {
            InitCards(playerList[i].ActorNumber, DrawCards(START_HAND)); 
        }

        players = new List<Photon.Realtime.Player>(PhotonNetwork.PlayerList.Length);
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            players.Add(player);

        int index = Random.Range(0, players.Count);
        currentPlayer = players[index];
    }

    private string DrawCard() {
        if (cardStr.Count == 0)
            return "";

        int cardIndex = Random.Range(0, cardStr.Count);
        string result = cardStr[cardIndex];
        cardStr.RemoveAt(cardIndex);
        return result;
    }

    private string[] DrawCards(int noOfCards) {
        string[] result = new string[noOfCards];
        for(int i = 0; i < noOfCards; i++) {
            string card = DrawCard();
            result[i] = card;
        }

        UpdateCardsLeft(cardStr.Count);
        return result;
    }

    private void GetNextPlayer() {
        int index = FindIndex(players, currentPlayer);
        index++;
        if (index >= players.Count)
            index = 0;
        currentPlayer = players[index];
    }

    private void Skip(int howManyTimes) {
        for (int i = 0; i < howManyTimes; i++)
            GetNextPlayer();
    }

    private int FindIndex(List<Photon.Realtime.Player> playerList, Photon.Realtime.Player player) {
        for (int i = 0; i < playerList.Count; i++)
            if (player == playerList[i])
                return i;

        return -1;
    }

    private void RemovePlayer(Photon.Realtime.Player player) {
        players.Remove(player);
    }

    private void EndGame() {
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.LoadLevel(0);
    }

    private void InitCards(int actorNo, string[] cards) {
        object[] content = new object[] { actorNo, cards };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(EventCodes.InitCardsEvent, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void SendCards(int actorNo, string[] cards) {
        object[] content = new object[] { actorNo, cards }; 
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(EventCodes.DrawCardEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    
    private void YourTurn(int actorNo) {
        object[] content = new object[] { actorNo }; 
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(EventCodes.YourTurnEvent, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void UpdateCardsLeft(int cardsLeft) {
        object[] content = new object[] { cardsLeft };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(EventCodes.UpdateDrawCardsLeftEvent, content, raiseEventOptions, SendOptions.SendReliable);
    }

    /*
     * Initialization:
     *      -Deal 3 face down cards, then 3 face up cards
     *      -Deal 3 cards to each player
     *      -Allow swapping of cards 
     *      
     * Turn:
     *      -Check which cards are playable
     *      -Play card(s)
     *      -Deal cards til 3 cards min are in hand
     *      -Signal next player's turn
     * 
     */
}
