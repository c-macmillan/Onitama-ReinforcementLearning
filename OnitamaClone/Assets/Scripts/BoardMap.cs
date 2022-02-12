using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMap : MonoBehaviour
{
    public int Mapx = 5;
    public int Mapy = 7;

    public enum Piece {PLAYER1_STUDENT, PLAYER1_MASTER, PLAYER2_STUDENT, PLAYER2_MASTER };
    public Piece[,] Map;

    // Start is called before the first frame update
    void Start()
    {
        CreateMap(Mapx, Mapy);
    }

    void CreateMap(int x, int y)
    {
        for (int i = 0; i < Mapx; i++)
        {
            if (i == x / 2) { Map[i, 0] = Piece.PLAYER1_MASTER; }
            else
            {
                Map[i, 0] = Piece.PLAYER1_STUDENT;
            }
        }

        for (int i = 0; i < Mapx; i++)
        {
            if (i == x / 2) { Map[i, Mapy] = Piece.PLAYER1_MASTER; }
            else
            {
                Map[i, Mapy] = Piece.PLAYER1_STUDENT;
            }
        }
    }
}
