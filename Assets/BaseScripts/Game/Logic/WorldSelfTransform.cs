using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSelfTransform : Singleton<WorldSelfTransform>
{
    public Vector3 WorldToSelf(Vector3 selfPos,Vector3 forward,Vector3 right,Vector3 up,Vector3 target)
    {
        transform.position=selfPos;
        transform.forward=forward;
        transform.right=right;
        transform.up=up;
        
        return transform.InverseTransformPoint(target);
    }
    public Vector3 SelfToWorld(Vector3 selfPos,Vector3 forward,Vector3 right,Vector3 up,Vector3 target)
    {
        transform.position=selfPos;
        transform.forward=forward;
        transform.right=right;
        transform.up=up;
        return transform.TransformPoint(target);
    }
}
