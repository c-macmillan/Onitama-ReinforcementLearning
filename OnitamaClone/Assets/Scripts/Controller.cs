using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameMap GameMap;
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
        GameMap.BoardSetUp();
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
        if(opponent.HasMaster() == false){
            currentPlayerWon = true;
        }
        if(opponent.MasterStartingLocation == activePlayer.MasterPiece.tile){
            currentPlayerWon = true;
        }
        if(opponent.GetAvailablePlayerPieces().Count == 0){
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

        if(IsValidMove(focusedPlayerPiece, focusedLocationTile, focusedMoveCard)){
            if(focusedLocationTile.piece != null){
                focusedLocationTile.piece.Captured();
                }
            focusedPlayerPiece.Move(focusedLocationTile);
            turnComplete = true;
        }
        else {
            Debug.Log("Cannot move " + focusedPlayerPiece + " to " + focusedLocationTile + " using " + focusedMoveCard);
            focusedLocationTile = null;
        }
    }

    private bool IsValidMove(PlayerPiece chosenPlayerPiece, MapLocation chosenLocationTile, MoveCard chosenMoveCard){
        
        List<Vector2Int> validMoves = GetValidMoves(chosenMoveCard, chosenPlayerPiece);
        return validMoves.Contains(chosenLocationTile.Location);
    }

    public List<Vector2Int> GetValidMoves(MoveCard chosenCard, PlayerPiece chosenPiece){
        List<Vector2Int> possibleMoves = MapMovesToBoardLocations(chosenCard.GetMoves(), chosenPiece);

        List<Vector2Int> validMoves = new List<Vector2Int>();
        foreach(Vector2Int move in possibleMoves){
            if(move.x < 0 | move.y < 0 | move.x >= GameMap.MapX | move.y >= GameMap.MapY){
                continue;
            }
            PlayerPiece otherPlayerPiece = GameMap.MapLocations[move.x, move.y].piece;
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

    private List<Vector2Int> MapMovesToBoardLocations(List<Vector2Int> possibleLocations, PlayerPiece focusedPiece){
        List<Vector2Int> mappedLocations = new List<Vector2Int>();
        int playerFacingDirection = focusedPiece.PlayerOwner == BluePlayer ? 1 : -1;
        for(int i = 0; i < possibleLocations.Count; i++){
            Vector2Int location = new Vector2Int(playerFacingDirection * possibleLocations[i].x + focusedPiece.tile.Location.x, playerFacingDirection * possibleLocations[i].y + focusedPiece.tile.Location.y);
            mappedLocations.Add(location);
        }
        return mappedLocations;
    }
}
