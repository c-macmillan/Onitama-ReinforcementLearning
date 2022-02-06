using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    [SerializeField]Controller controller;
    GameObject clickedObject;
    bool clicked = false;
    // Start is called before the first frame update
    void Start()
    {
        if(controller == null){controller = GetComponentInParent<Controller>();}
        if(controller == null){controller = FindObjectOfType<Controller>();}
        clickedObject = this.transform.gameObject;
    }

    void OnMouseOver() {
        PlayerPiece clickedPiece;
        MapLocation clickedLocation;
        MoveCard clickedMoveCard;
        if(Input.GetMouseButton(0) & !clicked){
            if(clickedObject.GetComponent<MapLocation>()){
                clickedLocation = clickedObject.GetComponent<MapLocation>();
                controller.Clicked(clickedLocation);
                }

            if(clickedObject.GetComponent<PlayerPiece>()){
                clickedPiece = clickedObject.GetComponent<PlayerPiece>();
                controller.Clicked(clickedPiece);}

            if(clickedObject.GetComponent<MoveCard>()){
                clickedMoveCard = clickedObject.GetComponent<MoveCard>();
                controller.Clicked(clickedMoveCard);}
            clicked = true;
        }
    }
    
    void OnMouseUp() {
        clicked = false;
    }
}
