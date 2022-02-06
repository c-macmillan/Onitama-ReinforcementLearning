using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

//This is the base class for all AI and human players, it should not be set to a specific player
public class Player : MonoBehaviour{
    [SerializeField] protected Controller controller;
    [SerializeField] protected MoveCard[] PlayerMoveCards;
    [SerializeField] protected PlayerColor OwnerColor;
    public PlayerPiece MasterPiece;
    public MapLocation MasterStartingLocation;
    protected List<Vector2Int> _validMoveLocations =>  controller.GetValidMoves(focusedMoveCard, focusedPlayerPiece);
    public PlayerPiece focusedPlayerPiece {protected set; get;}
    public MoveCard focusedMoveCard {protected set; get;}
    public MapLocation focusedLocationTile {protected set; get;}

    private void Awake() {
        if(controller == null){
            controller = GetComponent<Controller>();
        }
    }

    private void Start(){
        SetMasterPiece();
    }

    private void SetMasterPiece(){
        foreach(PlayerPiece piece in GetAvailablePlayerPieces()){
            if(piece.isMaster){
                MasterPiece = piece;
                MasterStartingLocation = piece.tile;
            }
        }
    }

    public bool HasMaster(){
        return (GetAvailablePlayerPieces().Contains(MasterPiece));
    }

    public List<PlayerPiece> GetAvailablePlayerPieces(){
        List<PlayerPiece> _activePlayerPieces = new List<PlayerPiece>();
        PlayerPiece[] _allPieces = FindObjectsOfType<PlayerPiece>();
        foreach(PlayerPiece _playerPiece in _allPieces){
            if(_playerPiece.PlayerOwner == this){
                _activePlayerPieces.Add(_playerPiece);
            }
        }
        Debug.Log("Found " + _activePlayerPieces.Count + " Player Pieces", this);
        return _activePlayerPieces;
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
    }

    virtual protected void ChoosePiece(){
    }

    virtual protected void ChooseMove(){
    }

    public void ResetSelections(){
        focusedPlayerPiece = null;
        focusedMoveCard = null;
        focusedLocationTile = null;
    }   

    protected void ConcedeGame(){
        Debug.Log(this + " concedes because they have no possible turn");
    }   
}
