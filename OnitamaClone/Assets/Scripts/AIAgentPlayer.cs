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
            Debug.Log("Chose a captured piece");
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
        int moveScore = Random.Range(-1, 1);
        if (move.chosenPlayerPiece.isActiveAndEnabled == false)
        {
            moveScore -= 50;
            return moveScore;
        }

        if (vulnerableLocations.Contains(move.chosenPlayerPiece.tile.Location))
        {
            moveScore += move.chosenPlayerPiece == MasterPiece ? 30 : 2;
        }

        if (vulnerableLocations.Contains(move.targetMapLocation.Location))
        {
            moveScore -= move.chosenPlayerPiece == MasterPiece ? 20 : 1;
        }

        List<PlayerPiece> opponentPieces = opponent.AvailablePieces;
        foreach (PlayerPiece opponentPiece in opponentPieces)
        {
            if (opponentPiece.tile == move.targetMapLocation)
            {
                moveScore += opponentPiece.isMaster ? 50 : 5;
            }
        }
        if (move.chosenPlayerPiece.isMaster & move.targetMapLocation.isThrone & move.targetMapLocation != MasterStartingLocation)
        {
            moveScore += 50;
        }

        return moveScore;
    }

    protected override void ConcedeGame()
    {
        base.ConcedeGame();
        controller.EndTurn();
    }
}
