using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChessBoardGenerator : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSize = 0.3f;
    [SerializeField] private float deathSpacing = 0.3f;
    [SerializeField] private float dragOffset = 0.75f;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private Transform rematchIndicator;
    [SerializeField] private Button rematchButton;
    

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    private Piece[,] pieces;
    private Piece currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<Piece> deadWhites = new List<Piece>();
    private List<Piece> deadBlacks = new List<Piece>();
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private bool isWhiteTurn;

    // Multiplayer stuff
    private int playerCount = -1;
    private int currentTeam = -1;
    private bool localGame = true;
    private bool[] playerRematch = new bool[2];

    void Start()
    {
        isWhiteTurn = true;
        victoryScreen.SetActive(false);

        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositioningAllPieces();

        RegisterEvents();
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
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
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
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we press down on the mouse
            if (Input.GetMouseButtonDown(0))
            {
                if (pieces[hitPosition.x, hitPosition.y] != null)
                {
                    if ((pieces[hitPosition.x, hitPosition.y].sideType == 0 && isWhiteTurn && currentTeam == 0)
                        || (pieces[hitPosition.x, hitPosition.y].sideType == 1 && !isWhiteTurn && currentTeam == 1))
                    {
                        currentlyDragging = pieces[hitPosition.x, hitPosition.y];

                        // Get a list of where i can go, hightlight tiles as well
                        availableMoves = currentlyDragging.GetAvailableMoves(ref pieces, TILE_COUNT_X, TILE_COUNT_Y);
                        HighlightTiles();
                    }
                }
            }

            // If we are releasing the mouse button
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

                if(ContainsValidMove(ref availableMoves, new Vector2(hitPosition.x,hitPosition.y)))
                {
                    MoveTo(previousPosition.x, previousPosition.y, hitPosition.x, hitPosition.y);

                    // Net implementation
                    NetMakeMove mm = new NetMakeMove();
                    mm.originalX = previousPosition.x;
                    mm.originalY = previousPosition.y;
                    mm.destinationX = hitPosition.x;
                    mm.destinationY = hitPosition.y;
                    mm.teamId = currentTeam;
                    Client.Instance.SendToServer(mm);
                }
                else
                {
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
                    currentlyDragging = null;
                    RemoveHighlightTiles();
                }
            }

        }
        else 
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }

        // If we're dragging a picee
        if (currentlyDragging)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance))
            {
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
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
        pieces[x, y].SetPosition(GetTileCenter(x, y), force);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    //Highlight Tiles
    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer  = LayerMask.NameToLayer("Highlight");
        }
    }

    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer  = LayerMask.NameToLayer("Tile");
        }

        availableMoves.Clear();
    }

    // Checkmate
    private void CheckMate(int team)
    {
        DisplayVictory(team);
    }

    private void DisplayVictory(int winningTeam)
    {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }

    public void OnRematchButton()
    {
        if (localGame)
        {
            NetRematch wrm = new NetRematch();
            wrm.teamId = 0;
            wrm.wantRematch = 1;
            Client.Instance.SendToServer(wrm);

            NetRematch brm = new NetRematch();
            brm.teamId = 1;
            brm.wantRematch = 1;
            Client.Instance.SendToServer(brm);
        }
        else
        {
            NetRematch rm = new NetRematch();
            rm.teamId = currentTeam;
            rm.wantRematch = 1;
            Client.Instance.SendToServer(rm);
        }
        

    }

    public void GameReset()
    {
         // UI

        rematchButton.interactable = true;

        rematchIndicator.transform.GetChild(0).gameObject.SetActive(false);
        rematchIndicator.transform.GetChild(1).gameObject.SetActive(false);

        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.SetActive(false);

        //Fields reset
        currentlyDragging = null;
        availableMoves.Clear();
        playerRematch[0] = playerRematch[1] = false;

        // Clean up
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (pieces[x,y] != null)
                {
                    Destroy(pieces[x, y].gameObject);
                }
                pieces[x, y] = null;
            }
        }

        for (int i = 0; i < deadWhites.Count; i++)
        {
            Destroy(deadWhites[i].gameObject);
        }
        for (int i = 0; i < deadBlacks.Count; i++)
        {
            Destroy(deadBlacks[i].gameObject);
        }

        deadWhites.Clear();
        deadBlacks.Clear();

        SpawnAllPieces();
        PositioningAllPieces();
        isWhiteTurn = true;
    }

    public void OnMenuButton()
    {
        NetRematch rm = new NetRematch();
        rm.teamId = currentTeam;
        rm.wantRematch = 0;
        Client.Instance.SendToServer(rm);

        GameReset();
        GameUI.Instance.OnLeaveFromGameMenu();

        Invoke("ShutdownRelay", 1.0f);

        //Reset some values
        playerCount = -1;
        currentTeam = -1;
    }
    
    // Operations
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y) 
            {
                return true;
            }
        }
        return false;
    }

    private void MoveTo(int originalX, int originalY, int x, int y)
    {
        Piece cp = pieces[originalX, originalY];
        Vector2Int previousPosition = new Vector2Int(originalX, originalY);

        if (pieces[x, y] != null)
        {
            Piece ocp = pieces[x, y];
            if (cp.sideType == ocp.sideType)
            {
                return;
            }

            // If its the enemy team
            if (ocp.sideType == 0)
            {
                if (ocp.pieceType == PieceType.King)
                {
                    CheckMate(1);
                }

                deadWhites.Add(ocp);
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(new Vector3 (8 * tileSize, yOffset, -1 * tileSize) 
                                            - bounds 
                                            + new Vector3(tileSize / 2, 0, tileSize / 2) 
                                            + (Vector3.forward * deathSpacing) * deadWhites.Count);
            }
            else
            {
                if (ocp.pieceType == PieceType.King)
                {
                    CheckMate(0);
                }

                deadBlacks.Add(ocp);
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(new Vector3 (-1 * tileSize, yOffset, 8 * tileSize) 
                                            - bounds 
                                            + new Vector3(tileSize / 2, 0, tileSize / 2) 
                                            + (Vector3.back * deathSpacing) * deadBlacks.Count);
            }
        }

        pieces[x, y] = cp;
        pieces[previousPosition.x, previousPosition.y] = null;

        PositioningSinglePiece(x, y);

        isWhiteTurn = !isWhiteTurn;

        if (localGame)
        {
            currentTeam = (currentTeam == 0) ? 1 : 0;
        }

        if (currentlyDragging)
            currentlyDragging = null;
        RemoveHighlightTiles();

        return;
    }
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if(tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one; // Invalid
    }

    #region 

    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;
        NetUtility.S_REMATCH += OnRematchServer;

        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGameClient;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
        NetUtility.C_REMATCH += OnRematchClient;

        GameUI.Instance.SetLocalGame += OnSetLocalGame;
    }

    private void UnregisterEvents()
    {
        NetUtility.S_WELCOME -= OnWelcomeServer;
        NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;
        NetUtility.S_REMATCH -= OnRematchServer;    

        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_START_GAME -= OnStartGameClient;
        NetUtility.C_MAKE_MOVE -= OnMakeMoveClient;
        NetUtility.S_REMATCH -= OnRematchServer;

        GameUI.Instance.SetLocalGame -= OnSetLocalGame;
    }

    // Server
    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        NetWelcome netWelcome = msg as NetWelcome;

        // Assign a team
        netWelcome.AssignedTeam = ++playerCount;

        // Return back to the client
        Server.Instance.SendToClient(cnn, netWelcome);

        if (playerCount == 1)
        {
            // Start the game
            Server.Instance.Broadcast(new NetStartGame());
        }
    }

    private void OnMakeMoveServer(NetMessage msg, NetworkConnection cnn)
    {
        NetMakeMove netMakeMove = msg as NetMakeMove;

        // Move the piece
        //MoveTo(netMakeMove.OriginalX, netMakeMove.OriginalY, netMakeMove.X, netMakeMove.Y);

        // Broadcast the move
        Server.Instance.Broadcast(netMakeMove);

    }

    private void OnRematchServer(NetMessage msg, NetworkConnection cnn)
    {
        Server.Instance.Broadcast(msg);
    }

    //Client
    private void OnWelcomeClient(NetMessage msg)
    {
        NetWelcome netWelcome = msg as NetWelcome;

        // Assign a team
        currentTeam = netWelcome.AssignedTeam;

        Debug.Log("Assigned Team: " + currentTeam);

        if (localGame && currentTeam == 0)
        {
            Server.Instance.Broadcast(new NetStartGame());
        }

    }

    private void OnMakeMoveClient(NetMessage msg)
    {
        NetMakeMove netMakeMove = msg as NetMakeMove;

        Debug.Log("Received Move: " + netMakeMove.originalX + ", " + netMakeMove.originalY + " to " + netMakeMove.destinationX + ", " + netMakeMove.destinationY);
        if (netMakeMove.teamId != currentTeam)
        {
            Piece taget = pieces[netMakeMove.originalX, netMakeMove.originalY];

            availableMoves = taget.GetAvailableMoves(ref pieces, TILE_COUNT_X, TILE_COUNT_Y);
            //specialMove = taget.GetSpecialMove(ref pieces, ref moveList, ref availableMoves);

            // Move the piece
            MoveTo(netMakeMove.originalX, netMakeMove.originalY, netMakeMove.destinationX, netMakeMove.destinationY);
        }
    }

    private void OnRematchClient(NetMessage msg)
    {
        NetRematch netMakeMove = msg as NetRematch;

        playerRematch[netMakeMove.teamId] = netMakeMove.wantRematch == 1;

        if (netMakeMove.teamId != currentTeam)
        {
            rematchIndicator.transform.GetChild((netMakeMove.wantRematch == 1) ? 0 : 1).gameObject.SetActive(true);
            if (netMakeMove.wantRematch != 1)
            {
                rematchButton.interactable = false;
            }
        }

        if (playerRematch[0] && playerRematch[1])
        {
            GameReset();
        }
    }


    private void OnStartGameClient(NetMessage obj)
    {
        GameUI.Instance.SetCameraAngle((currentTeam == 0) ? CameraAngles.whiteTeam : CameraAngles.blackTeam);
    }

    private void OnSetLocalGame(bool obj)
    {
        playerCount = -1;
        currentTeam = -1;
        localGame = obj;
    }
    
    private void ShutdownRelay()
    {
        Client.Instance.Shutdown();
        Server.Instance.Shutdown();
    }
    #endregion
    
}
