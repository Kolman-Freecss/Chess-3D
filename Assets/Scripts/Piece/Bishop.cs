using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    
    public override List<Vector2Int> GetAvailableMoves(ref Piece[,] board, int tileCountX, int tileCountY)
    {

        List<Vector2Int> r = new List<Vector2Int>();

        // Top right
        for (int x = currentX + 1, y = currentY + 1; x < tileCountX && y < tileCountY; x++, y ++)
        {
            if(board[x,y] == null)
                r.Add(new Vector2Int(x, y));
            else
            {
                if (board[x,y].sideType != sideType)
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }

        //Top Left
        for (int x = currentX - 1, y = currentY + 1; x >= 0 && y < tileCountY; x--, y ++)
        {
            if(board[x,y] == null)
                r.Add(new Vector2Int(x, y));
            else
            {
                if (board[x,y].sideType != sideType)
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }

        //Bottom Right
        for (int x = currentX + 1, y = currentY - 1; x < tileCountX && y >= 0; x++, y --)
        {
            if(board[x,y] == null)
                r.Add(new Vector2Int(x, y));
            else
            {
                if (board[x,y].sideType != sideType)
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }

        //Bottom Left
        for (int x = currentX - 1, y = currentY - 1; x >= 0 && y >= 0; x--, y --)
        {
            if(board[x,y] == null)
                r.Add(new Vector2Int(x, y));
            else
            {
                if (board[x,y].sideType != sideType)
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }

        return r;

    }
    
}
