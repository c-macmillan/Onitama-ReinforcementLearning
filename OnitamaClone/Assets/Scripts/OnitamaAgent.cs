using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class OnitamaAgent : Agent
{
    private int _playerPieceIndex;
    private int _moveCardIndex;
    private int _targetLocationX;
    private int _targetLocationY;
    MapLocation selectedLocation;
    MoveCard selectedMoveCard;
    PlayerPiece selectedPlayerPiece;
    private Player opponent;
    private Player agentPlayer;
    private bool resultsGenerated = false;
    [SerializeField] private Controller controller;

    private void Start()
    {
        agentPlayer = GetComponent<AIAgentPlayer>();
        opponent = agentPlayer == controller.BluePlayer ? controller.RedPlayer : controller.BluePlayer;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //Max Observations = 74
        //Owned Pieces (x, y, isMaster), Opponent Pieces (x, y, isMaster) 
        // Captured Pieces have default of -1 for all
        // max = 5pieces * 3stateInfo * 2players = 30
        //Card1ValidMoves (localX, localY), Card2ValidMoves(localX, localY)
        // Cards w/ less than 4 moves, or invalid moves default to int.min
        // max = 2cards * 4possibleMoves * 2players * 2stateInfo = 32
        //SidelineCard (localX, localY)
        // max = 4possibleMoves * 2stateInfo = 8
        //MyMasterStartingLocation (x, y), OpponentMasterStartingLocation(x,y)
        // max = 2 * 2 = 4
        sensor.AddObservation(agentPlayer.GetPiecesStateInfo());
        sensor.AddObservation(opponent.GetPiecesStateInfo());

        sensor.AddObservation(agentPlayer.GetMoveCardsInfo());
        sensor.AddObservation(opponent.GetMoveCardsInfo());

        sensor.AddObservation(controller.sidelineCard.GetMovesAsFloat());

        sensor.AddObservation(agentPlayer.MasterStartingLocation.Location);
        sensor.AddObservation(opponent.MasterStartingLocation.Location);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        _playerPieceIndex = actions.DiscreteActions[0];
        _moveCardIndex = actions.DiscreteActions[1];
        _targetLocationX = actions.DiscreteActions[2];
        _targetLocationY = actions.DiscreteActions[3];

        selectedLocation = controller.GameMap.MapLocations[_targetLocationX, _targetLocationY];
        selectedMoveCard = agentPlayer.PlayerMoveCards[_moveCardIndex];
        selectedPlayerPiece = agentPlayer.StartingPieces[_playerPieceIndex];

        resultsGenerated = true;
    }

    public Move GetRecommendedMove()
    {
        Move recommendedMove = new Move(selectedPlayerPiece, selectedMoveCard, selectedLocation);
        resultsGenerated = false;
        return recommendedMove;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Random.Range(0, 5);
        discreteActions[1] = Random.Range(0, 2);
        discreteActions[2] = Random.Range(0, 5);
        discreteActions[3] = Random.Range(0, 5);
    }

    public IEnumerator WaitForResultsCoroutine()
    {
        while(resultsGenerated == false){
            yield return null;
        }
        yield return new WaitUntil(() => resultsGenerated);
    }

}
