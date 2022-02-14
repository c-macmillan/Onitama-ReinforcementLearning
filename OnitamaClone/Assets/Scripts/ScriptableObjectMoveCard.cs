using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveCard", menuName = "OnitamaClone/MoveCard", order = 0)]
public class ScriptableObjectMoveCard : ScriptableObject {
    private const int maxMoves = 4;
    public Vector2Int[] availableMoves = new Vector2Int[maxMoves];
    public string Name;
    public PlayerColor firstPlayerColor;
    public Sprite cardSprite;

    private void OnValidate() {
        if(availableMoves.Length != maxMoves){
            Debug.LogWarning("Every Move Card needs exactly 4 moves in its array, use -6 as default", this);
        }
    }
}