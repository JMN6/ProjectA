using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayFindingManager : MonoBehaviour
{
    static float WayFindingCoolTime = 0.3f;
    static int goalToleranceCells = 5;
    static int fallbackScanRadius = 20;

    public class Node
    {
        public int TotalCost => FromStart + ToEnd;
        public int FromStart { get; set; }
        public int ToEnd { get; set; }
        public Vector2Int ParentIndex { get; set; }
    }


    [SerializeField] private Collider2D col;
    private Vector2Int size;

    public Stack<Vector2Int> Route { get; private set; } = new Stack<Vector2Int>();
    private Node[,] Nodes { get; set; }
    private bool[,] isVisited;
    private Vector3Int mapSize;

    public bool IsWayFindingCoolTimeOver { get; set; } = true;
    private WaitForSeconds WFCoolTime;

    private TileMapDataConverter tileManager;

    private void Start()
    {
        tileManager = TileMapDataConverter.Instance;
        size = tileManager.GetSizeInGrid(col.bounds);

        WFCoolTime = new WaitForSeconds(WayFindingCoolTime);

        mapSize = tileManager.MapSize;
        isVisited = new bool[mapSize.x, mapSize.y];
        Nodes = new Node[mapSize.x, mapSize.y];
        for (int x = 0; x < mapSize.x; ++x)
        {
            for (int y = 0; y < mapSize.y; ++y)
            {
                Nodes[x, y] = new Node 
                { 
                    FromStart = int.MaxValue, 
                    ToEnd = int.MaxValue, 
                    ParentIndex = new Vector2Int(-1, -1) 
                };
            }
        }
    }

    private IEnumerator CoCountCoolTime()
    {
        yield return WFCoolTime;
        IsWayFindingCoolTimeOver = true;
    }

    public bool SearchWay(Vector3 startPosition, Vector3 endPosition, bool ignoreStartPosValidation = false)
    {
        if(IsWayFindingCoolTimeOver == false)
        {
            return false;
        }

        ResetRoute();

        if ((ignoreStartPosValidation && tileManager.IsValidPosition(startPosition) == false) ||
            tileManager.IsValidPosition(endPosition) == false)
            return false;

        // 길찾기 쿨타임
        IsWayFindingCoolTimeOver = false;
        StartCoroutine(CoCountCoolTime());

        Vector2Int start = tileManager.GetNodeIndex(startPosition);
        Vector2Int end = tileManager.GetNodeIndex(endPosition);

        if(tileManager.IsValidPosition(endPosition))
        {
            Vector2Int alt = FindClosestStandable(end, fallbackScanRadius);
            if (alt.x == -1) return false;
            end = alt;
        }

        List<Vector2Int> openList = new List<Vector2Int>();
        List<Vector2Int> closedList = new List<Vector2Int>();

        Nodes[start.x, start.y].FromStart = 0;
        Nodes[start.x, start.y].ToEnd = (Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y)) * 10;
        openList.Add(start);

        // 후보 추적
        Vector2Int bestSoFar = start;
        int bestH = Nodes[start.x, start.y].ToEnd;

        // 목표 허용 비용
        int tolCost = goalToleranceCells * 10;
        bool reached = false;
        Vector2Int reachedIndex = start;

        while (openList.Count > 0)
        {
            Vector2Int curIndex = openList[^1];
            Node curNode = Nodes[curIndex.x, curIndex.y];

            openList.RemoveAt(openList.Count - 1);

            if (isVisited[curIndex.x, curIndex.y] == true)
                continue;
            isVisited[curIndex.x, curIndex.y] = true;
            
            closedList.Add(curIndex);

            // 후보 갱신
            if(curNode.ToEnd < bestH)
            {
                bestH = curNode.ToEnd;
                bestSoFar = curIndex;
            }

            if(curIndex == end || curNode.ToEnd <= tolCost)
            {
                reached = true;
                reachedIndex = curIndex;
                break;
            }

            // left
            AddNodeToOpenList(new Vector2Int(curIndex.x, curIndex.y - 1), curIndex, 10, in end, ref openList);

            // right
            AddNodeToOpenList(new Vector2Int(curIndex.x, curIndex.y + 1), curIndex, 10, in end, ref openList);

            // bottom
            AddNodeToOpenList(new Vector2Int(curIndex.x - 1, curIndex.y), curIndex, 8, in end, ref openList);

            openList.Sort(cmp);
        }

        Vector2Int goalIndex = reached ? reachedIndex : bestSoFar;

        if (closedList.Count == 0)
            return false;

        Vector2Int next = goalIndex;
        Route.Push(next);
        while (next != start)
        {
            Vector2Int parent = Nodes[next.x, next.y].ParentIndex;
            if (parent.x < 0) return false; // 부모가 없으면 복원 실패
            next = parent;
            Route.Push(next);
        }
        return true;
    }

    private bool AddNodeToOpenList(
        Vector2Int newNode, in Vector2Int parentNode, 
        int stepWeight, in Vector2Int endNode,
        ref List<Vector2Int> openList)
    {
        // 맵 범위를 넘었는지
        if (TileMapDataConverter.Instance.IsIndexOutOfRange(newNode) == true)
            return false;

        // 대상이 서있을 수 없는지
        if (TileMapDataConverter.Instance.CheckIfObjCanStand(size, newNode) == false)
            return false;

        int fromStart = Nodes[parentNode.x, parentNode.y].FromStart + stepWeight;
        int toEnd = (Mathf.Abs(endNode.x - newNode.x) + Mathf.Abs(endNode.y - newNode.y)) * 10;

        if (!isVisited[newNode.x,newNode.y])
        {
            isVisited[newNode.x, newNode.y] = true;

            Node addNode = Nodes[newNode.x, newNode.y];
            addNode.FromStart = fromStart;
            addNode.ToEnd = toEnd;
            addNode.ParentIndex = parentNode;

            openList.Add(newNode);

            return true;
        }

        Node addedNode = Nodes[newNode.x, newNode.y];
        if(addedNode.FromStart > fromStart)
        {
            addedNode.FromStart = fromStart;
            addedNode.ParentIndex = parentNode;
        }

        return true;
    }

    private Vector2Int FindClosestStandable(Vector2Int target, int maxR)
    {
        for (int r = 0; r <= maxR; r++)
        {
            for (int dy = -r; dy <= r; dy++)
            {
                int dx = r - Mathf.Abs(dy);

                foreach (int sx in new int[] { -dx, dx })
                {
                    Vector2Int cand = new Vector2Int(target.x + sx, target.y + dy);
                    if (tileManager.IsIndexOutOfRange(cand)) continue;
                    if (tileManager.CheckIfObjCanStand(size, cand)) return cand;
                }
            }
        }
        return new Vector2Int(-1, -1);
    }


    private int cmp(Vector2Int a, Vector2Int b)
    {
        int aCost = Nodes[a.x, a.y].TotalCost;
        int bCost = Nodes[b.x, b.y].TotalCost;

        if (aCost < bCost) return 1;
        else if (aCost > bCost) return -1;
        else return 0;
    }

    private void ResetRoute()
    {
        Route.Clear();

        for(int i = 0; i< mapSize.x; ++i)
        {
            for(int j = 0; j< mapSize.y; ++j)
            {
                isVisited[i, j] = false;
            }
        }
    }
}
