using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgentPlayer : AIGreedy
{
    [SerializeField] private OnitamaAgent onitamaAgent;
    private int turnAttempt = 0;

    public override void TakeTurn()
    {
        if (controller.GameOver)
        {
            return;
        }
        turnAttempt += 1;
        if (turnAttempt > 20)
        {
            Debug.Log("Ran out of attempts", this);
            turnAttempt = 0;
            ConcedeGame();
            return;
        }

        onitamaAgent.RequestDecision();
        onitamaAgent.AddReward(-10 * turnAttempt);
        StartCoroutine(GetRecommendation());
    }

    private IEnumerator GetRecommendation()
    {
        yield return StartCoroutine(onitamaAgent.WaitForResultsCoroutine());

        Move move = onitamaAgent.GetRecommendedMove();
        focusedMoveCard = move.chosenMoveCard;
        focusedPlayerPiece = move.chosenPlayerPiece;
        focusedLocationTile = move.targetMapLocation;
        onitamaAgent.AddReward(ScoreMove(move));
        if (move.chosenPlayerPiece.isActiveAndEnabled == false)
        {
            ConcedeGame();

        }
        else
        {
            turnAttempt = 0;
            controller.EndTurn();
        }
    }

    private int ScoreMove(Move move)
    {
        var vulnerableLocations = GetVulnerableLocations();
        int moveScore = 0;
        if (move.chosenPlayerPiece.isActiveAndEnabled == false)
        {
            moveScore -= 50;
            return moveScore;
        }
        else if(controller.GetValidMoveLocations(move.chosenMoveCard, move.chosenPlayerPiece).Contains(move.targetMapLocation.Location) == false){
            moveScore -= 50;
            return moveScore;
        }
        else{
            moveScore += 25;
        }


        if (vulnerableLocations.Contains(move.chosenPlayerPiece.tile.Location))
        {
            moveScore += move.chosenPlayerPiece == MasterPiece ? 100 : 25;
        }

        if (vulnerableLocations.Contains(move.targetMapLocation.Location))
        {
            moveScore -= move.chosenPlayerPiece == MasterPiece ? 200 : 15;
        }

        if(opponent == null){
            opponent = this == controller.RedPlayer ? controller.BluePlayer : controller.RedPlayer;
        }
        List<PlayerPiece> opponentPieces = opponent.AvailablePieces;
        foreach (PlayerPiece opponentPiece in opponentPieces)
        {
            if (opponentPiece.tile == move.targetMapLocation)
            {
                moveScore += opponentPiece.isMaster ? 1000 : 100;
            }
        }
        if (move.chosenPlayerPiece.isMaster & move.targetMapLocation.isThrone & move.targetMapLocation != MasterStartingLocation)
        {
            moveScore += 1000;
        }

        return moveScore;
    }

    protected override void ConcedeGame()
    {
        base.ConcedeGame();
        onitamaAgent.AddReward(-500);
        controller.EndTurn();
    }
}
