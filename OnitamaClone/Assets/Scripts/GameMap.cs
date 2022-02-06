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
    public MapLocation[,] MapLocations = new MapLocation[5, 5];

    public void BoardSetUp()
    {
        GameBoard = this.transform;

        for (int y = 0; y < MapY; y++)
        {
            for (int x = 0; x < MapX; x++)
            {
                MapLocation _mapLocation = CreateMapTile(x,y);
                GameObject _piece = CreatePiece(_mapLocation);
                if(_piece != null){
                    PlayerPiece playerPiece = _piece.GetComponent<PlayerPiece>();
                    InitializeSettings(playerPiece, _mapLocation);
                }
            }
        }
    }

    private MapLocation CreateMapTile(int x, int y){
        GameObject Tile = Instantiate(MapTile, new Vector3(x * TileHeight, 0, y * TileWidth), Quaternion.identity);
        Tile.transform.SetParent(GameBoard);
        Tile.name = "MapTile(" + x + "," + y + ")";
        MapLocation mapLocation = Tile.GetComponent<MapLocation>();
        mapLocation.Location = new Vector2Int(x,y);
        MapLocations[x,y] = mapLocation;

        return mapLocation;
    }

    private GameObject CreatePiece(MapLocation mapLocation){
        int x = mapLocation.Location.x;
        int y = mapLocation.Location.y;
        GameObject piece = null;
        if(y == 0 | y == MapY - 1){
            
        // The middle spot is reserved for the master
            if(x == MapX / 2){
                mapLocation.isThrone = true;
                piece = Instantiate(Master, new Vector3(x * TileHeight, 3, y * TileWidth), Quaternion.identity);
                piece.GetComponent<PlayerPiece>().isMaster = true;
            }
            // It wasn't a master's spot, so spawn a student
            else piece = Instantiate(Student, new Vector3(x * TileHeight, 2, y * TileWidth), Quaternion.identity);
        }

        return piece;
    }

    private void InitializeSettings(PlayerPiece _playerPiece, MapLocation _mapLocation){
        int x = _mapLocation.Location.x;
        int y = _mapLocation.Location.y;
        _playerPiece.PlayerOwnerColor = y == 0 ? PlayerColor.BLUE_PLAYER : PlayerColor.RED_PLAYER;
        _playerPiece.PlayerOwner = y == 0 ? controller.BluePlayer : controller.RedPlayer;
        _playerPiece.gameMap = this;
        _playerPiece.tile = _mapLocation;
        _playerPiece.tile.Location = new Vector2Int(x,y);
        _playerPiece.GetComponent<MeshRenderer>().material = _playerPiece.PlayerOwnerColor == PlayerColor.BLUE_PLAYER ? Player1Mat : Player2Mat; 
        _playerPiece.transform.SetParent(GameBoard);
        _playerPiece.name = "Player " + _playerPiece.PlayerOwner + " " + (_playerPiece.isMaster ? "Master" : "Student");
        _mapLocation.piece = _playerPiece;
    }
}
