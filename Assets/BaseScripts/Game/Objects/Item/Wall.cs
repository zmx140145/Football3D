using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private Vector3 normal=Vector3.zero;
    public GameObject pluspoint;
    
    public GameObject subtractpoint;
   
    void Start()
    {
        pluspoint=transform.GetChild(0).gameObject;
        subtractpoint=transform.GetChild(1).gameObject;
    }
    public Vector3 ToMeNormal(Transform tf)
    {
        Vector3 tfToWall;
    tfToWall=transform.InverseTransformPoint(tf.position);
    if((pluspoint.transform.localPosition-tfToWall).sqrMagnitude<(subtractpoint.transform.localPosition-tfToWall).sqrMagnitude)
    {
    normal=pluspoint.transform.localPosition;
    return normal;
    }
    else
    {
        normal=subtractpoint.transform.localPosition;
    return normal;
    }
    }
}
