using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] public PieceType pieceType;
    [SerializeField] public int sideType;
    public int currentX;
    public int currentY;

    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;

    private void Update() 
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
        {
            transform.position = desiredPosition;
        }
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
        {
            transform.localScale = desiredScale;
        }
    }
}
