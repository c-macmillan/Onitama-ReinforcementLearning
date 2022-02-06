using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCard : MonoBehaviour
{
    public ScriptableObjectMoveCard moveCardData;
    public Player currentOwner;
    public SpriteRenderer spriteRenderer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = moveCardData.cardSprite;
    }

    public List<Vector2Int> GetMoves(){
        return moveCardData.availableMoves;
    }

    public void SwapCard(MoveCard otherCard){      
        ScriptableObjectMoveCard tempCardData = this.moveCardData;
        this.moveCardData = otherCard.moveCardData;
        otherCard.moveCardData = tempCardData;

        spriteRenderer.sprite = moveCardData.cardSprite;
        otherCard.spriteRenderer.sprite = otherCard.moveCardData.cardSprite;
    }
}

public enum PlayerColor {RED_PLAYER, BLUE_PLAYER, SIDELINE}
