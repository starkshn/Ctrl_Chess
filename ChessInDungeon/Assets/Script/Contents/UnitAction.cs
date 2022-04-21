using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAction : MonoBehaviour
{

    Vector3 _destPos;
    float _moveSpeed = 1.0f;
    public bool _onAction;

    //public IEnumerator _action;

    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void Move(Vector3 destPos)
    {
        _destPos = destPos;
        StartCoroutine( MoveAction() );
        
    }

    //public void Attack()
    //{
    //    if (_action != null)
    //    {
    //        StopCoroutine(_action);

    //        _action = null;
    //    }

    //    _action = AttackAction();
    //    StartCoroutine ( _action );
    //}
    //IEnumerator AttackAction()
    //{

    //    yield return new WaitForSeconds( _moveSpeed );
    //}

    IEnumerator MoveAction()
    {
        _onAction = true;

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destPos, _moveSpeed * Time.deltaTime);

            yield return null;

            if (transform.position == _destPos)
            {
                yield return new WaitForSeconds(0.5f);
                break;
            }
                
        }

        _onAction = false;
    }
}
