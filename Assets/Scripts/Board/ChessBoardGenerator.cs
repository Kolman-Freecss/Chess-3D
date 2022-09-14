using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChessBoardGenerator : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    private Piece[,] pieces;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    void Awake()
    {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositioningAllPieces();
    }

    private void Update() 
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }
        HighlightTile();
        
    }

    private void HighlightTile()
    {
        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover")))
        {
            // Get the infexes of the tile i've hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we we're already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
        }
        else 
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
        }
    }

    // Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x,y] = GenerateSingleTile(tileSize, x, y);
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y+1) * tileSize) - bounds;
        vertices[2] = new Vector3((x+1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x+1) * tileSize, yOffset, (y+1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>().size = new Vector3(tileSize, 0.1f, tileSize);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return tileObject;
    }

    // Spawning of the pieces
    private void SpawnAllPieces()
    {
        pieces = new Piece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        // White team
        pieces[0,0] = SpawnSinglePiece(PieceType.Rook, whiteTeam);
        pieces[1,0] = SpawnSinglePiece(PieceType.Knight, whiteTeam);
        pieces[2,0] = SpawnSinglePiece(PieceType.Bishop, whiteTeam);
        pieces[3,0] = SpawnSinglePiece(PieceType.Queen, whiteTeam);
        pieces[4,0] = SpawnSinglePiece(PieceType.King, whiteTeam);
        pieces[5,0] = SpawnSinglePiece(PieceType.Bishop, whiteTeam);
        pieces[6,0] = SpawnSinglePiece(PieceType.Knight, whiteTeam);
        pieces[7,0] = SpawnSinglePiece(PieceType.Rook, whiteTeam);

        for (int i = 0; i < TILE_COUNT_X; i++)
            pieces[i, 1] = SpawnSinglePiece(PieceType.Pawn, whiteTeam);

        // Black team
        pieces[0,7] = SpawnSinglePiece(PieceType.Rook, blackTeam);
        pieces[1,7] = SpawnSinglePiece(PieceType.Knight, blackTeam);
        pieces[2,7] = SpawnSinglePiece(PieceType.Bishop, blackTeam);
        pieces[3,7] = SpawnSinglePiece(PieceType.Queen, blackTeam);
        pieces[4,7] = SpawnSinglePiece(PieceType.King, blackTeam);
        pieces[5,7] = SpawnSinglePiece(PieceType.Bishop, blackTeam);
        pieces[6,7] = SpawnSinglePiece(PieceType.Knight, blackTeam);
        pieces[7,7] = SpawnSinglePiece(PieceType.Rook, blackTeam);

        for (int i = 0; i < TILE_COUNT_X; i++)
            pieces[i, 6] = SpawnSinglePiece(PieceType.Pawn, blackTeam);
    }

    private Piece SpawnSinglePiece(PieceType type, int team)
    {
        Piece piece = Instantiate(prefabs[(int) type - 1], transform).GetComponent<Piece>();
        
        piece.pieceType = type;
        piece.sideType = team;

        piece.GetComponent<MeshRenderer>().material = teamMaterials[team];

        return piece;
    }
    
    // Positioning
    private void PositioningAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if(pieces[x,y] != null)
                    PositioningSinglePiece(x, y, true);
    }

    private void PositioningSinglePiece(int x, int y, bool force = false)
    {
        pieces[x, y].currentX = x;
        pieces[x, y].currentY = y;
        pieces[x, y].transform.position = GetTileCenter(x, y);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    // Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if(tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one; // Invalid
    }
}
