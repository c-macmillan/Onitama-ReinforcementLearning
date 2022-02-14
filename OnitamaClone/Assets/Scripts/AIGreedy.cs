using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGreedy : Player
{
    private List<Vector2Int> vulnerableLocations;
    public override void TakeTurn()
    {
        if(controller.GameOver){
            return;
        }
        SelectBestMove();
        controller.EndTurn();
    }

    protected List<Vector2Int> GetVulnerableLocations(){
        List<Vector2Int> _vulnerableLocations = new List<Vector2Int>();
        foreach(PlayerPiece opponentPiece in opponent.AvailablePieces)
        {
            foreach(MoveCard oppMoveCard in opponent.PlayerMoveCards){
                foreach(Vector2Int location in controller.GetValidMoveLocations(oppMoveCard, opponentPiece)){
                    _vulnerableLocations.Add(location);
                }
            }
        }
        return _vulnerableLocations;
    }

    protected void SelectBestMove(){
        Move bestMove = new Move();
        int bestMoveScore = int.MinValue;
        vulnerableLocations = GetVulnerableLocations();
        List<Move> allValidMoves = GetAllValidMoves();
        foreach(Move move in allValidMoves){
            int moveScore = ScoreMove(move);
            if(moveScore > bestMoveScore){
                bestMove = move;
                bestMoveScore = moveScore;
            }
        }

        focusedPlayerPiece = bestMove.chosenPlayerPiece;
        focusedLocationTile = bestMove.targetMapLocation;
        focusedMoveCard = bestMove.chosenMoveCard;
    }

    private int ScoreMove(Move move){
        int moveScore = Random.Range(-1,1);
        if(vulnerableLocations.Contains(move.chosenPlayerPiece.tile.Location)){
            moveScore += move.chosenPlayerPiece == MasterPiece ? 20 : 1;
        }
        if(vulnerableLocations.Contains(move.targetMapLocation.Location)){
            moveScore -= move.chosenPlayerPiece == MasterPiece ? 20 : 1;
        }
        List<PlayerPiece> opponentPieces = opponent.AvailablePieces;
        foreach(PlayerPiece opponentPiece in opponentPieces){
            if(opponentPiece.tile == move.targetMapLocation){
                moveScore += opponentPiece.isMaster ? 50 : 5;
            }
        }
        if(move.chosenPlayerPiece.isMaster & move.targetMapLocation.isThrone & move.targetMapLocation != MasterStartingLocation){
            moveScore += 50;
        }

        return moveScore;
    }

    private List<Move> GetAllValidMoves(){
        List<Move> allValidMoves = new List<Move>();
        foreach(PlayerPiece activePiece in AvailablePieces){
            foreach(MoveCard moveCard in PlayerMoveCards){
                List<Vector2Int> validMoveLocations = controller.GetValidMoveLocations(moveCard, activePiece);
                foreach(Vector2Int validMoveLocation in validMoveLocations){
                    MapLocation targetLocation = controller.GameMap.MapLocations[validMoveLocation.x, validMoveLocation.y];
                    Move validMove = new Move(activePiece, moveCard, targetLocation);
                    allValidMoves.Add(validMove);
                }
            }
        }
        return allValidMoves;
    }
}
