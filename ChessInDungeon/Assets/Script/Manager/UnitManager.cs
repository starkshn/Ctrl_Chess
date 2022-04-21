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
            {
                s_instance = FindObjectOfType<UnitManager>();
                Debug.Log("find UnitManager");
            }
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
        _controllerList.Add(new UnitInformation(controller.gameObject, controller.transform, controller));
    }

    public Transform TargetFinder(int instanceID)
    {
        return targetFinder.TargetUnitFinder(instanceID, dic);
    }

    public Vector3 NextPos(int posZ, int posX, int destZ, int destX)
    {
        var myPoints = AstarManager.Instance.FindAstar(posZ, posX, destZ, destX);

        return new Vector3(myPoints[1].X, 0, myPoints[1].Z);
    }

}