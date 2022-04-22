using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerDribbleBall : State<PlayerController>
{
      
      public string Name="PlayerDribbleBall";
    private Region region;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool OnMessage(PlayerController entity_Type, Telegram telegram)
    {
        
      
        return false;
    }
    public override void Enter(PlayerController player)
    {
    //让球队知到球员带球了 
    player.myTeam.SetControllingPlayer(player);
    
    }
     public override void Execute(PlayerController player)
    {

        Vector3 ToTarget;
          if(player.Dirction!=Vector3.zero)
        {
            ToTarget=player.Dirction;
        }
        else
        {
           ToTarget=player.m_pBall.M_Pos()-player.M_Pos();
           ToTarget.Normalize();
        }
        //关于速度过大要先停一下球
        if(player.m_pBall.M_vVelocity().magnitude<player.playerData.MaxStopBallSpeed)
        {
           if(player.isReadyForNextKick())
           {
           
            
                player.m_pBall.Trap(player);
            
           }
        
        }
        float dot=Vector3.Dot(ToTarget,player.M_vHeading());
        //如果球在队员和自己方球门之间，他需要通过多次轻踢，来小转弯
        //使得球朝向正确方向
        if(dot<Mathf.Cos(WorldObject.Instance.AngleTransformToFloat(player.playerData.CanTurnBallAngle) ))
        {
           if(dot<Mathf.Cos(WorldObject.Instance.AngleTransformToFloat(player.playerData.TrunBackRange) ))
           {
            //队员的朝向稍微转向一些（pi/4）,然后在那个方向踢球
            Vector3 direction=player.M_vHeading();
            //计算队员的朝向和球门朝向之间的角度的正负号
            //使得队员可以转到正确的方向
            
            float sign=Mathf.Sign(Vector3.SignedAngle(direction,ToTarget.normalized,Vector3.up)) ;
            //单次转向的可转角度
            float angle=sign*WorldObject.Instance.AngleTransformToFloat(player.playerData.SingleTurnBackBallAngle);
            Vector3 input=player.M_vHeading();
            Vector3 output;
          output.y=input.y;
            output.z=input.z*Mathf.Cos(angle)-input.x*Mathf.Sin(angle);
     output.x=input.x*Mathf.Cos(angle)+input.z*Mathf.Sin(angle);
           //计算方向
           direction=(output-input+Vector3.ClampMagnitude(player.M_vVelocity()/player.playerData.MaxSpeedWithBall,player.playerData.TurnDirctionValue*player.LeftShiftTurnDirectionPower)*player.playerData.TurnDirectionGlobalValue).normalized;
          
           float dotPower=Vector3.Dot(player.M_vHeading(),direction);
           //当球员正在试图控制球，且同时转弯时，这个值起到很好地作用
            float KickingForce=player.playerData.TrunBackBallForce*(1-player.M_vVelocity().magnitude/player.playerData.MaxSpeedWithBall);
           
           player.m_pBall.Kick(direction,KickingForce*player.LeftShiftKickBallPower);
            
           player.m_pSteering.SetArriveTarget(player.m_pBall.FuturePosition(player.playerData.PredictTime),Deceleration.fast);
          player.InitKickCD();
           }
           else
           {
               //队员的朝向稍微转向一些（pi/4）,然后在那个方向踢球
            Vector3 direction=player.M_vHeading();
            //计算队员的朝向和球门朝向之间的角度的正负号
            //使得队员可以转到正确的方向
            
            float sign=Mathf.Sign(Vector3.SignedAngle(direction,ToTarget.normalized,Vector3.up)) ;
            //单次转向的可转角度
            float angle=sign*WorldObject.Instance.AngleTransformToFloat(player.playerData.SingleTurnBallAngle);
            Vector3 input=player.M_vHeading();
            Vector3 output;
          output.y=input.y;
            output.z=input.z*Mathf.Cos(angle)-input.x*Mathf.Sin(angle);
     output.x=input.x*Mathf.Cos(angle)+input.z*Mathf.Sin(angle);
           //计算方向
           direction=(output-input+Vector3.ClampMagnitude(player.M_vVelocity()/player.playerData.MaxSpeedWithBall,player.playerData.TurnDirctionValue*player.LeftShiftTurnDirectionPower)*player.playerData.TurnDirectionGlobalValue).normalized;
          
           float dotPower=Vector3.Dot(player.M_vHeading(),direction);
           //当球员正在试图控制球，且同时转弯时，这个值起到很好地作用
            float KickingForce=player.playerData.TurnBallForce;
           
           player.m_pBall.Kick(direction,KickingForce*player.LeftShiftKickBallPower);
            
           player.m_pSteering.SetArriveTarget(player.m_pBall.FuturePosition(player.playerData.PredictTime),Deceleration.fast);
          player.InitKickCD();
           }
            
        } 
        else
        {
            
            
            player.m_pBall.Kick(ToTarget,Player.Instance.player_SO.MaxDribbleForce*player.LeftShiftKickBallPower);
            
            player.m_pSteering.SetArriveTarget(player.m_pBall.FuturePosition(player.playerData.PredictTime),Deceleration.fast);
            player.InitKickCD();
        }
        player.m_pBall.m_pOwner=player;
        //该队员已经踢球了，所以他必须改变状态去追球
        player.GetFSM().ChangeState(PlayerControllerChaseBall.Instance);
        return;
    }
     public override void Exit(PlayerController player)
    {
        
    }
    public override string GetName()
    {
        return Name;
    }
    private static PlayerControllerDribbleBall instance;
     public static PlayerControllerDribbleBall Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerControllerDribbleBall();
    }
    return instance;
}
     }
}
