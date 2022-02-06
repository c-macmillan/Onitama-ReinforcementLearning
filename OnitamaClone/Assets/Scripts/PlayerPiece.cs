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

    // Start is called before the first frame update
    void Start()
    {
        gameMap = GameObject.Find("GameBoard").GetComponent<GameMap>();
        Debug.Log("Found " + gameMap);
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position = gameMap.MapIndexToWorld(Location.x,Location.y);
    }

    public void Move(MapLocation targetLocation){
        Debug.Log("Trying to move " + this + " to " + targetLocation);
        this.tile.piece = null;
        this.tile = targetLocation;
        targetLocation.piece = this;
        transform.position = new Vector3(targetLocation.transform.position.x, transform.position.y, targetLocation.transform.position.z);
    }

    public void Captured(){
        Debug.Log(this + " has been captured");
        if(this.isMaster){ Debug.Log((object)("PLAYER " + this.PlayerOwner + " JUST LOST!!!!"));}
        Destroy(this.gameObject);
    }
}
