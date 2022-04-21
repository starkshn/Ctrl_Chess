using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarManager : MonoBehaviour
{
    static AstarManager s_instance;
    public static AstarManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<AstarManager>();
                Debug.Log("find UnitManager");
            }
            return s_instance;
        }
    }
    
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

    public List<Pos> FindAstar(int posZ, int posX, int destZ, int destX)
    {
        // 상좌하우 예약하기 위한 배열
        // U L D R UL DL DR UR
        int[] deltaZ = new int[] { -1, 0, 1, 0, -1, 1, 1, -1 };
        int[] deltaX = new int[] { 0, -1, 0, 1, -1, -1, 1, 1 };
        int[] cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 }; // U L D R UL DL DR UR로 가는 비용

        // (x, z) 이미 방문했는지 여부
        bool[,] closed = new bool[Board.Instance.Size, Board.Instance.Size];

        // (x, z) 가는 길을 한 번이라도 발견했는지 여부
        // 발견 못했으면 MaxValue
        // 발견했으면 F = G + H
        int[,] open = new int[Board.Instance.Size, Board.Instance.Size];

        for (int z = 0; z < Board.Instance.Size; z++)
            for (int x = 0; x < Board.Instance.Size; x++)
                open[z, x] = Int32.MaxValue;

        Pos[,] parent = new Pos[Board.Instance.Size, Board.Instance.Size];

        // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        // 시작점 발견 (예약 진행)
        open[posZ, posX] = 10 * (Math.Abs(destZ - posZ) + Math.Abs(destX - posX));
        pq.Push(new PQNode() { F = 10 * (Math.Abs(destZ - posZ) + Math.Abs(destX - posX)), G = 0, Z = posZ, X = posX });
        parent[posZ, posX] = new Pos(posZ, posX);

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
            if (node.Z == destZ && node.X == destX)
                break;

            // TileType이 유닛일 경우 AND 나의 좌표와 같지 않을 경우 => 즉, 내 타일은 InUnit인데 나의 InUnit이 아닐 경우
            if (Board.Instance.Tile[node.Z, node.X] == Define.TileType.InUnit && node.Z != posZ && node.X != posX)
                continue;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다.
            for (int i = 0; i < deltaZ.Length; i++)
            {
                int nextZ = node.Z + deltaZ[i];
                int nextX = node.X + deltaX[i];

                // 유효범위를 벗어났으면 스킵
                if (nextX < 0 || nextX >= Board.Instance.Size || nextZ < 0 || nextZ >= Board.Instance.Size)
                    continue;

                // 벽으로 막혀서 갈 수 없으면 스킵
                if (Board.Instance.Tile[nextZ, nextX] == Define.TileType.Wall)
                    continue;

                // 이미 방문한 곳이라면 스킵
                if (closed[nextZ, nextX])
                    continue;

                // 비용계산
                int g = node.G + cost[i];
                int h = 10 * (Math.Abs(destZ - nextZ) + Math.Abs(destX - nextX));

                // 그런데 다른 경로에서 더 빠른길 이미 찾았으면 스킵한다.
                if (open[nextZ, nextX] < g + h)
                    continue;

                // 예약 진행
                open[nextZ, nextX] = g + h;
                pq.Push(new PQNode() { F = g + h, G = g, Z = nextZ, X = nextX });
                parent[nextZ, nextX] = new Pos(node.Z, node.X);

            }
        }

        return CalcPathFromParent(parent, destZ, destX);
    }
    List<Pos> CalcPathFromParent(Pos[,] parent, int destZ, int destX)
    {
        List<Pos> _points = new List<Pos>();
        _points.Clear();

        int z = destZ;
        int x = destX;

        while (parent[z, x].Z != z || parent[z, x].X != x)
        {
            _points.Add(new Pos(z, x));
            Pos pos = parent[z, x];

            z = pos.Z;
            x = pos.X;
        }
        _points.Add(new Pos(z, x));
        _points.Reverse();

        //return _unit.SetNextPath(_points);
        return _points;
    }
}
