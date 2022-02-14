using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;

//This is the base class for all AI and human players, it should not be set to a specific player
public class Player : MonoBehaviour
{
    [SerializeField] protected Controller controller;
    [SerializeField] public MoveCard[] PlayerMoveCards;
    [SerializeField] public PlayerColor OwnerColor;
    public List<PlayerPiece> AvailablePieces { protected set; get; }
    public PlayerPiece[] StartingPieces;
    public PlayerPiece MasterPiece;
    [HideInInspector] public MapLocation MasterStartingLocation;
    protected List<Vector2Int> _validMoveLocations => controller.GetValidMoveLocations(focusedMoveCard, focusedPlayerPiece);
    public PlayerPiece focusedPlayerPiece;
    public MoveCard focusedMoveCard;
    public MapLocation focusedLocationTile;
    public bool HasConceded = false;
    protected Player opponent;

    private void Awake()
    {
        if (controller == null)
        {
            controller = GetComponent<Controller>();
        }
    }

    private void Start()
    {
        AvailablePieces = new List<PlayerPiece>();
        opponent = controller.BluePlayer == this ? controller.RedPlayer : controller.BluePlayer;
    }
    public virtual void TakeTurn()
    {
        ChooseCard();
        ChoosePiece();
        ChooseMove();
        Debug.Log(this + " is trying to move " + focusedPlayerPiece + " to " + focusedLocationTile + " using " + focusedMoveCard);
        controller.EndTurn();
    }

    public virtual void Clicked(MapLocation clickedLocation)
    {
    }

    public virtual void Clicked(MoveCard clickedMoveCard)
    {
    }
    public virtual void Clicked(PlayerPiece clickedPiece)
    {
    }

    public List<float> GetPiecesStateInfo()
    {
        List<float> _piecesStateInfo = new List<float>();
        foreach (PlayerPiece _piece in StartingPieces)
        {
            if (_piece.isActiveAndEnabled)
            {
                _piecesStateInfo.Add(_piece.tile.Location.x);
                _piecesStateInfo.Add(_piece.tile.Location.y);
                _piecesStateInfo.Add(_piece.isMaster ? 1f : 0f);
            }
            else
            {
                _piecesStateInfo.Add(-1);
                _piecesStateInfo.Add(-1);
                _piecesStateInfo.Add(-1);
            }
        }

        while (_piecesStateInfo.Count < 15)
        {
            _piecesStateInfo.Add(-1f);
        }

        return _piecesStateInfo;
    }
    public List<float> GetMoveCardsInfo()
    {
        List<float> _moveCardsInfo = new List<float>();
        foreach (MoveCard moveCard in PlayerMoveCards)
        {
            foreach (float f in moveCard.GetMovesAsFloat())
            {
                _moveCardsInfo.Add(f);
            }
        }
        while (_moveCardsInfo.Count < 16)
        {
            _moveCardsInfo.Add(float.MinValue);
        }
        return _moveCardsInfo;
    }

    public void RemoveFromBoard(PlayerPiece _piece)
    {
        AvailablePieces.Remove(_piece);
    }

    virtual protected void ChooseCard()
    {
    }

    virtual protected void ChoosePiece()
    {
    }

    virtual protected void ChooseMove()
    {
    }

    public void ResetSelections()
    {
        focusedPlayerPiece = null;
        focusedMoveCard = null;
        focusedLocationTile = null;
    }

    public void ResetPieces()
    {
        foreach (PlayerPiece _piece in StartingPieces)
        {
            _piece.MoveToStart();
            _piece.gameObject.SetActive(true);
        }
    }

    protected virtual void ConcedeGame()
    {
        Debug.Log(this + " concedes because they have no possible turn");
        HasConceded = true;
    }
}
