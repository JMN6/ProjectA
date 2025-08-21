using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapDataConverter : MonoSingleton<TileMapDataConverter>
{
    public class TileInfo
    {
        public bool IsBlocked { get; set; }
        public bool IsConvertible { get; set; }
        public Vector2Int Index { get; set; }
        public Vector3 Position { get; set; }
        public ConvertibleTile Obj { get; set; }

        public bool CheckIfTileIsBlocked()
        {
            if (IsBlocked == true)
                return true;

            if (IsConvertible == false)
                return false;

            return Obj.CheckIfTileisBlocked(this);
        }
    }

    [Header("MapBound")]
    [SerializeField] private Transform mapMin;
    [SerializeField] private Transform mapMax;

    [Header("TileMaps")]
    [field: SerializeField] public TileInfo[,] NodeMap { get; private set; }
    [SerializeField] private Grid grid;

    [Header("ExtraMapObjects")]
    [SerializeField] private List<Tilemap> tilemaps;
    [SerializeField] private List<Collider2D> staticObjects;
    [SerializeField] private List<ConvertibleTile> convertibleObjects;

    private bool isMapSet = false;

    private Tilemap defaultMap;
    private Vector3 tileOffset;
    public Vector3Int MapSize { get; private set; }

    private void Awake()
    {
        ResetMap();
    }

    public void ResetMap()
    {
        if (isMapSet == true)
            return;

        if (tilemaps.Count == 0 || mapMin == null || mapMax == null || grid == null)
        {
            Debug.LogError("Map Settings Cannot be done");
            return;
        }

        isMapSet = true;

        MapSize = new Vector3Int(
            Mathf.CeilToInt((mapMax.position.y - mapMin.position.y) / grid.cellSize.y),
            Mathf.CeilToInt((mapMax.position.x - mapMin.position.x) / grid.cellSize.x),
            0);

        tileOffset = grid.cellSize;

        NodeMap = new TileInfo[MapSize.x, MapSize.y];
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                NodeMap[x, y] = new TileInfo
                {
                    Index = new Vector2Int(x, y),
                    Position = GetTilePosition(new Vector2Int(x, y)),
                    IsBlocked = false,
                    IsConvertible = false,
                    Obj = null
                };
            }
        }

        defaultMap = tilemaps[0];

        foreach (var tilemap in tilemaps)
        {
            AddTileMapObjToMap(tilemap);
        }

        foreach (var obj in staticObjects)
        {
            AddStaticObjToMap(obj);
        }

        foreach (var obj in convertibleObjects)
        {
            AddConvertibleObjToMap(obj);
        }

        // debug
        var p = new GameObject("map");
        string s = string.Empty;
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                if (NodeMap[i, j].IsBlocked == true ||
                    NodeMap[i, j].IsConvertible == true)
                {
                    var o = new GameObject($"{i}, {j}");
                    o.transform.parent = p.transform;
                    o.transform.position = NodeMap[i, j].Position;
                }
            }
        }
    }

    private void AddTileMapObjToMap(Tilemap map)
    {
        for (int i = 0; i < MapSize.x; i++)
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                Vector3 center = GetTilePosition(new Vector2Int(i, j));
                var cell = map.WorldToCell(center);
                if(map.HasTile(cell) == false)
                {
                    continue;
                }

                NodeMap[i, j].IsBlocked = true;
                NodeMap[i, j].IsConvertible = false;
            }
        }
    }

    private void AddStaticObjToMap(Collider2D obj)
    {
        Vector2Int min, max;
        GetObjMinAndMaxBoundIndex(obj.bounds, out min, out max);

        AddObjToMap(min, max, true, false);
    }

    private void AddConvertibleObjToMap(ConvertibleTile tile)
    {
        Vector2Int min, max;
        tile.GetBoundIndex(out min, out max);

        AddObjToMap(min, max, false, true, tile);
    }

    private void AddObjToMap(Vector2Int min, Vector2Int max, bool isBlocked, bool isConvertible, ConvertibleTile obj = null)
    {
        for (int i = min.x; i <= max.x; ++i)
        {
            for (int j = min.y; j <= max.y; ++j)
            {
                NodeMap[i, j].IsBlocked = isBlocked;
                NodeMap[i, j].IsConvertible = isConvertible;
                NodeMap[i, j].Obj = obj;
            }
        }
    }

    public void GetObjMinAndMaxBoundIndex(Bounds bound, out Vector2Int min, out Vector2Int max)
    {
        if (isMapSet == false)
        {
            min = max = Vector2Int.one * -1;
            return;
        }

        min = GetNodeIndex(bound.min);
        max = GetNodeIndex(bound.max);
    }

    public bool IsThereTile(Vector3 position)
    {
        foreach (var tilemap in tilemaps)
        {
            // 타일이 있는 곳이라면 불가능
            if (tilemap.HasTile(tilemap.WorldToCell(position)) == true)
                return true;
        }

        return false;
    }

    public bool IsValidPosition(Vector3 position)
    {
        if (isMapSet == false)
            return false;

        if (position.x > mapMax.position.x ||
            position.x < mapMin.position.x)
            return false;

        if (position.y > mapMax.position.y ||
            position.y < mapMin.position.y)
            return false;

        Vector2Int index = GetNodeIndex(position);
        if (NodeMap[index.x, index.y].CheckIfTileIsBlocked() == true)
            return false;

        return true;
    }

    public Vector2Int GetNodeIndex(Vector3 position)
    {
        if (isMapSet == false)
        {
            return new Vector2Int(-1, -1);
        }

        return new Vector2Int(
            Mathf.Clamp(Mathf.FloorToInt((position.y - mapMin.position.y) / tileOffset.y), 0, MapSize.x - 1),
            Mathf.Clamp(Mathf.FloorToInt((position.x - mapMin.position.x) / tileOffset.x), 0, MapSize.y - 1));
    }

    public Vector3 GetTilePosition(Vector2Int index)
    {
        if (isMapSet == false)
        {
            return Vector3.zero;
        }

        Vector3 position = mapMin.position + tileOffset * 0.5f;
        position.y += tileOffset.y * index.x;
        position.x += tileOffset.x * index.y;

        return position;
    }

    public int GetFloorTileYValue(Vector2Int index)
    {
        if (isMapSet == false)
            return -1;

        for (int i = index.x; i >= 0; --i)
        {
            if (NodeMap[i, index.y].CheckIfTileIsBlocked() == false)
                return i;
        }

        return -1;
    }

    public bool CheckIfObjCanStand(Vector2Int size, Vector2Int bottomCenterIndex)
    {
        int height = Mathf.Max(0, size.x);
        int width = Mathf.Max(0, size.y);
        int halfW = (width - 1) / 2;

        Vector2Int min = new Vector2Int(bottomCenterIndex.x, bottomCenterIndex.y - halfW);
        Vector2Int max = new Vector2Int(bottomCenterIndex.x + height - 1, bottomCenterIndex.y + halfW);


        if (IsIndexOutOfRange(min) || IsIndexOutOfRange(max))
        {
            return false;
        }

        // 바닥이 있는지 확인
        if (min.x - 1 < 0)
            return false;

        bool isThereFloor = false;
        for (int i = min.y; i <= max.y; ++i)
        {
            if (NodeMap[min.x - 1, i].CheckIfTileIsBlocked() == true)
            {
                isThereFloor = true;
                break;
            }
        }
        if (isThereFloor == false)
            return false;

        // 지금 있는 공간이 충분히 서있을 수 있는 공간인지
        for (int i = min.x; i <= max.x; ++i)
        {
            for (int j = min.y; j <= max.y; ++j)
            {
                if (NodeMap[i, j].CheckIfTileIsBlocked() == true)
                    return false;
            }
        }

        return true;
    }

    public Vector2Int GetSizeInGrid(Bounds bound)
    {
        if (isMapSet == false)
            return Vector2Int.zero;

        return new Vector2Int(
            Mathf.CeilToInt(Mathf.Abs(bound.max.y - bound.min.y) / tileOffset.y), 
            Mathf.CeilToInt(Mathf.Abs(bound.max.x - bound.min.x) / tileOffset.x));
    }

    public TileInfo GetTileInfo(Vector2Int index)
    {

        if (IsIndexOutOfRange(index))
        {
            return null;
        }

        return NodeMap[index.x, index.y];
    }

    public bool IsIndexOutOfRange(Vector2Int index) =>
            isMapSet == false ||
            index.x < 0 || index.x >= MapSize.x ||
            index.y < 0 || index.y >= MapSize.y;
}
