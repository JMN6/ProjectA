using UnityEngine;
using Tileinfo = TileMapDataConverter.TileInfo;

public class ConvertibleTile : MonoBehaviour
{
    [SerializeField] protected Collider2D col;
    private bool isCalculated = false;
    protected Vector2Int minIndex;
    protected Vector2Int maxIndex;

    private Vector3 lastPos; 
    private Quaternion lastRot; 
    private Vector3 lastScale;


    public virtual bool CheckIfTileisBlocked(Tileinfo info)
    {
        if(isCalculated == false)
        {
            Calculate();
        }

        Vector2Int tileIndex = TileMapDataConverter.Instance.GetNodeIndex(info.Position);
        return tileIndex.x >= minIndex.x && tileIndex.x <= maxIndex.x &&
            tileIndex.y >= minIndex.y && tileIndex.y <= maxIndex.y;
    }

    public virtual void Calculate()
    {
        TileMapDataConverter.Instance.GetObjMinAndMaxBoundIndex
            (col.bounds, out minIndex, out maxIndex);

        isCalculated = true;
    }

    public virtual void GetBoundIndex(out Vector2Int min, out Vector2Int max)
    {
        Calculate();

        min = minIndex;
        max = maxIndex;
    }

    private void LateUpdate()
    {
        if (transform.position != lastPos || 
            transform.rotation != lastRot || 
            transform.localScale != lastScale)
        {
            isCalculated = false;
            lastPos = transform.position; lastRot = transform.rotation; lastScale = transform.localScale;
        }
    }

}
