using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public string cardName;
    public int value;
    public string suit;
    public bool isFrontCard;
    public int faceOrientation; 
    /*
     * 0 = face up
     * 1 = face down
     * 2 = no card
     */

    void Start() {
        SetupOnClick();
    }

    public void Init(string strCardName) {
        cardName = strCardName;
        value = Toolbox.GetCardValue(strCardName);
        if(strCardName != "")
            suit = strCardName.Substring(0, 1);
    }

    public void SetFaceUp(int orientation) {
        faceOrientation = orientation;
    }

    void SetupOnClick() {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick() {
        GetComponentInParent<UIHandler>().CardSelected(gameObject, isFrontCard);
    }
}
