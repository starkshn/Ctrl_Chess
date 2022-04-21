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
    
    struct PQNode : IComparable<PQNode>   // priorityQueue�� �� ���
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
        // �����Ͽ� �����ϱ� ���� �迭
        // U L D R UL DL DR UR
        int[] deltaZ = new int[] { -1, 0, 1, 0, -1, 1, 1, -1 };
        int[] deltaX = new int[] { 0, -1, 0, 1, -1, -1, 1, 1 };
        int[] cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 }; // U L D R UL DL DR UR�� ���� ���

        // (x, z) �̹� �湮�ߴ��� ����
        bool[,] closed = new bool[Board.Instance.Size, Board.Instance.Size];

        // (x, z) ���� ���� �� ���̶� �߰��ߴ��� ����
        // �߰� �������� MaxValue
        // �߰������� F = G + H
        int[,] open = new int[Board.Instance.Size, Board.Instance.Size];

        for (int z = 0; z < Board.Instance.Size; z++)
            for (int x = 0; x < Board.Instance.Size; x++)
                open[z, x] = Int32.MaxValue;

        Pos[,] parent = new Pos[Board.Instance.Size, Board.Instance.Size];

        // ���¸���Ʈ�� �ִ� ������ �߿���, ���� ���� �ĺ��� ������ �̾ƿ��� ���� ����
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        // ������ �߰� (���� ����)
        open[posZ, posX] = 10 * (Math.Abs(destZ - posZ) + Math.Abs(destX - posX));
        pq.Push(new PQNode() { F = 10 * (Math.Abs(destZ - posZ) + Math.Abs(destX - posX)), G = 0, Z = posZ, X = posX });
        parent[posZ, posX] = new Pos(posZ, posX);

        while (pq.Count > 0)
        {
            // ���� ���� �ĺ��� ã�´�.
            PQNode node = pq.Pop();

            // �湮�� ���̶�� ��ŵ
            if (closed[node.Z, node.X])
                continue;

            // �湮�Ѵ�.
            closed[node.Z, node.X] = true;

            // ������ ���������� �ٷ�����
            if (node.Z == destZ && node.X == destX)
                break;

            // TileType�� ������ ��� AND ���� ��ǥ�� ���� ���� ��� => ��, �� Ÿ���� InUnit�ε� ���� InUnit�� �ƴ� ���
            if (Board.Instance.Tile[node.Z, node.X] == Define.TileType.InUnit && node.Z != posZ && node.X != posX)
                continue;

            // �����¿� �� �̵��� �� �ִ� ��ǥ���� Ȯ���ؼ� ����(open)�Ѵ�.
            for (int i = 0; i < deltaZ.Length; i++)
            {
                int nextZ = node.Z + deltaZ[i];
                int nextX = node.X + deltaX[i];

                // ��ȿ������ ������� ��ŵ
                if (nextX < 0 || nextX >= Board.Instance.Size || nextZ < 0 || nextZ >= Board.Instance.Size)
                    continue;

                // ������ ������ �� �� ������ ��ŵ
                if (Board.Instance.Tile[nextZ, nextX] == Define.TileType.Wall)
                    continue;

                // �̹� �湮�� ���̶�� ��ŵ
                if (closed[nextZ, nextX])
                    continue;

                // �����
                int g = node.G + cost[i];
                int h = 10 * (Math.Abs(destZ - nextZ) + Math.Abs(destX - nextX));

                // �׷��� �ٸ� ��ο��� �� ������ �̹� ã������ ��ŵ�Ѵ�.
                if (open[nextZ, nextX] < g + h)
                    continue;

                // ���� ����
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
