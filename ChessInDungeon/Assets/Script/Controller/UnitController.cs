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
    Animator anim;
    public int PosZ { get; set; }
    public int PosX { get; set; }

    public int DestZ { get;  set; }
    public int DestX { get;  set; }

    public Vector3 _nextMovePos;
    public float _moveSpeed = 2.0f;

    public bool _onField;
    public bool _onAction;

    Define.State _state = Define.State.Idle;

    Transform _targetTransform;
    Vector3 _nextUnitMovePos;

    UnitAction _action;
    
    private Define.State Root()
    {
        _targetTransform = UnitManager.Instance.TargetFinder(gameObject.GetInstanceID());

        if (Vector3.Distance(transform.position, _targetTransform.position) <= 1.45f)
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
        Animator anim = GetComponent<Animator>();
        _action = GetComponent<UnitAction>();
    }

    void Update()
    {
        if (UnitManager.Instance.isBattleMode)
        {
            if (!_action._onAction)
            {
                Debug.Log("_onAction true!");
                _state = Root();
                switch (_state)
                {
                    case Define.State.Idle:
                        {

                        }
                        break;
                    case Define.State.Moving:
                        {
                            Debug.Log("Start Moving!");
                         
                            MoveAction();
                            
                        }
                        break;
                    case Define.State.Attack:
                        {
                            Debug.Log($"{this.gameObject.name}On Attack!");
                           
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void MoveAction()
    {
        
        PosZ = (int)this.gameObject.transform.position.z;
        PosX = (int)this.gameObject.transform.position.x;
        DestZ = (int)_targetTransform.position.z;
        DestX = (int)_targetTransform.position.x;

        Board.Instance.BoardInitialize(6, DestZ, DestX, this);
        
        _nextUnitMovePos = UnitManager.Instance.NextPos(PosZ, PosX, DestZ, DestX);

        Board.Instance.Tile[(int)_nextUnitMovePos.z, (int)_nextUnitMovePos.x] = Define.TileType.InUnit;
        Board.Instance.Tile[PosZ, PosX] = Define.TileType.Empty;

        _action.Move(_nextUnitMovePos);
       
    }
    //void StartAstar()
    //{

    //    PosZ = (int)this.gameObject.transform.position.z;
    //    PosX = (int)this.gameObject.transform.position.x;
    //    DestZ = (int)_targetTransform.position.z;
    //    DestX = (int)_targetTransform.position.x;

    //    //Board.Instance.BoardInitialize(6, DestZ, DestX, this);
    //    UnitManager.Instance.UnitInitialize( PosZ, PosX, DestZ, DestX);
    //}
    //internal List<UnitManager.Pos> SetNextPath(List<UnitManager.Pos> points)
    //{
    //    myPoints = points;

    //    _nextMovePos = new Vector3(myPoints[1].X, 0, myPoints[1].Z);
    //    Debug.Log($"{this.gameObject.name}의 다음 벡터 : {_nextMovePos}");

    //    Board.Instance.Tile[myPoints[1].Z, myPoints[1].X] = Define.TileType.InUnit;
    //    Board.Instance.Tile[myPoints[0].Z, myPoints[0].X] = Define.TileType.Empty;

    //    for (int i = 0; i < myPoints.Count; i++)
    //        Debug.Log($"{this.gameObject.name}의 {myPoints[i].Z} {myPoints[i].X} ");

    //    _nextUnitMovePos = new Vector3(myPoints[1].X, 0, myPoints[1].Z);

    //    transform.position = _nextUnitMovePos;

    //    return null;
    //}
}