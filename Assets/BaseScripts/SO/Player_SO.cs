using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Data",menuName ="Player/Data")]
public class Player_SO :ScriptableObject
{
    //加速倍率
    public float LeftShiftPower;
    public float LeftShiftWithBallPower;
    public float LeftShiftKickBallPower;
    public float LeftShiftFirstKickBallBall;
    public float LeftShiftTurnDirectionPower;
    //关于提升mpsteering的速度
    public float calculatePower;
    public float TurnDirctionValue;
    public float TurnDirectionGlobalValue;

    //最大速度
    public float MaxSpeed;
    //最大带球速度
    public float MaxSpeedWithBall;
    //最大加速度
    public float MaxForce;
    //最大踢球的力
     public float MaxShootingForce;
     public float MinPassingDistance;
     [Header("传球操作的设置")]
     //到达最小传球里的时间
     public float NicePassingTime;
     //最大传球的时间
     public float MaxPassingTime;
    //最大传球的力度
     public float MaxPassingForce;
     //最佳传球力度
     public float NicePassingForce;
    
      //-1到1
     //限制传球的角度
     public float limitDotDomainToPass;
     [Header("带球的操作设置")]
     //最大的带球力度
     public float MaxDribbleForce;
     //转向时踢球的力
     public float TurnBallForce;
     public float TrunBackBallForce;
     
     public float CanTurnBallAngle;
     public float TrunBackRange;
     //这个是转向时的转向角度
     public float SingleTurnBallAngle;
       public float SingleTurnBackBallAngle;
      [Header("空间感知距离设置")]
     public float InsideRange;
     public float OutsideRange;
     //强制碰撞体积 就是人的大小
     public float coliderVolume;
     //视野设置
     //视野长度
     public float ViewLength;
     //视野角度
     public float ViewAngle;
     //感知长度
     public float TouchLength;
     //感知角度
     public float TouchAngle;
     //接球的范围
     public float ReceiveBallRange;
     //可以踢球的范围
     public float KickBallRange;
     //控制的范围 超过这范围就判断为丢球 被对手夺球
     public float ControllBallRange;
     //踢球的CD
     public float KickBallCD;
     //球员的身体半径
     public float BodyRadius;
         [Header("抢球相关设置")]
     public float CutBallRange;
     public float CutBallAngle;
     public float pb_StrongOfBodyStrenth;
     public float AttemptToCutBallPosibility;
        
     [Header("玩家数值")]
     //玩家觉得受到威胁的程度 可以接受的范围
     //这个值是影响队员传球判断的
     //小于这个值的情况都能传球
     //1 是只有完全不被威胁才传球 2是轻度威胁 3是中度威胁 4是任何情况都能传
     public int ThreatenedPassFactor;
     //重量
     public float weight;
     //防守时和敌人保持的距离
     public float DefendKeepDistance;
     //与要追随的人保持的距离
      public float DefendPursuitKeepDistance;
     public float DefendPursuitRange;
public float MaxStopBallSpeed;
public float NeedStopBallSpeed;
//玩家预测时间
     public float PredictTime;
    

}
