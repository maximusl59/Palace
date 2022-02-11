using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayArea : MonoBehaviour
{
    public TMP_Text drawCardsLeft;
    public List<string> cardsInPlay;

    public GameObject topCardGO;
    public string topCard;
    public int topCardMultipleNo;

    public TMP_Text realValue; // for clone card
    public string realCard; //for clone card

    private void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent) {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.PlayCardEventCode) {
            object[] data = (object[])photonEvent.CustomData;
            HandleCardPlayed((string[])data[0]);
        }
        else if (eventCode == EventCodes.UpdateDrawCardsLeftEvent) {
            object[] data = (object[])photonEvent.CustomData;
            drawCardsLeft.text = ((int)data[0]).ToString();
        }
        else if (eventCode == EventCodes.BurnEvent) 
            Burn();
        else if (eventCode == EventCodes.PickUpEvent)
            Burn();
    }

    private void HandleCardPlayed(string[] cardsPlayed) {
        foreach (string str in cardsPlayed) {
            cardsInPlay.Add(str);
        }
        topCard = cardsPlayed[cardsPlayed.Length - 1];
        topCardGO.GetComponent<Image>().sprite = Resources.Load<Sprite>(Toolbox.GetSpritePath(topCard));
        topCardGO.SetActive(true);
        topCardMultipleNo = CalculateMultiple();

        if(topCardMultipleNo > 1) 
            topCardGO.GetComponentInChildren<TMP_Text>().text = topCardMultipleNo.ToString();
        else 
            topCardGO.GetComponentInChildren<TMP_Text>().text = "";

        CheckBurnableEvent.OnOnCheckBurnable(); //send burn check event

        if (Toolbox.GetCardValue(topCard) == 3) { //if clone card
            realCard = GetLastValidCard();
            realValue.text = "Real Value: " + Toolbox.GetCardValueString(realCard);
        } else {
            realCard = topCard;
            realValue.text = "";
        }
    }

    private int CalculateMultiple() {
        int result = 0;
        int topVal = Toolbox.GetCardValue(topCard);

        for(int i = cardsInPlay.Count - 1; i > -1; i--) {
            if (Toolbox.GetCardValue(cardsInPlay[i]) != topVal)
                break;
            result++;
        }

        return result;
    }

    private string GetLastValidCard() {
        string result = "";
        int topVal = Toolbox.GetCardValue(topCard);

        for (int i = cardsInPlay.Count - 1; i > -1; i--) {
            if (Toolbox.GetCardValue(cardsInPlay[i]) != topVal) {
                result = cardsInPlay[i];
                break;
            }
        }

        return result;
    }

    public void Burn() {
        cardsInPlay.Clear();
        topCardGO.SetActive(false);
        topCardGO.GetComponent<Button>().interactable = false;
        topCardMultipleNo = 0;
        realCard = "";
        realValue.text = "";
    }

    public void EnableMidButton() {
        topCardGO.GetComponent<Button>().interactable = true;
    }
}
