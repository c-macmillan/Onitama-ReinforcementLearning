using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        return moveCardData.availableMoves.ToList();
    }

    public List<float> GetMovesAsFloat(){
        List<float> _availableMovesAsFloat = new List<float>();
        foreach(Vector2Int _moveVector2Int in moveCardData.availableMoves){
            _availableMovesAsFloat.Add(_moveVector2Int.x);
            _availableMovesAsFloat.Add(_moveVector2Int.y);
        }
        while(_availableMovesAsFloat.Count < 8){
            _availableMovesAsFloat.Add(float.MinValue);
        }

        return _availableMovesAsFloat;
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
