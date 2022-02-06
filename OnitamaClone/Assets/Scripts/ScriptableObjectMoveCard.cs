using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveCard", menuName = "OnitamaClone/MoveCard", order = 0)]
public class ScriptableObjectMoveCard : ScriptableObject {
    public List<Vector2Int> availableMoves = new List<Vector2Int>();
    public string Name;
    public PlayerColor firstPlayerColor;
    public Sprite cardSprite;
}