using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKickBall : State<FieldPlayer>
{
      public string Name="PlayerKickBall";
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
    //让球队知道该球员正在控制球
    player.myTeam.SetControllingPlayer(player);
    player.m_pBall.m_pOwner=player;
    //该球员每秒只能进行有限次数的踢球
    if(!player.isReadyForNextKick())
    {
        player.GetFSM().ChangeState(PlayerChaseBall.Instance);
    }

    }
     public override void Execute(FieldPlayer player)
    {
        //计算指向球的向量与球员自己的朝向向量的点积
        Vector3 ToBall=player.m_pBall.M_Pos()-player.M_Pos();
        float dot=Vector3.Dot(player.M_vHeading(),Vector3.Normalize(ToBall));
        //如果守门员控制了球，或者球在该球员的后方
        //或者已经分配了接球队员，就不能踢球，所以只是继续追球
        if(player.myTeam.m_pReceivingPlayer!=null||SoccerPitch.Instance.GoalKeeperHasBall()||(dot<0))
        {
            player.GetFSM().ChangeState(PlayerWait.Instance);
            return;
        }
        /*尝试射门*/
        //点积用来调整射门力度，球越在球员的正前方，踢球的力就越大
        float power=Player.Instance.player_SO.MaxShootingForce*dot;
        //如果可以射门，这个向量用来储存该球员瞄准的在对方 线上的位置
        Vector3 BallTarget=Vector3.zero;
        //如果确认该球员可以在这个位置射门，或者无论如何它都该带一下球
        //那该队员则试图射门
        if(player.myTeam.CanShoot(player.m_pBall.M_Pos(),power,ref BallTarget)&&Random.Range(0f,1f)<WorldObject.Instance.worldObject_SO.ChancePlayerAttemptsPotShot)
        {
         
        TODO: //通过改变playerkickingaccuracy来产生干扰
          //这是踢球的方向
          BallTarget=BallBehaviorsCalculate.Instance.AddNoiseToKick(player.m_pBall.M_Pos(),BallTarget);
          Vector3 KickDirection=BallTarget-player.m_pBall.M_Pos();
          KickDirection.y=0f;
          player.m_pBall.Kick(KickDirection.normalized,power);
          //改变状态
          player.GetFSM().ChangeState(PlayerWait.Instance);
          player.FindSupport();
          return;
        }
        /*尝试传球*/
        //如果找到接球队员，那么这将指向他
        PlayerBase receiver=null;
        power=Player.Instance.player_SO.MaxPassingForce*dot;
        //测试是否有任何潜在的候选人可以传球 
        //威胁指数1和0
        if(player.isThreatened()>0&&player.myTeam.FindPass(player,ref receiver,ref BallTarget,power,Player.Instance.player_SO.MinPassingDistance))
        {
//给踢球增加干扰
Debug.Log("传球");
BallTarget=BallBehaviorsCalculate.Instance.AddNoiseToKick(player.m_pBall.M_Pos(),BallTarget);
Vector3 KickDirection=BallTarget-player.m_pBall.M_Pos();
KickDirection.y=0f;
player.m_pBall.Kick(KickDirection,power);
//让接球的队员知道要传球了
MessageDispatcher.Instance.DispatchMessage(0,player.ID,receiver.ID,MessageType.PlayerRececeiveBall,BallTarget);
//该队员应该在当前位置等待，除非另有指示
player.FindSupport();
player.GetFSM().ChangeState(PlayerWait.Instance);

return;


        }
        //不能射门和传球，所以只能带球
        else{
        player.FindSupport();
        player.GetFSM().ChangeState(PlayerDribbleBall.Instance);
        }
    }
     public override void Exit(FieldPlayer player)
    {
       player.m_pSteering.InitBool();
    }
   
    public override string GetName()
    {
        return Name;
    }
    private static PlayerKickBall instance;
     public static PlayerKickBall Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerKickBall();
    }
    return instance;
}
     }
}
