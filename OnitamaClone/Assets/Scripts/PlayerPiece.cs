using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    public Player PlayerOwner;
    public PlayerColor PlayerOwnerColor;
    public bool isMaster = false;
    //public Vector2Int Location;
    public GameMap gameMap;
    public MapLocation tile;
    private MapLocation startingTile;
    [SerializeField] private Controller controller;

    void Start()
    {
        gameMap = GameObject.Find("GameBoard").GetComponent<GameMap>();
        startingTile = tile;
        PlayerOwner = PlayerOwnerColor == PlayerColor.BLUE_PLAYER ? controller.BluePlayer : controller.RedPlayer;
    }

    public void Move(MapLocation targetLocation){
        if(this.tile != null){
            this.tile.piece = null;
        }
        this.tile = targetLocation;
        targetLocation.piece = this;
        transform.position = new Vector3(targetLocation.transform.position.x, transform.position.y, targetLocation.transform.position.z);
    }

    public void Captured(){
        tile = null;
        PlayerOwner.RemoveFromBoard(this);
        this.gameObject.SetActive(false);
    }

    public void MoveToStart(){
        Move(startingTile);
    }
}
