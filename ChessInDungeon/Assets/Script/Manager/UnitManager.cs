using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitManager : MonoBehaviour
{
    public struct UnitInformation
    {
        public GameObject unit;
        public Transform transform;
        public UnitController unitController;

        public UnitInformation(GameObject go, Transform tr, UnitController controller) { unit = go; transform = tr; unitController = controller; }
    }

    static UnitManager s_instance;
    public static UnitManager Instance 
    { 
        get 
        { 
            if (s_instance == null) 
                s_instance = FindObjectOfType<UnitManager>();
            return s_instance; 
        }
    }

    List<UnitInformation> _controllerList = new List<UnitInformation>();

    private bool _isBattleMode;
    public bool isBattleMode { get => _isBattleMode; }

    private Dictionary<int, UnitInformation> dic = new Dictionary<int, UnitInformation>();

    private TargetFinder targetFinder;

    void Update()
    {
        if (!isBattleMode)
        {
            BattleStart();
        }
    }
    void Start()
    {
        targetFinder = GetComponent<TargetFinder>();
    }

    public void BattleStart()
    {
        for (int i = 0; i < _controllerList.Count; i++)
        {
            if (_controllerList[i].unitController._onField)
            {
                dic.Add(_controllerList[i].unit.GetInstanceID(), _controllerList[i]);
            }
        }
        _isBattleMode = true;
    }

    public void AddUnitInformation(UnitController controller)
    {
        _controllerList.Add(new UnitInformation(controller.gameObject, controller.transform, controller) );
    }

    public Transform TargetFinder(int instanceID)
    {
        return targetFinder.TargetUnitFinder(instanceID, dic);
    }

    #region Astar()

    Board _board;
    UnitController _unit;

    int _dir = (int)Define.Dir.Up;

    struct PQNode : IComparable<PQNode>   // priorityQueue에 들어갈 노드
    {
        public int F;
        public int G;
        public int Z;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
                return 0;
            return F < other.F ? 1 : -1;
        }
    }
    public struct Pos
    {
        public Pos(int z, int x) { Z = z; X = x; }
        public int Z;
        public int X;
    }
    public void UnitInitialize(int posZ, int posX, Board board, UnitController unit)
    {
        _unit = unit;

        _board = board;

        _unit.PosZ = 0;
        _unit.PosX = 0;
        _unit.PosZ = posZ;
        _unit.PosX = posX;

        //Debug.Log($"UnitInitialize : {_unit.PosZ} {_unit.PosX} {_unit.gameObject.name}");
        //Debug.Log($"UnitInitialize : {_board.DestZ} {_board.DestX}");
       
        FindAstar(_unit);
    }
   
    void FindAstar(UnitController unit)
    {
        // 상좌하우 예약하기 위한 배열
        // U L D R UL DL DR UR
        int[] deltaZ = new int[] { -1, 0, 1, 0, -1, 1, 1, -1 };
        int[] deltaX = new int[] { 0, -1, 0, 1, -1, -1, 1, 1 };
        int[] cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 }; // U L D R UL DL DR UR로 가는 비용

        // (x, z) 이미 방문했는지 여부
        bool[,] closed = new bool[_board.Size, _board.Size];

        // (x, z) 가는 길을 한 번이라도 발견했는지 여부
        // 발견 못했으면 MaxValue
        // 발견했으면 F = G + H
        int[,] open = new int[_board.Size, _board.Size];

        for (int z = 0; z < _board.Size; z++)
            for (int x = 0; x < _board.Size; x++)
                open[z, x] = Int32.MaxValue;

        Pos[,] parent = new Pos[_board.Size, _board.Size];

        // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        // 시작점 발견 (예약 진행)
        open[_unit.PosZ, _unit.PosX] = 10 * (Math.Abs(_board.DestZ - _unit.PosZ) + Math.Abs(_board.DestX - _unit.PosX));
        pq.Push(new PQNode() { F = 10 * (Math.Abs(_board.DestZ - _unit.PosZ) + Math.Abs(_board.DestX - _unit.PosX)), G = 0, Z = _unit.PosZ, X = _unit.PosX });
        parent[_unit.PosZ, _unit.PosX] = new Pos(_unit.PosZ, _unit.PosX);

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다.
            PQNode node = pq.Pop();
           
            // 방문한 곳이라면 스킵
            if (closed[node.Z, node.X])
                continue;

            // 방문한다.
            closed[node.Z, node.X] = true;
            // 목적지 도착했으면 바로종료
            if (node.Z == _board.DestZ && node.X == _board.DestX)
                break;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다.
            for (int i = 0; i < deltaZ.Length; i++)
            {
                int nextZ = node.Z + deltaZ[i];
                int nextX = node.X + deltaX[i];

                // 유효범위를 벗어났으면 스킵
                if (nextX < 0 || nextX >= _board.Size || nextZ < 0 || nextZ >= _board.Size)
                    continue;

                // 벽으로 막혀서 갈 수 없으면 스킵
                if (_board.Tile[nextZ, nextX] == Define.TileType.Wall)
                {
                    Debug.Log($"{_board.Tile[nextZ, nextX]}는 가로막힘!");
                    continue;
                }
                
                // 이미 방문한 곳이라면 스킵
                if (closed[nextZ, nextX])
                    continue;

                // 비용계산
                int g = node.G + cost[i];
                int h = 10 * (Math.Abs(_board.DestZ - nextZ) + Math.Abs(_board.DestX - nextX));

                // 그런데 다른 경로에서 더 빠른길 이미 찾았으면 스킵한다.
                if (open[nextZ, nextX] < g + h)
                    continue;

                // 예약 진행
                open[nextZ, nextX] = g + h;
                pq.Push(new PQNode() { F = g + h, G = g, Z = nextZ, X = nextX });
                parent[nextZ, nextX] = new Pos(node.Z, node.X);
                
            }
        }

        CalcPathFromParent(parent);
    }
    List<Pos> CalcPathFromParent(Pos[,] parent)
    {
        List<Pos> _points = new List<Pos>();
        _points.Clear();
        
        int z = _board.DestZ;
        int x = _board.DestX;
        while (parent[z, x].Z != z || parent[z, x].X != x)
        {
            _points.Add(new Pos(z, x));
            Pos pos = parent[z, x];
            
            z = pos.Z;
            x = pos.X;
            
        }
        _points.Add(new Pos(z, x));    
        _points.Reverse();
        
        return _unit.SetNextPath(_points);
    }

    #endregion
}
