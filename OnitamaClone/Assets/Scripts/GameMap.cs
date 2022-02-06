using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    public int MapX {get; private set;} = 5;
    public int MapY {get; private set;} = 5; 
    [SerializeField] private GameObject MapTile;
    [SerializeField] private GameObject Student;
    [SerializeField] private GameObject Master;
    [SerializeField] private Material Player1Mat;
    [SerializeField] private Material Player2Mat;
    [SerializeField] private Controller controller;
    private float TileWidth = 10;
    private float TileHeight = 10;
    private Transform GameBoard;
    public MapLocation[,] mapLocations = new MapLocation[5, 5];
    // Start is called before the first frame update
    void Awake()
    {
        BoardSetUp();
    }

    public Vector3 MapIndexToWorld(int x, int y){
        return new Vector3(x * TileHeight, 0, y * TileWidth);
    }

    private void BoardSetUp()
    {
        GameBoard = this.transform;

        for (int y = 0; y < MapY; y++)
        {
            for (int x = 0; x < MapX; x++)
            {
                // Create the map tile
                GameObject Tile = Instantiate(MapTile, new Vector3(x * TileHeight, 0, y * TileWidth), Quaternion.identity);
                Tile.transform.SetParent(GameBoard);
                Tile.name = "MapTile(" + x + "," + y + ")";
                MapLocation mapLocation = Tile.GetComponent<MapLocation>();
                mapLocation.Location = new Vector2Int(x,y);
                mapLocations[x,y] = mapLocation; // Create the array of locations

                // Put the correct piece in its starting position
                // We only put pieces on the bottom row and the top row
                if(y == 0 | y == MapY - 1){
                    GameObject Piece;
                    // The middle spot is reserved for the master
                    if(x == MapX / 2){
                        mapLocation.isThrone = true;
                        Piece = Instantiate(Master, new Vector3(x * TileHeight, 3, y * TileWidth), Quaternion.identity);
                        Piece.GetComponent<PlayerPiece>().isMaster = true;
                    }
                    // It wasn't a master's spot, so spawn a student
                    else Piece = Instantiate(Student, new Vector3(x * TileHeight, 2, y * TileWidth), Quaternion.identity);
                 // Set the piece's player
                 PlayerPiece playerPiece = Piece.GetComponent<PlayerPiece>();
                 playerPiece.PlayerOwnerColor = y == 0 ? PlayerColor.BLUE_PLAYER : PlayerColor.RED_PLAYER;
                 playerPiece.PlayerOwner = y == 0 ? controller.BluePlayer : controller.RedPlayer;
                 playerPiece.gameMap = this;
                 playerPiece.tile = mapLocation;
                 playerPiece.tile.Location = new Vector2Int(x,y);
                 playerPiece.GetComponent<MeshRenderer>().material = playerPiece.PlayerOwnerColor == PlayerColor.BLUE_PLAYER ? Player1Mat : Player2Mat; 
                playerPiece.transform.SetParent(GameBoard);
                playerPiece.name = "Player " + playerPiece.PlayerOwner + " " + (playerPiece.isMaster ? "Master" : "Student");
                mapLocation.piece = playerPiece;
            }
        }
    }
}
}
