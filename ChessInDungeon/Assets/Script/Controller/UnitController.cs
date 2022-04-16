using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct Pos
{
    public Pos(int z, int x) { Z = z; X = x; }
    public int Z;
    public int X;
}

public class UnitController : MonoBehaviour
{
    int count;

    public int PosZ { get; set; }
    public int PosX { get; set; }

    public bool _onField;
    Define.State _state = Define.State.Idle;

    public bool _onAction;

    Transform _targetTransform;
    Vector3 _NextMovePos;

    Board _board;
    UnitController _unit;
    AStar _astar = new AStar();

    private Define.State Root()
    {
        _targetTransform = UnitManager.Instance.TargetFinder(gameObject.GetInstanceID() );
        Debug.Log($"{gameObject.name}Target Transform : {_targetTransform.position}");
        
        if (Vector3.Distance(transform.position, _targetTransform.position) <= 1.1f)
        {
            
            return Attack();
        }
        else
        {
            
            return Move();
        }

    }
    private Define.State Move()
    {
       
        return Define.State.Moving;
    }

    private Define.State Attack()
    {
        return Define.State.Attack;
    }

    void Start()
    {
        UnitManager.Instance.AddUnitInformation(gameObject.GetComponent<UnitController>());
        _board = new Board();
    }

    void Update()
    {
        if (UnitManager.Instance.isBattleMode)
        {
            if (!_onAction)
            {
                _state = Root();
                switch (_state)
                {
                    case Define.State.Idle:
                        {

                        }
                        break;
                    case Define.State.Moving:
                        {
                            // Astar������ �ٷ� ������ ĭ�� ���� �ϰ� ->
                            StartAstar();
                            // ������ �̵� ��Ų��.
                            // �ڷ�ƾ���� �̵���Ų��.
                            // �ڷ�ƾ �����Ҷ� _onAction = true�̴�. (�ൿ����������)
                            _onAction = true;

                            transform.position = _NextMovePos;
                            // �ڷ�ƾ ������ _onAction = false��.
                        }
                        break;
                    case Define.State.Attack:
                        {
                            Debug.Log("On Attack!");
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
    
    internal List<UnitManager.Pos> SetNextPath(List<UnitManager.Pos> points)
    {
        _NextMovePos = new Vector3(points[1].X, 0, points[1].Z); // ������ ��ġ ����
        Debug.Log($"{gameObject.name}'s nextMovePos {_NextMovePos}");
        StartCoroutine(MovePath(_NextMovePos));
        return null;
    }

    IEnumerator MovePath(Vector3 pos)
    {
        yield return new WaitForSeconds(2.0f);
        
        _onAction = false;
    }

    void StartAstar()
    {
        _board.BoardInitialize(6, (int)_targetTransform.position.z, (int)_targetTransform.position.x, this);
        UnitManager.Instance.UnitInitialize( (int)gameObject.transform.position.z, (int)gameObject.transform.position.x, _board, this);
    }
}