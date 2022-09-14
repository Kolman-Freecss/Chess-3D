using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] public PieceType pieceType;
    [SerializeField] public int sideType;
    public int currentX;
    public int currentY;

    private Vector3 desiredPosition;
    private Vector3 desiredScale;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
