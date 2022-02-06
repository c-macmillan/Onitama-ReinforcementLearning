using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour{
    [SerializeField] protected Controller controller;
    [SerializeField] protected MoveCard[] PlayerMoveCards;
    [SerializeField] protected PlayerColor OwnerColor;
    public PlayerPiece MasterPiece;
    public MapLocation MasterStartingLocation;
    public List<PlayerPiece> availablePlayerPieces =>  GetPlayerPieces();
    protected List<Vector2Int> validMoveLocations =>  controller.GetValidMoves(focusedMoveCard, focusedPlayerPiece);
    public PlayerPiece focusedPlayerPiece {protected set; get;}
    public MoveCard focusedMoveCard {protected set; get;}
    public MapLocation focusedLocationTile {protected set; get;}

    private void Awake() {
        if(controller == null){
            controller = GetComponent<Controller>();
        }
    }

    private void Start(){
        foreach(PlayerPiece piece in availablePlayerPieces){
            if(piece.isMaster){
                MasterPiece = piece;
                MasterStartingLocation = piece.tile;
            }
        }
    }

    public bool HasMaster(){
        return (availablePlayerPieces.Contains(MasterPiece));
    }

    public List<PlayerPiece> GetPlayerPieces(){
        List<PlayerPiece> activePlayerPieces = new List<PlayerPiece>();
        PlayerPiece[] allPieces = FindObjectsOfType<PlayerPiece>();
        foreach(PlayerPiece playerPiece in allPieces){
            if(playerPiece.PlayerOwner == this){
                activePlayerPieces.Add(playerPiece);
            }
        }
        Debug.Log("Found " + activePlayerPieces.Count + " Player Pieces", this);
        return activePlayerPieces;
    }

    public void TakeTurn(){
        if(focusedMoveCard == null){
            ChooseCard();
        }
        if(focusedPlayerPiece == null){
            ChoosePiece();
        }
        if(focusedLocationTile == null){
        ChooseMove();
        }
    }
    virtual protected void ChooseCard(){
        controller.focusedMoveCard = PlayerMoveCards[Random.Range(0,1)];
    }

    virtual protected void ChoosePiece(){
        controller.focusedPlayerPiece = availablePlayerPieces[Random.Range(0, availablePlayerPieces.Count)];
    }

    virtual protected void ChooseMove(){
        List<Vector2Int> availableMoveLocations = validMoveLocations;
        Vector2Int chosenLocation = availableMoveLocations[Random.Range(0, availableMoveLocations.Count)];
        controller.focusedLocationTile = controller.gameMap.mapLocations[chosenLocation.x, chosenLocation.y];
    }

    public void ResetSelections(){
        focusedPlayerPiece = null;
        focusedMoveCard = null;
        focusedLocationTile = null;
    }      
}
