using UnityEngine;

public class OnOffTile : ConvertibleTile
{
    public override void Calculate()
    {
        if(col.enabled == false || gameObject.activeSelf == false)
        {
            minIndex = maxIndex = Vector2Int.zero;
        }
        else
        {
            base.Calculate();
        }
    }

    public override void GetBoundIndex(out Vector2Int min, out Vector2Int max)
    {
        base.Calculate();

        min = minIndex;
        max = maxIndex;
    }
}
