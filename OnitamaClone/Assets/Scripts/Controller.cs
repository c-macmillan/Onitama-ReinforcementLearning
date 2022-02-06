using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameMap gameMap;
    public List<ScriptableObjectMoveCard> deckOfCards;
    public MoveCard[] redPlayerCards = new MoveCard[2];
    public MoveCard[] bluePlayerCards = new MoveCard[2];
    public MoveCard sidelineCard;
    public Player activePlayer{private set; get;}
    public Player BluePlayer;
    public Player RedPlayer;
    public PlayerPiece focusedPlayerPiece;
    public MoveCard focusedMoveCard;
    public MapLocation focusedLocationTile;
    private bool turnComplete = false;
    private bool currentPlayerWon = false;
    private string LOCATION_TAG = "Location";

    private void Awake() {
        DealCards();
        activePlayer = sidelineCard.moveCardData.firstPlayerColor == PlayerColor.RED_PLAYER ? RedPlayer : BluePlayer;
    }

    private void Update() {
        if(currentPlayerWon){
            Debug.Log("You WIN!!!!!!");
            return;
        }
        if(!turnComplete){
            activePlayer.TakeTurn();
            ConfirmMove();
            CheckIfCurrentPlayerWon();
            }
        else{
            ChangeActivePlayer();
            ExchangeCards();
            ResetSelections();
            turnComplete = false;
        }
    }

    private void CheckIfCurrentPlayerWon(){
        Player opponent = activePlayer == RedPlayer ? BluePlayer : RedPlayer;
        if(!opponent.HasMaster()){
            currentPlayerWon = true;
        }
        if(opponent.MasterStartingLocation == activePlayer.MasterPiece.tile){
            currentPlayerWon = true;
        }
        if(opponent.availablePlayerPieces.Count == 0){
            currentPlayerWon = true;
        }

    }

    private void ChangeActivePlayer(){
        if(activePlayer == BluePlayer){
            activePlayer = RedPlayer;
        }
        else {
            activePlayer = BluePlayer;
        }
    }

    private void ExchangeCards(){
        focusedMoveCard.SwapCard(sidelineCard);
    }
    
    private void ResetSelections(){
        focusedLocationTile = null;
        focusedMoveCard = null;
        focusedPlayerPiece = null;
        activePlayer.ResetSelections();
    }

    private void ConfirmMove(){
        focusedLocationTile = activePlayer.focusedLocationTile;
        focusedMoveCard = activePlayer.focusedMoveCard;
        focusedPlayerPiece = activePlayer.focusedPlayerPiece;

        if(CheckIfValidMove(focusedPlayerPiece, focusedLocationTile, focusedMoveCard)){
            if(focusedLocationTile.piece != null){
                focusedLocationTile.piece.Captured();
                }
            focusedPlayerPiece.Move(focusedLocationTile);
            Debug.Log("Moved piece " + focusedPlayerPiece + " to " + focusedLocationTile, this);
            turnComplete = true;
        }
        else {
            Debug.Log("Invalid move, try again");
            focusedLocationTile = null;
        }
    }

    private bool CheckIfValidMove(PlayerPiece chosenPlayerPiece, MapLocation chosenLocationTile, MoveCard chosenMoveCard){
        
        List<Vector2Int> validMoves = GetValidMoves(chosenMoveCard, chosenPlayerPiece);
        if(validMoves.Contains(chosenLocationTile.Location)){
            return true;
        }
        else{
            return false;
        }
    }

    public List<Vector2Int> GetValidMoves(MoveCard chosenCard, PlayerPiece chosenPiece){
        List<Vector2Int> possibleMoves = MapMovesToBoardLocations(chosenCard.GetMoves(), chosenPiece);

        List<Vector2Int> validMoves = new List<Vector2Int>();
        foreach(Vector2Int move in possibleMoves){
            if(move.x < 0 | move.y < 0 | move.x >= gameMap.MapX | move.y >= gameMap.MapY){
                continue;
            }
            PlayerPiece otherPlayerPiece = gameMap.mapLocations[move.x, move.y].piece;
            if( otherPlayerPiece != null){
                if(chosenPiece.PlayerOwner == otherPlayerPiece.PlayerOwner){
                    continue;
                }
                else {validMoves.Add(move);}
            }
            else {
                validMoves.Add(move);
            }
        }

        return validMoves;
    }

    private void DealCards(){
        foreach (MoveCard card in redPlayerCards){
            card.moveCardData = DrawRandomCard();
            card.currentOwner = RedPlayer;
        }
        foreach (MoveCard card in bluePlayerCards){
            card.moveCardData = DrawRandomCard();
            card.currentOwner = BluePlayer;
        }
        sidelineCard.moveCardData = DrawRandomCard();
        sidelineCard.currentOwner = null;
    }

    private ScriptableObjectMoveCard DrawRandomCard(){
        int i = Random.Range(0, deckOfCards.Count);
        ScriptableObjectMoveCard card;
        card = deckOfCards[i];
        deckOfCards.RemoveAt(i);
        return card;

    }

    public void Clicked(MapLocation clickedLocation){
        if(clickedLocation.tag == LOCATION_TAG){
            focusedLocationTile = clickedLocation;
        }   
    }

    public void Clicked(MoveCard clickedMoveCard){
        if(clickedMoveCard.currentOwner == activePlayer){
            focusedMoveCard = clickedMoveCard;
        }
    }
    public void Clicked(PlayerPiece clickedPiece){
        if(clickedPiece.tag == "Piece"){
            focusedPlayerPiece = clickedPiece;
        }
        
    }

    public List<Vector2Int> getValidLocations(PlayerPiece focusedPiece, MoveCard focusedMoveCard){
        List<Vector2Int> validLocations = new List<Vector2Int>();
        validLocations = ValidateLocations(MapMovesToBoardLocations(focusedMoveCard.moveCardData.availableMoves, focusedPiece));      
        return validLocations;
    }

    private List<Vector2Int> ValidateLocations(List<Vector2Int> possibleLocations){
        List<Vector2Int> validLocations = new List<Vector2Int>();
        foreach(Vector2Int location in possibleLocations){
            if((location.x < 0) | (location.y < 0) | (location.x >= gameMap.MapX) | (location.y >= gameMap.MapY)) {
                continue;
            }
            PlayerPiece targetTilesPiece = gameMap.mapLocations[location.x, location.y].piece;
            if(targetTilesPiece != null){
                if(targetTilesPiece.PlayerOwner == activePlayer){
                continue;
                }
            }
            validLocations.Add(location);
        }
        return validLocations;
    }

    private List<Vector2Int> MapMovesToBoardLocations(List<Vector2Int> possibleLocations, PlayerPiece focusedPiece){
        List<Vector2Int> mappedLocations = new List<Vector2Int>();
        int playerFacingDirection = focusedPiece.PlayerOwner == BluePlayer ? 1 : -1;
        for(int i = 0; i < possibleLocations.Count; i++){
            Vector2Int location = new Vector2Int(playerFacingDirection * possibleLocations[i].x + focusedPiece.tile.Location.x, playerFacingDirection * possibleLocations[i].y + focusedPiece.tile.Location.y);
            mappedLocations.Add(location);
        }
        return mappedLocations;
    }

    List<Vector2Int> GetPlayerMoves(MoveCard[] playerCards){
        List<Vector2Int> moveLocations = new List<Vector2Int>();
        foreach(MoveCard card in playerCards){
            foreach(Vector2Int location in card.GetMoves()){
                    moveLocations.Add(location);
                    }
            }
        return moveLocations;
    }

    public MapLocation getMapLocationFromIndex(Vector2Int location){
        return gameMap.mapLocations[location.x, location.y];
    }
}
