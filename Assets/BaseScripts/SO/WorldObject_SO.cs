using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Data",menuName ="WorldObject/Data")]
public class WorldObject_SO :ScriptableObject
{
    [Header("检测盒子")]
    public float DetectionBoxLength;
    public float DetectionBoxWidth;
    public float DetectionBoxHeight;
    public bool DetectionBoxJustFront;
    [Header("检测视野")]
    public float DetectionViewLength;
    public float DetectionViewAngle;
     [Header("世界力")]
     //摩擦力
     public float Firction;
     //对球的摩擦力
     public float BallFirction;
     
     [Header("世界选项")]
     //射门随机选取点的次数
     public int NumAttemptsToFindValidStrike;
     public float BallOwnerRange;
     //球员之间如果大于这个距离 就切换进攻
     public float JudgeBallOwnerRange;
     //球的判断owner距离
      [Header("steer力的比重")]
      //Arrive
     public float Mult_Arrive;
     //Seek
     public float Mult_Seek;
     public float Mult_Flee;
     public float Mult_Pursuit;
     public float Mult_WallAvoidance;
     public float  Mult_ObstacleAvoidance;
     public float Mult_Wander;
     public float Mult_Evade;
     [Header("积分点选项")]
    
     //传球安全的分数
     public float Spot_PassSafeStrength;
     //可以射门的分数
     public float Spot_CanScoreStrength;
     //传球距离的加成分数
     public float Spot_PassDistanceStrength;
     //离支援球员距离的加成分数
     public float Spot_SupportDistanceStrength;
     //离进攻区域近的加成分数
     public float Spot_NearAttackAreaStrength;
     //最佳区域距离
     public float NiceAreaDistance;
     public float MaxAreaDistance;
     //离支援球员最佳距离
     public float NiceSupportDistance;
     //离支援球员最远的距离
     public float MaxSupportDistance;
     //离支援球员最近的距离
     public float MinSupportDistance;
     //传球的最远距离
     public float OptimalDistance;
     //传球最佳距离
     public float NicePassDistance;
      [Header("关于截球")]
      public float StrengthDifferInfluence;
      public float CutBallProbability; 
     [Header("行为概率")]
     public float ChanceOfUsingArriveTypeReceiveBall;
     //球员尝试射门的概率
     public float ChancePlayerAttemptsPotShot;
     //射门偏差的概率(0到1)
     public float PlayerKickingAccuracy;
      [Header("更新时间")]
      public float SearchPlayerTime;


}
