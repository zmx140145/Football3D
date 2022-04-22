using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCutBall : State<FieldPlayer>
{
   
      public string Name="PlayerCutBall";
    private Region region;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool OnMessage(FieldPlayer entity_Type, Telegram telegram)
    {
        
      
        return false;
    }
    public override void Enter(FieldPlayer player)
    {
        if(player.myTeam.IsOurTeamMember( player.m_pBall.m_pOwner))
        {
            player.GetFSM().ChangeState(PlayerWait.Instance);
            player.DefendTarget=null;
                  return;
        }
        //如果判断无法抢断 那么就返回上一个状态
        //如果有defendTarget说明是从player chase attacker里面来的 如果没有那么就是global里面来 叫你去截传球
       if(player.DefendTarget)
       {
//如果有defendTarget那么只能从playerDefendSupport或者playerChaseAttacker里面来 
       
    float OppStrong=player.DefendTarget.playerData.pb_StrongOfBodyStrenth;
        float PlayerStrong=player.playerData.pb_StrongOfBodyStrenth;
        if(OppStrong>PlayerStrong&&(player.DefendTarget.M_Pos()-player.M_Pos()).magnitude<player.playerData.BodyRadius+player.DefendTarget.playerData.BodyRadius)
            {
                //利用一个随机数 如果小于可以抢球的概率 那么就不能执行抢球 返回到之前进入抢球的状态
              if(Random.Range(0f,1f)*(1+(PlayerStrong-OppStrong)/WorldObject.Instance.worldObject_SO.StrengthDifferInfluence)<WorldObject.Instance.worldObject_SO.CutBallProbability)
              {
                  player.GetFSM().RevertToPreviousState();
                  return;
              }
            }
       }
        player.m_pSteering.arrive=true;
        player.m_pSteering.seek=false;
    }
     public override void Execute(FieldPlayer player)
    {
           //这里的Target要去赋值 
        //通过SupportPointCalculate来赋值
        //他是通过allowflow来控制
        Vector3 ToTarget;
       
           ToTarget=player.m_pBall.M_Pos()-player.M_Pos();
           ToTarget.Normalize();
        if(player.BallWithinKickingRange())
        {
        float dot=Vector3.Dot(ToTarget,player.M_vHeading());
        //如果球在队员和自己方球门之间，他需要通过多次轻踢，来小转弯
        //使得球朝向正确方向
        if(dot<Mathf.Cos(WorldObject.Instance.AngleTransformToFloat(player.playerData.CanTurnBallAngle) ))
        {
            Debug.Log("在转向");
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
           direction=(output-input+Vector3.ClampMagnitude(player.M_vVelocity(),player.playerData.TurnDirctionValue*player.LeftShiftTurnDirectionPower)*player.playerData.TurnDirectionGlobalValue).normalized;
          
           float dotPower=Vector3.Dot(player.M_vHeading(),direction);
           //当球员正在试图控制球，且同时转弯时，这个值起到很好地作用
            float KickingForce=player.playerData.TurnBallForce;
           
           player.m_pBall.Kick(direction,KickingForce*player.LeftShiftKickBallPower);
          
           player.m_pSteering.SetArriveTarget(player.m_pBall.M_Pos()+player.m_pBall.M_vVelocity(),Deceleration.fast);
          player.InitKickCD();
            
            
        } 
        else
        {
            Debug.Log("向前");
            player.m_pBall.Kick(ToTarget,Player.Instance.player_SO.MaxDribbleForce*player.LeftShiftKickBallPower);
            player.m_pSteering.SetArriveTarget(player.m_pBall.M_Pos()+player.m_pBall.M_vVelocity(),Deceleration.fast);
            player.InitKickCD();
    }
       //该队员已经踢球了，所以他必须改变状态去追球
        player.m_pBall.m_pOwner=player;
    }
        player.GetFSM().ChangeState(PlayerSnatchBall.Instance);
        return;
    }
     public override void Exit(FieldPlayer player)
    {
         player.m_pSteering.InitBool();
        //这个defendtarget是在chase和support状态留下来的 防止mp——owner为空
    
    }
   
    public override string GetName()
    {
        return Name;
    }
    private static PlayerCutBall instance;
     public static PlayerCutBall Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerCutBall();
    }
    return instance;
}
     }
}
