using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    
    public override List<Vector2Int> GetAvailableMoves(ref Piece[,] board, int tileCountX, int tileCountY)
    {

        List<Vector2Int> r = new List<Vector2Int>();

        //Right

        if (currentX + 1 < tileCountX)
        {
            //Right
            if (board[currentX + 1, currentY] == null)
            {
                r.Add(new Vector2Int(currentX + 1, currentY));
            }
            else if (board[currentX + 1, currentY].sideType != sideType)
            {
                r.Add(new Vector2Int(currentX + 1, currentY));
            }

            // Top Right
            if (currentY + 1 < tileCountY)
            {
                if (board[currentX + 1, currentY + 1] == null)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY + 1));
                }
                else if (board[currentX + 1, currentY + 1].sideType != sideType)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY + 1));
                }
            }

            // Bottom right
            if (currentY - 1 >= 0)
            {
                if (board[currentX + 1, currentY - 1] == null)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY - 1));
                }
                else if (board[currentX + 1, currentY - 1].sideType != sideType)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY - 1));
                }
            }
        }

        //Left
        if (currentX - 1 >= 0)
        {
            //Left
            if (board[currentX - 1, currentY] == null)
            {
                r.Add(new Vector2Int(currentX - 1, currentY));
            }
            else if (board[currentX - 1, currentY].sideType != sideType)
            {
                r.Add(new Vector2Int(currentX - 1, currentY));
            }

            // Top Right
            if (currentY + 1 < tileCountY)
            {
                if (board[currentX - 1, currentY + 1] == null)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY + 1));
                }
                else if (board[currentX - 1, currentY + 1].sideType != sideType)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY + 1));
                }
            }

            // Bottom right
            if (currentY - 1 >= 0)
            {
                if (board[currentX - 1, currentY - 1] == null)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY - 1));
                }
                else if (board[currentX - 1, currentY - 1].sideType != sideType)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY - 1));
                }
            }
        }

        //Up
        if (currentY + 1 < tileCountY)
        {
            if(board[currentX, currentY + 1] == null || board[currentX, currentY + 1].sideType != sideType)
            {
                r.Add(new Vector2Int(currentX, currentY + 1));
            }
        }

        //Down
        if (currentY - 1 >= 0)
        {
            if(board[currentX, currentY - 1] == null || board[currentX, currentY - 1].sideType != sideType)
            {
                r.Add(new Vector2Int(currentX, currentY - 1));
            }
        }

        return r;

    }
    
}
