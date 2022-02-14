public struct Move
{
    public Move(PlayerPiece _chosenPlayerPiece, MoveCard _chosenMoveCard, MapLocation _targetLocation)
    {
        chosenPlayerPiece = _chosenPlayerPiece;
        chosenMoveCard = _chosenMoveCard;
        targetMapLocation = _targetLocation;
    }

    public PlayerPiece chosenPlayerPiece;
    public MoveCard chosenMoveCard;
    public MapLocation targetMapLocation;
}
