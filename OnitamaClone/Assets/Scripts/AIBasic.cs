using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIBasic : Player
{
    private int _turnAttempts = 0;
    private int MaxIterations = 10;
    override protected void ChooseCard(){
        int index = Random.Range(0,1);
        focusedMoveCard = PlayerMoveCards[index];
    }

    override protected void ChoosePiece(){
        if(AvailablePieces.Count == 0){
            ConcedeGame();
            return;
        }
        int index = Random.Range(0, AvailablePieces.Count);
        focusedPlayerPiece = AvailablePieces[index];
    }

    override protected void ChooseMove(){
        List<Vector2Int> _availableMoveLocations = _validMoveLocations;
        if(_validMoveLocations.Count == 0){
            _turnAttempts += 1;
            if(_turnAttempts <= MaxIterations){
                ResetSelections();
                TakeTurn();
            }
            else{
                ConcedeGame();
                return;
            }
        }
        else{
            int index = Random.Range(0, _availableMoveLocations.Count);
            Vector2Int chosenLocation = _availableMoveLocations[index];
            focusedLocationTile = controller.GameMap.MapLocations[chosenLocation.x, chosenLocation.y];
            _turnAttempts = 0;
        }
    }
}
