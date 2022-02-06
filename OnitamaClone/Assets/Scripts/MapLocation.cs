using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocation : MonoBehaviour
{
    public PlayerPiece piece;
    public bool isThrone = false;
    public Vector2Int Location;
    // Start is called before the first frame update
    void Start()
    {
        if(isThrone){
            // Change the visual so we know it is the player's throne
            // this.GetComponent<Material>().SetColor("_Player1", Color.blue);
        }
    }
}
