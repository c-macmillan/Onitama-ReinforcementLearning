using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIBasic : Player
{
    private int iterations = 0;
    private int MaxIterations = 10;
    override protected void ChooseCard(){
        int index = Random.Range(0,1);
        focusedMoveCard = PlayerMoveCards[index];
    }

    override protected void ChoosePiece(){
        List<PlayerPiece> availablePieces = availablePlayerPieces;
        int index = Random.Range(0, availablePieces.Count);
        focusedPlayerPiece = availablePieces[index];
    }

    override protected void ChooseMove(){
        List<Vector2Int> availableMoveLocations = validMoveLocations;
        if(validMoveLocations.Count == 0){
            iterations += 1;
            if(iterations <= MaxIterations){
            ResetSelections();
            TakeTurn();
            }
            else{
                Debug.Log("Couldn't find a good turn");
                return;
            }
        }
        else{
            foreach(Vector2Int move in availableMoveLocations){
                Debug.Log("Possible choice to move to: " + move);
            }
            int index = Random.Range(0, availableMoveLocations.Count);
            Vector2Int chosenLocation = availableMoveLocations[index];
            focusedLocationTile = controller.gameMap.mapLocations[chosenLocation.x, chosenLocation.y];
            
            iterations = 0;
        }
    }
}
