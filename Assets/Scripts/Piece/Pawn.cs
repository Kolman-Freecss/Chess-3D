using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    
    public override List<Vector2Int> GetAvailableMoves(ref Piece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (sideType == 0) ? 1 : -1;

        // One in front
        if (board[currentX, currentY + direction] == null)
            r.Add(new Vector2Int(currentX, currentY + direction));

        // Two in front
        if (board[currentX, currentY + direction] == null)
        {
            // White team
            if (sideType == 0 && currentY == 1 && board[currentX, currentY + direction * 2] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }
            // Black team
            if (sideType == 1 && currentY == 6 && board[currentX, currentY + direction * 2] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }
        }

        // Kill move
        if (currentX != tileCountX - 1)
        {
            if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].sideType != sideType)
            {
                r.Add(new Vector2Int(currentX + 1, currentY + direction));
            }
        }

        if (currentX != 0)
        {
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].sideType != sideType)
            {
                r.Add(new Vector2Int(currentX - 1, currentY + direction));
            }
        }

        return r;
    }
}
