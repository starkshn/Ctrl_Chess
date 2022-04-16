using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    Board _board;
    public int PosZ { get; private set; }
    public int PosX { get; private set; }

    int _dir = (int)Define.Dir.Up;
    public  List<Pos> _points = new List<Pos>();

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
    public void UnitInitialize(int posZ, int posX, Board board)
    {
        PosZ = posZ;
        PosX = posX;
        _board = board;

        FindAstar();
    }

    void FindAstar()
    {
        //Debug.Log("Start Astar!");
        // �����Ͽ� �����ϱ� ���� �迭
        // U L D R UL DL DR UR
        int[] deltaZ = new int[] { -1, 0, 1, 0, -1, 1, 1, -1 };
        int[] deltaX = new int[] { 0, -1, 0, 1, -1, -1, 1, 1 };
        int[] cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 }; // U L D R UL DL DR UR�� ���� ���

        // (x, z) �̹� �湮�ߴ��� ����
        bool[,] closed = new bool[_board.Size, _board.Size];

        // (x, z) ���� ���� �� ���̶� �߰��ߴ��� ����
        // �߰� �������� MaxValue
        // �߰������� F = G + H
        int[,] open = new int[_board.Size, _board.Size];
        for (int z = 0; z < _board.Size; z++)
            for (int x = 0; x < _board.Size; x++)
                open[z, x] = Int32.MaxValue;

        Pos[,] parent = new Pos[_board.Size, _board.Size];

        // ���¸���Ʈ�� �ִ� ������ �߿���, ���� ���� �ĺ��� ������ �̾ƿ��� ���� ����
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        // ������ �߰� (���� ����)
        open[PosZ, PosX] = 10 * (Math.Abs(_board.DestZ - PosZ) + Math.Abs(_board.DestX - PosX)); // ���� targetPos => z, x = (5, 3)
        pq.Push(new PQNode() { F = 10 * (Math.Abs(_board.DestZ - PosZ) + Math.Abs(_board.DestX - PosX)), G = 0, Z = PosZ, X = PosX });
        parent[PosZ, PosX] = new Pos(PosZ, PosX);


        while (pq.Count > 0)
        {
            //Debug.Log("Start To Find ���������ĺ�!");
            // ���� ���� �ĺ��� ã�´�.
            PQNode node = pq.Pop();
            //Debug.Log($"node PosZ : {node.Z} node PosX : {node.X}");

            // �湮�� ���̶�� ��ŵ
            if (closed[node.Z, node.X])
                continue;

            // �湮�Ѵ�.
            closed[node.Z, node.X] = true;
            // ������ ���������� �ٷ�����
            if (node.Z == _board.DestZ && node.X == _board.DestX)
                break;

            // �����¿� �� �̵��� �� �ִ� ��ǥ���� Ȯ���ؼ� ����(open)�Ѵ�.
            for (int i = 0; i < deltaZ.Length; i++)
            {
                int nextZ = node.Z + deltaZ[i];
                int nextX = node.X + deltaX[i];

                // ��ȿ������ ������� ��ŵ
                if (nextX < 0 || nextX >= _board.Size || nextZ < 0 || nextZ >= _board.Size)
                    continue;
                // ������ ������ �� �� ������ ��ŵ
                if (_board.Tile[nextZ, nextX] == Define.TileType.Wall)
                    continue;
                // �̹� �湮�� ���̶�� ��ŵ
                if (closed[nextZ, nextX])
                    continue;

                // �����
                int g = node.G + cost[i];
                int h = 10 * (Math.Abs(_board.DestZ - nextZ) + Math.Abs(_board.DestX - nextX));

                // �׷��� �ٸ� ��ο��� �� ������ �̹� ã������ ��ŵ�Ѵ�.
                if (open[nextZ, nextX] < g + h)
                    continue;

                // ���� ����
                open[nextZ, nextX] = g + h;
                pq.Push(new PQNode() { F = g + h, G = g, Z = nextZ, X = nextX });
                parent[nextZ, nextX] = new Pos(node.Z, node.X);
                //_tryToFindAstar = false;
                //Debug.Log($"for�����鼭 Ž���Ѵ�! next PosZ : {nextZ} next PosX : {nextX}");
            }

        }
        //Debug.Log("End Astar!");
        CalcPathFromParent(parent);
    }

    void CalcPathFromParent(Pos[,] parent)
    {
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
    }
}
