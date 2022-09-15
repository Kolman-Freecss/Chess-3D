using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    
    public override List<Vector2Int> GetAvailableMoves(ref Piece[,] board, int tileCountX, int tileCountY)
    {

        List<Vector2Int> r = new List<Vector2Int>();

        // Down
        for (int i = currentY - 1; i >= 0; i--)
        {
            if(board[currentX, i] == null)
                r.Add(new Vector2Int(currentX, i));

            if(board[currentX, i] != null)
            {
                if (board[currentX, i].sideType != sideType)
                    r.Add(new Vector2Int(currentX, i));

                break;
            }
        }

        // Up
        for (int i = currentY + 1; i < tileCountY; i++)
        {
            if(board[currentX, i] == null)
                r.Add(new Vector2Int(currentX, i));

            if(board[currentX, i] != null)
            {
                if (board[currentX, i].sideType != sideType)
                    r.Add(new Vector2Int(currentX, i));

                break;
            }
        }

        // Left
        for (int i = currentX - 1; i >= 0; i--)
        {
            if(board[i, currentY] == null)
                r.Add(new Vector2Int(i, currentY));

            if(board[i, currentY] != null)
            {
                if (board[i, currentY].sideType != sideType)
                    r.Add(new Vector2Int(i, currentY));

                break;
            }
        }

        
        // Right
        for (int i = currentX + 1; i < tileCountX; i++)
        {
            if(board[i, currentY] == null)
                r.Add(new Vector2Int(i, currentY));

            if(board[i, currentY] != null)
            {
                if (board[i, currentY].sideType != sideType)
                    r.Add(new Vector2Int(i, currentY));

                break;
            }
        }


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
