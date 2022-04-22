using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerReceiveBall : State<PlayerController>
{
    public string Name="PlayerControllerReceiveBall";
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
    
 //让球队知道，这个球员正在接球
        player.myTeam.SetReceivingPlayer(player);
        //该球员现在也是控球队员
        player.myTeam.SetControllingPlayer(player);
        //有两类接球行为
        //1.用Arrive指导接球球员到达传球球员发送消息中提供的位置
        //2.用Pursuit来追球
        //ChanceOfUsingArriveTypeReceiveBehavior的可能性来进行选择
        //判断是否有对方球员靠近接球队员
        //是否接球队员在对方禁区（通过自己的区号来判断 而且不同队伍的人判断的区号范围不一样）
        
        if(player.InHotArea||Random.Range(0f,1f)<WorldObject.Instance.worldObject_SO.ChanceOfUsingArriveTypeReceiveBall&&(!player.OpponentWithinRadius()))
        {
player.m_pSteering.arrive=true;
player.m_pSteering.pursuit=false;
        }
        else{
            player.m_pSteering.arrive=false;
            player.m_pSteering.pursuit=true;
        }
    }
     public override void Execute(PlayerController player)
    {
       //如果他离球足够近或者他的球队失去球的控制权，那么应该改变状态去追球
       if(player.BallWithinReceiverRange()||!player.myTeam.BallInControll())
       {
           player.GetFSM().ChangeState(PlayerControllerChaseBall.Instance);

return;
       }
       //如果pirsuit操控行为被用来追球，那么该队员的目标必须随着球的位置
       //不断地更新
       if(player.m_pSteering.pursuit)
       {
           player.m_pSteering.SetPursuitTarget(player.m_pBall,Vector3.zero);
           
       }
       //r如果该队员到达了操控目标位置，那么它应该等在那里，并转向面对着球
       if(player.AtTarget())
       {
           player.m_pSteering.arrive=false;
           player.m_pSteering.pursuit=false;
           player.TrackBall();
           player.SetVelocity(Vector3.zero) ;
       }
    }
     public override void Exit(PlayerController player)
    {
        
    }
    public override string GetName()
    {
        return Name;
    }
    private static PlayerControllerReceiveBall instance;
     public static PlayerControllerReceiveBall Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerControllerReceiveBall();
    }
    return instance;
}
     }
}
