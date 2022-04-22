using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviorsCalculate : Singleton<BallBehaviorsCalculate>
{
    public  Vector3 AddNoiseToKick(Vector3 StartPos,Vector3 Target)
    {
        Vector3 noise=Vector3.Cross(Target-StartPos,Vector3.up);
        return Target+(Random.Range(-1f,1f)*noise.normalized)*WorldObject.Instance .worldObject_SO.PlayerKickingAccuracy;
    }
}
