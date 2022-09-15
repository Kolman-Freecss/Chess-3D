using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    
    public override List<Vector2Int> GetAvailableMoves(ref Piece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // Top right
        int x = currentX + 1;
        int y = currentY + 2;
        if (x < tileCountX && y < tileCountY)
        {
            if (board[x, y] == null || board[x, y].sideType != sideType)
            {
                r.Add(new Vector2Int(x, y));
            }
        }

        x = currentX + 2;
        y = currentY + 1;
        if (x < tileCountX && y < tileCountY)
        {
            if (board[x, y] == null || board[x, y].sideType != sideType)
            {
                r.Add(new Vector2Int(x, y));
            }
        }

        // Top Left
        x = currentX - 1;
        y = currentY + 2;
        if (x >= 0 && y < tileCountY)
            if (board[x, y] == null || board[x, y].sideType != sideType)
            {
                r.Add(new Vector2Int(x, y));
            }

        x = currentX - 2;
        y = currentY + 1;
        if (x >= 0 && y < tileCountY)
            if (board[x, y] == null || board[x, y].sideType != sideType)
            {
                r.Add(new Vector2Int(x, y));
            }

        // Bottom right
        x = currentX + 1;
        y = currentY - 2;
        if (x < tileCountX && y >= 0)
            if (board[x, y] == null || board[x, y].sideType != sideType)
            {
                r.Add(new Vector2Int(x, y));
            }

        x = currentX + 2;
        y = currentY - 1;
        if (x < tileCountX && y >= 0)
            if (board[x, y] == null || board[x, y].sideType != sideType)
            {
                r.Add(new Vector2Int(x, y));
            }

        // Bottom left
        x = currentX - 1;
        y = currentY - 2;
        if (x >= 0 && y >= 0)
            if (board[x, y] == null || board[x, y].sideType != sideType)
            {
                r.Add(new Vector2Int(x, y));
            }

        x = currentX - 2;
        y = currentY - 1;
        if (x >= 0 && y >= 0)
            if (board[x, y] == null || board[x, y].sideType != sideType)
            {
                r.Add(new Vector2Int(x, y));
            }

        return r;
    }
    
}
