using UnityEngine;

public class MovingTile : ConvertibleTile
{
    [SerializeField] private Transform minPos;
    [SerializeField] private Transform maxPos;

    public override void GetBoundIndex(out Vector2Int min, out Vector2Int max)
    {
        var b = col.bounds;

        Vector3 centerOffset = b.center - transform.position;

        Bounds a = new Bounds(minPos.position + centerOffset, b.size);
        Bounds c = new Bounds(maxPos.position + centerOffset, b.size);

        Bounds u = a;
        u.Encapsulate(c);

        const float EPS = 1e-4f;
        u.Expand(new Vector3(EPS, EPS, 0f));

        TileMapDataConverter.Instance.GetObjMinAndMaxBoundIndex(u, out min, out max);
    }
}
