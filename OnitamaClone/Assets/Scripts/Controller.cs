using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameMap GameMap;
    public DeckOfCards deckOfCards;
    public MoveCard[] redPlayerCards = new MoveCard[2];
    public MoveCard[] bluePlayerCards = new MoveCard[2];
    public MoveCard sidelineCard;
    public Player activePlayer{private set; get;}
    public Player BluePlayer;
    public Player RedPlayer;
    public PlayerPiece focusedPlayerPiece;
    public MoveCard focusedMoveCard;
    public MapLocation focusedLocationTile;
    private bool _activePlayerStartedTurn = false;
    public bool PlayTheGame = false;
    public bool GameOver{private set; get;} = false;

    private void Start() {
        DealCards();
        activePlayer = sidelineCard.moveCardData.firstPlayerColor == PlayerColor.RED_PLAYER ? RedPlayer : BluePlayer;
        AssignPiecesToPlayers();
        //GameMap.BoardSetUp();
    }

    private void Update(){
        if(PlayTheGame == false){
            return;
        }
        else if(_activePlayerStartedTurn == false){
            _activePlayerStartedTurn = true;
            activePlayer.TakeTurn();
        }
    }

    public void EndTurn(){
        Player opponent = activePlayer == BluePlayer ? RedPlayer : BluePlayer;
        if(activePlayer.HasConceded){
            activePlayer = opponent;
            WinScreen();
            return;
        }
        if(MoveConfirmed() == false){
            activePlayer.TakeTurn();
            return;
        }
        else if(HasCurrentPlayerWon()){
            WinScreen();
        }
        else {
            ChangeActivePlayer();
            ExchangeCards();
            ResetSelections();
            _activePlayerStartedTurn = false;
        }
    }

    private void WinScreen(){
        Debug.Log(activePlayer + " WINS !!!!!!");
        GameOver = true;
        Player opponentPlayer = activePlayer == BluePlayer ? RedPlayer : BluePlayer;
        OnitamaAgent activePlayerAgent;
        OnitamaAgent opponentPlayerAgent;
        if (activePlayer.TryGetComponent<OnitamaAgent>(out activePlayerAgent))
        {
            if (activePlayerAgent.isActiveAndEnabled)
            {
                activePlayerAgent.AddReward(50);
                activePlayerAgent.EndEpisode();
            }
        }
        else if (opponentPlayer.TryGetComponent<OnitamaAgent>(out opponentPlayerAgent))
        {
            if (opponentPlayerAgent.isActiveAndEnabled)
            {
                opponentPlayerAgent.AddReward(-50);
                opponentPlayerAgent.EndEpisode();
            }
        }
        //PlayTheGame = false;
        RestartGame();

    }
    private void RestartGame(){
        ResetGame();
        DealCards();
        activePlayer = sidelineCard.moveCardData.firstPlayerColor == PlayerColor.RED_PLAYER ? RedPlayer : BluePlayer;
        _activePlayerStartedTurn = false;
        GameOver = false;
    }


    private void ResetGame(){
        BluePlayer.HasConceded = false;
        RedPlayer.HasConceded = false;
        BluePlayer.ResetPieces();
        RedPlayer.ResetPieces();
        BluePlayer.ResetSelections();
        RedPlayer.ResetSelections();
        ResetSelections();
        deckOfCards.ResetDeck();
        AssignPiecesToPlayers();
    }

    private bool HasCurrentPlayerWon(){
        Player opponent = activePlayer == RedPlayer ? BluePlayer : RedPlayer;
        if(opponent.AvailablePieces.Contains(opponent.MasterPiece) == false){
            return true;
        }
        if(opponent.MasterStartingLocation == activePlayer.MasterPiece.tile){
            Debug.Log(activePlayer + " Sat on " + opponent + "'s throne");
            return true;
        }
        if(opponent.AvailablePieces.Count == 0){
            Debug.Log(opponent + " Has no pieces to move");
            return true;
        }

        return false;
    }

    private void AssignPiecesToPlayers()
    {
        PlayerPiece[] _allPieces = FindObjectsOfType<PlayerPiece>();
        foreach (PlayerPiece _playerPiece in _allPieces)
        {
            if (_playerPiece.PlayerOwnerColor == BluePlayer.OwnerColor)
            {
                if (BluePlayer.AvailablePieces.Contains(_playerPiece) == false)
                {
                    BluePlayer.AvailablePieces.Add(_playerPiece);
                    if (_playerPiece.isMaster & BluePlayer.MasterPiece == null)
                    {
                        BluePlayer.MasterPiece = _playerPiece;
                        BluePlayer.MasterStartingLocation = _playerPiece.tile;
                    }
                }
            }
            else if (_playerPiece.PlayerOwnerColor == RedPlayer.OwnerColor)
            {
                if (RedPlayer.AvailablePieces.Contains(_playerPiece) == false)
                {
                    RedPlayer.AvailablePieces.Add(_playerPiece);
                    if (_playerPiece.isMaster & RedPlayer.MasterPiece == null)
                    {
                        RedPlayer.MasterPiece = _playerPiece;
                        RedPlayer.MasterStartingLocation = _playerPiece.tile;
                    }
                }
            }
        }
    }

    private void ChangeActivePlayer(){
        activePlayer = activePlayer == BluePlayer ? RedPlayer : BluePlayer;
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

    private bool MoveConfirmed(){
        focusedLocationTile = activePlayer.focusedLocationTile;
        focusedMoveCard = activePlayer.focusedMoveCard;
        focusedPlayerPiece = activePlayer.focusedPlayerPiece;
        if(focusedLocationTile == null | focusedMoveCard == null | focusedPlayerPiece == null){
            return false;
        }

        if(IsValidMove(focusedPlayerPiece, focusedLocationTile, focusedMoveCard)){
            if(focusedLocationTile.piece != null){
                focusedLocationTile.piece.Captured();
                }
            focusedPlayerPiece.Move(focusedLocationTile);
        }
        else {
            focusedLocationTile = null;
        }

        return (focusedLocationTile != null);
    }

    private bool IsValidMove(PlayerPiece chosenPlayerPiece, MapLocation chosenLocationTile, MoveCard chosenMoveCard){
        
        List<Vector2Int> validMoves = GetValidMoveLocations(chosenMoveCard, chosenPlayerPiece);
        return validMoves.Contains(chosenLocationTile.Location);
    }

    public List<Vector2Int> GetValidMoveLocations(MoveCard chosenCard, PlayerPiece chosenPiece){
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
            card.moveCardData = deckOfCards.DrawRandomCard();
            card.currentOwner = RedPlayer;
        }
        foreach (MoveCard card in bluePlayerCards){
            card.moveCardData = deckOfCards.DrawRandomCard();
            card.currentOwner = BluePlayer;
        }
        sidelineCard.moveCardData = deckOfCards.DrawRandomCard();
        sidelineCard.currentOwner = null;
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
