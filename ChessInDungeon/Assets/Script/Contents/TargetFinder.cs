using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    public Transform TargetUnitFinder(int instanceID, Dictionary<int, UnitManager.UnitInformation> dic)
    {
        float distance = 9999.0f;
        int targetInstanceID = 0;

        foreach (KeyValuePair<int, UnitManager.UnitInformation> testDic in dic)
        {
            if (testDic.Key != instanceID)
            {
                if (testDic.Value.unit.tag != dic[instanceID].unit.tag)
                {
                    if (distance > Mathf.Abs(testDic.Value.transform.position.x - dic[instanceID].transform.position.x) + Mathf.Abs(testDic.Value.transform.position.z - dic[instanceID].transform.position.z))
                    {
                        distance = Mathf.Abs(testDic.Value.transform.position.x - dic[instanceID].transform.position.x) + Mathf.Abs(testDic.Value.transform.position.z - dic[instanceID].transform.position.z);
                        targetInstanceID = testDic.Key;
                    }
                }
            }
        }
        return dic[targetInstanceID].transform;
    }
}
