using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public Player player;
    public CardLayoutManager cardLayoutManager;
    public GameObject checkmarkPrefab;
    public GameObject deselectButton;
    public GameObject interactButton;

    public bool swapPhase;

    public List<GameObject> selectedCards;

    void Start() {
        selectedCards = new List<GameObject>();
    }

    public void CardSelected(GameObject go, bool isFrontCard) {
        if (swapPhase && SwapCheck(go, isFrontCard)) {
            DeselectCards();
            return;
        }

        if(CardSelectionChecks(go))
            return;

        GameObject clone = Instantiate(checkmarkPrefab, go.transform);
        clone.name = "Check";
        selectedCards.Add(go);
        deselectButton.SetActive(true);
        if(!swapPhase)
            EnableConfirmButton();
    }

    public void DeselectCards() {
        foreach(GameObject go in selectedCards) {
            Destroy(go.transform.Find("Check").gameObject);
        }
        selectedCards.Clear();
        deselectButton.SetActive(false);
        if(!swapPhase)
            interactButton.SetActive(false);
    }

    public void OnInteractButton() {
        if(swapPhase) {
            swapPhase = false;
            interactButton.SetActive(false); //TODO get rid of this and in deselect cards
            player.SendReadyEvent();
            DeselectCards();
            cardLayoutManager.NotYourTurn();
        } else if (selectedCards.Count == 1 && selectedCards[0].GetComponent<Card>().faceOrientation == 1 && !IsBurnable()) { // blind flip
            player.BlindFlip(selectedCards[0].GetComponent<Card>().cardName);
            DeselectCards();
            cardLayoutManager.NotYourTurn();
        } else if (selectedCards.Count > 0 && !IsBurnable()) { //Confirm phase
            string[] playedCards = new string[selectedCards.Count];
            for(int i = 0; i < selectedCards.Count; i++) {
                playedCards[i] = selectedCards[i].GetComponent<Card>().cardName;
            }
            player.PlayCards(playedCards);
            DeselectCards();
            cardLayoutManager.NotYourTurn();
        } else { //BURN phase
            if(selectedCards.Count > 0) {
                string[] playedCards = new string[selectedCards.Count];
                for (int i = 0; i < selectedCards.Count; i++) {
                    playedCards[i] = selectedCards[i].GetComponent<Card>().cardName;
                }
                player.BurnCards(playedCards);
                DeselectCards();
            } else {
                int multiple = 0;
                string[] playedCards = new string[4-player.playArea.topCardMultipleNo];
                if (player._cardsInHand.Count == 0) {
                    foreach (string str in player._faceUpCards)
                        if (Toolbox.GetCardValue(str) == Toolbox.GetCardValue(player.playArea.topCard)) {
                            playedCards[multiple] = str;
                            multiple++;
                        }
                            

                } else {
                    foreach (string str in player._cardsInHand)
                        if (Toolbox.GetCardValue(str) == Toolbox.GetCardValue(player.playArea.topCard)) {
                            playedCards[multiple] = str;
                            multiple++;
                        }
                }
                player.BurnCards(playedCards);
                DeselectCards();
            }
        }
    }

    public void PickUp() {
        player.PickUp();
        cardLayoutManager.NotYourTurn();
    }

    private void EnableConfirmButton() {
        interactButton.GetComponentInChildren<TMP_Text>().text = "Confirm?";
        interactButton.SetActive(true);
    }

    private bool CardSelectionChecks(GameObject go) {
        if (selectedCards.Contains(go))
            return true;
        else if (selectedCards.Count > 0 && go.GetComponent<Card>().value != selectedCards[0].GetComponent<Card>().value)
            DeselectCards();
        else if (go.GetComponent<Card>().faceOrientation == 1)
            DeselectCards();
        return false;
    }

    private bool SwapCheck(GameObject go, bool isFrontCard) {
        if(selectedCards.Count > 0 && selectedCards[0].GetComponent<Card>().isFrontCard != isFrontCard) {
            if (isFrontCard)
                player.SwapCards(go.GetComponent<Card>().cardName, selectedCards[0].GetComponent<Card>().cardName);
            else
                player.SwapCards(selectedCards[0].GetComponent<Card>().cardName, go.GetComponent<Card>().cardName);

            return true;
        }
        return false;
    }

    private bool IsBurnable() {
        int value = Toolbox.GetCardValue(selectedCards[0].GetComponent<Card>().cardName);

        if (value == 10)
            return true;

        int multiple = selectedCards.Count;
        int topCardValue = Toolbox.GetCardValue(player.playArea.topCard);
        int topCardMultiple = player.playArea.topCardMultipleNo;

        if (multiple == 4)
            return true;

        if (value == topCardValue && multiple + topCardMultiple == 4)
            return true;
        return false;
    }
}
