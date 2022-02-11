using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
    public int noCardsInHand;
    public PhotonView photonView;
    public CardLayoutManager cardLayoutManager;
    public UIHandler uiHandler;
    public PlayArea playArea;

    private int actorNum;
    public string[] _faceDownCards, _faceUpCards, _frontCards;
    public List<string> _cardsInHand;

    void Start() {
        InitLists();
        actorNum = photonView.ControllerActorNr;
    }

    private void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        if (photonView.IsMine)
            CheckBurnableEvent.OnCheckBurnable += OnCheckBurnable;
    }

    private void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        if (photonView.IsMine)
            CheckBurnableEvent.OnCheckBurnable -= OnCheckBurnable;
    }

    private void OnCheckBurnable() {
        int multiple = 0;
        if (_cardsInHand.Count == 0) {
            foreach (string str in _faceUpCards)
                if (Toolbox.GetCardValue(str) == Toolbox.GetCardValue(playArea.topCard))
                    multiple++;
            

        } else {
            foreach (string str in _cardsInHand)
                if (Toolbox.GetCardValue(str) == Toolbox.GetCardValue(playArea.topCard))
                    multiple++;
        }

        if (multiple != 0 && playArea.topCardMultipleNo != 0 && multiple + playArea.topCardMultipleNo == 4)
            cardLayoutManager.EnableBurn();
        else
            cardLayoutManager.DisableBurn();
    }

    private void OnEvent(EventData photonEvent) {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.YourTurnEvent) {
            object[] data = (object[])photonEvent.CustomData;
            if ((int)data[0] == actorNum)
                cardLayoutManager.nameText.color = Color.yellow;
            else
                cardLayoutManager.nameText.color = Color.white;
        }

        if (!photonView.IsMine)
            return;

        if (eventCode == EventCodes.InitCardsEvent) {
            object[] data = (object[])photonEvent.CustomData;
            if ((int)data[0] != actorNum)
                return;

            photonView.RPC("RPCParseCardInit", RpcTarget.All, (string[])data[1]);
        } else if (eventCode == EventCodes.YourTurnEvent) {
            object[] data = (object[])photonEvent.CustomData;
            if ((int)data[0] != actorNum) {
                cardLayoutManager.NotYourTurn();
                uiHandler.DeselectCards();
                OnCheckBurnable();
                return;
            }

            if (_cardsInHand.Count == 0 && _faceDownCards.Length == 0 && _faceUpCards.Length == 0) {
                SendRemovePlayerEvent(PhotonNetwork.LocalPlayer);
                return;
            }

            cardLayoutManager.YourTurn(this);
        } else if (eventCode == EventCodes.DrawCardEventCode) {
            object[] data = (object[])photonEvent.CustomData;
            if ((int)data[0] != actorNum)
                return;

            photonView.RPC("RPCDrawCards", RpcTarget.All, (object)data[1]);
        }
    }

    private void InitLists() {
        _faceDownCards = new string[3];
        _faceUpCards = new string[3];
        _frontCards = new string[3];
        _cardsInHand = new List<string>();
    }

    private void OnSeatsAssigned() {
        cardLayoutManager.ShowUI(_frontCards, _cardsInHand, photonView.IsMine, this);
        SeatsAssignedEvent.OnSeatsAssigned -= OnSeatsAssigned;
    }

    private int FindIndex(string[] strList, string str) {
        for (int i = 0; i < strList.Length; i++)
            if (str == strList[i])
                return i;

        return -1;
    }

    private void BubbleSort(List<string> arr) {
        int n = arr.Count;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
                if (Toolbox.GetCardValue(arr[j]) > Toolbox.GetCardValue(arr[j + 1])) {
                    // swap temp and arr[i]
                    string temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                }
    }

    private void CheckEmpty() {
        if (_faceUpCards.Length > 0 && _faceUpCards[0] == "" && _faceUpCards[1] == "" && _faceUpCards[2] == "")
            _faceUpCards = new string[0];
        if (_faceDownCards.Length > 0 && _faceDownCards[0] == "" && _faceDownCards[1] == "" && _faceDownCards[2] == "")
            _faceDownCards = new string[0];
    }

    public void BlindFlip(string cardPlayed) {
        if(Toolbox.CanPlay(playArea.realCard, cardPlayed)) {
            string[] cardsPlayed = new string[1];
            cardsPlayed[0] = cardPlayed;
            photonView.RPC("RPCPlayCards", RpcTarget.All, (object)cardsPlayed);
            SendPlayCardEvent(cardsPlayed, _cardsInHand.Count);
        } else {
            photonView.RPC("RPCBlindFlip", RpcTarget.All, cardPlayed);
            SendFinishedPlayingEvent();
        }
    }

    public void BurnCards(string[] cardsPlayed) {
        photonView.RPC("RPCBurnCards", RpcTarget.All, (object)cardsPlayed);
        SendBurnEvent(PhotonNetwork.LocalPlayer); //TODO
    }

    public void PlayCards(string[] cardsPlayed) {
        photonView.RPC("RPCPlayCards", RpcTarget.All, (object)cardsPlayed);
        SendPlayCardEvent(cardsPlayed, _cardsInHand.Count);
    }

    public void SwapCards(string frontCard, string handCard) {
        photonView.RPC("RPCSwapCards", RpcTarget.All, frontCard, handCard);
    }

    public void PickUp() {
        photonView.RPC("RPCPickUp", RpcTarget.All);
        SendPickUpEvent();
    }

    public void SendPlayCardEvent(string[] cardsPlayed, int cardsInHand) {
        object[] content = new object[] { cardsPlayed, cardsInHand };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(EventCodes.PlayCardEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendReadyEvent() {
        object[] content = new object[] {};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(EventCodes.ReadyEvent, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendPickUpEvent() {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(EventCodes.PickUpEvent, content, raiseEventOptions, SendOptions.SendReliable);
    }
    
    public void SendFinishedPlayingEvent() {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(EventCodes.FinishedPlayingEvent, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendBurnEvent(Photon.Realtime.Player player) {
        object[] content = new object[] { player };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(EventCodes.BurnEvent, content, raiseEventOptions, SendOptions.SendReliable);
    }
    
    public void SendRemovePlayerEvent(Photon.Realtime.Player player) {
        object[] content = new object[] { player };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(EventCodes.RemovePlayerEvent, content, raiseEventOptions, SendOptions.SendReliable);
    }

    [PunRPC]
    private void RPCParseCardInit(string[] cards) {
        for (int i = 0; i < cards.Length; i++) {
            int temp = i;
            if (i < 3)
                _faceDownCards[i] = cards[i];
            else if (i < 6)
                _faceUpCards[temp - 3] = cards[i];
            else
                _cardsInHand.Add(cards[i]);
        }

        _faceUpCards.CopyTo(_frontCards, 0);

        BubbleSort(_cardsInHand);

        if (cardLayoutManager != null)
            cardLayoutManager.ShowUI(_frontCards, _cardsInHand, photonView.IsMine, this);
        else
            SeatsAssignedEvent.OnSeatsAssigned += OnSeatsAssigned;
    }

    [PunRPC]
    private void RPCSwapCards(string frontCard, string handCard) {
        int faceUpIndex = FindIndex(_faceUpCards, frontCard);
        int handIndex = _cardsInHand.IndexOf(handCard);

        _faceUpCards[faceUpIndex] = handCard;
        _frontCards[faceUpIndex] = handCard;
        _cardsInHand[handIndex] = frontCard;

        cardLayoutManager.ShowUI(_frontCards, _cardsInHand, photonView.IsMine, this);
    }

    [PunRPC]
    private void RPCPlayCards(string[] playedCards) {
        foreach(string str in playedCards) {
            if(_cardsInHand.Contains(str)) 
                _cardsInHand.Remove(str);
            else if (_faceUpCards.Contains(str)) {
                int i = FindIndex(_faceUpCards, str);
                _faceUpCards[i] = "";
                _frontCards[i] = _faceDownCards[i];
            } else {
                int i = FindIndex(_faceDownCards, str);
                _faceDownCards[i] = "";
                _frontCards[i] = "";
            }
        }

        CheckEmpty();

        cardLayoutManager.ShowUI(_frontCards, _cardsInHand, photonView.IsMine, this);
    }

    [PunRPC]
    private void RPCDrawCards(string[] drawnCards) {
        for(int i = 0; i < drawnCards.Length; i++)
            _cardsInHand.Add(drawnCards[i]);
        BubbleSort(_cardsInHand);
        cardLayoutManager.ShowUI(_frontCards, _cardsInHand, photonView.IsMine, this);
        cardLayoutManager.NotYourTurn();
    }

    [PunRPC]
    private void RPCPickUp() {
        foreach (string str in playArea.cardsInPlay)
            _cardsInHand.Add(str);
        playArea.Burn();
        BubbleSort(_cardsInHand);
        cardLayoutManager.ShowUI(_frontCards, _cardsInHand, photonView.IsMine, this);
    }
    
    [PunRPC]
    private void RPCBurnCards(string[] playedCards) {
        foreach (string str in playedCards) {
            if (_cardsInHand.Contains(str))
                _cardsInHand.Remove(str);
            else if (_faceUpCards.Contains(str)) {
                int i = FindIndex(_faceUpCards, str);
                _faceUpCards[i] = "";
                _frontCards[i] = _faceDownCards[i];
            }
            else {
                int i = FindIndex(_faceDownCards, str);
                _faceDownCards[i] = "";
                _frontCards[i] = "";
            }
        }
        CheckEmpty();

        playArea.Burn();
        BubbleSort(_cardsInHand);
        cardLayoutManager.ShowUI(_frontCards, _cardsInHand, photonView.IsMine, this);
    }

    [PunRPC]
    private void RPCBlindFlip(string cardPlayed) {
        foreach (string str in playArea.cardsInPlay)
            _cardsInHand.Add(str);

        _cardsInHand.Add(cardPlayed);

        int i = FindIndex(_faceDownCards, cardPlayed);
        _faceDownCards[i] = "";
        _frontCards[i] = "";

        CheckEmpty();
        playArea.Burn();
        BubbleSort(_cardsInHand);
        cardLayoutManager.ShowUI(_frontCards, _cardsInHand, photonView.IsMine, this);
    }
}
