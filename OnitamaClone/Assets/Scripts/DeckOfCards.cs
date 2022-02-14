using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckOfCards : MonoBehaviour
{
    [SerializeField] private ScriptableObjectMoveCard[] startingCards;
    private List<ScriptableObjectMoveCard> availableCards;

    private void Awake() {
        availableCards = startingCards.ToList();
    }
    public ScriptableObjectMoveCard DrawRandomCard(){
        int i = Random.Range(0, availableCards.Count);
        ScriptableObjectMoveCard card;
        card = availableCards[i];
        availableCards.RemoveAt(i);
        return card;
    }

    public void ResetDeck(){
        availableCards = startingCards.ToList();
    }
}
