using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    private bool _isMyTurn = false;
    private readonly string LOCATION_TAG = "Location";
    private void Update() {
        if(_isMyTurn == false){
            return;
        }
        else if(focusedLocationTile != null & focusedMoveCard != null & focusedPlayerPiece != null){
            _isMyTurn = false;
            controller.EndTurn();
        }
    }

    public override void Clicked(MapLocation clickedLocation){
        if(clickedLocation.tag == LOCATION_TAG){
            focusedLocationTile = clickedLocation;
        }   
    }

    public override void Clicked(MoveCard clickedMoveCard){
        if(clickedMoveCard.currentOwner == this){
            focusedMoveCard = clickedMoveCard;
        }
    }
    public override void Clicked(PlayerPiece clickedPiece){
        if(clickedPiece.tag == "Piece"){
            focusedPlayerPiece = clickedPiece;
        }
    }

    public override void TakeTurn()
    {
        _isMyTurn = true;
    }
}
