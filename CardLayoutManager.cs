using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardLayoutManager : MonoBehaviour
{
    public GameObject[] frontCards;
    public GameObject cardPrefab;
    public Transform handParent;
    public TMP_Text numText;
    public TMP_Text nameText;

    public GameObject interactButton;

    private List<GameObject> handGOs;

    void Start() {
        handGOs = new List<GameObject>();
    }

    public void ShowUI(string[] faceUpCards, List<string> cardsInHand, bool isMine, Player player) {
        for(int i = 0; i < frontCards.Length; i++) {
            Card card = frontCards[i].GetComponent<Card>();
            frontCards[i].GetComponent<Card>().Init(faceUpCards[i]);

            if (faceUpCards[i] == "") //card is null 
                card.SetFaceUp(2);
            else if (player._faceUpCards.Contains(faceUpCards[i]))
                card.SetFaceUp(0);
            else if (player._faceDownCards.Contains(faceUpCards[i]))
                card.SetFaceUp(1);
        }

        ShowFrontCards();

        if (isMine) {
            ClearHand();
            for (int i = 0; i < cardsInHand.Count; i++) {
                GameObject clone = Instantiate(cardPrefab, handParent);
                clone.GetComponent<Image>().sprite = Resources.Load<Sprite>(Toolbox.GetSpritePath(cardsInHand[i]));
                clone.GetComponent<Card>().Init(cardsInHand[i]);
                handGOs.Add(clone);
            }
        } else
            numText.text = cardsInHand.Count.ToString();
    }

    public void EnableBurn() {
        interactButton.GetComponentInChildren<TMP_Text>().text = "BURN";
        interactButton.SetActive(true);
    }

    public void DisableBurn() {
        interactButton.SetActive(false);
    }

    public void NotYourTurn() {
        foreach(GameObject go in frontCards) {
            go.GetComponent<Button>().interactable = false;
        }

        foreach (GameObject go in handGOs) {
            go.GetComponent<Button>().interactable = false;
        }
    }

    public void YourTurn(Player player) { //TODO : if remaining cards are the same && cardsLeft == 0 you can play a palace card of the same value
        int cardsCanPlay = 0;

        if(handGOs.Count == 0) 
            foreach (GameObject go in frontCards) {
                if (go.GetComponent<Card>().faceOrientation == 1 && player._faceUpCards.Length == 0) {
                    go.GetComponent<Button>().interactable = true;
                    cardsCanPlay++;
                } else if (go.GetComponent<Card>().faceOrientation == 0 && 
                    Toolbox.CanPlay(player.playArea.realCard, go.GetComponent<Card>().cardName)) {

                    go.GetComponent<Button>().interactable = true;
                    cardsCanPlay++;
                } 
            }

        foreach (GameObject go in handGOs) {
            if(Toolbox.CanPlay(player.playArea.realCard, go.GetComponent<Card>().cardName)) {
                go.GetComponent<Button>().interactable = true;
                cardsCanPlay++;
            }
        }

        if (cardsCanPlay == 0)
            player.playArea.EnableMidButton();
    }

    private void ShowFrontCards() {
        for(int i = 0; i < frontCards.Length; i++) {
            Card card = frontCards[i].GetComponent<Card>();
            if (card.faceOrientation == 0)
                frontCards[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(Toolbox.GetSpritePath(card.cardName));
            else if (card.faceOrientation == 1)
                frontCards[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(Toolbox.GetCardBackPath());
            else
                frontCards[i].SetActive(false);
        }
    }

    private void ClearHand() {
        for(int i = handGOs.Count - 1; i > -1; i--) {
            GameObject remains = handGOs[i];
            handGOs.RemoveAt(i);
            Destroy(remains);
        }
    }
}
