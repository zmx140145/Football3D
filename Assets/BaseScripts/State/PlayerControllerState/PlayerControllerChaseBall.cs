using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerChaseBall : State<PlayerController>
{
  
           public string Name="PlayerChaseBall";
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
        //seek和arrive二选一
        Debug.Log("Enter");
    player.m_pSteering.seek=true;
    player.m_pSteering.arrive=false;

    }
     public override void Execute(PlayerController player)
    {
        player.m_pSteering.SetArriveTarget(player.m_pBall.FuturePosition(player.playerData.PredictTime),Deceleration.fast);
        
        if(player.BallOutOfViewRange())
        {
            player.GetFSM().ChangeState(PlayerControllerMoveFreedom.Instance);
            return;
        }
        //计算指向球的向量与球员自己的朝向向量的点积
        Vector3 ToBall=player.m_pBall.M_Pos()-player.M_Pos();
        float dot=Vector3.Dot(player.M_vHeading(),Vector3.Normalize(ToBall));
       
        //如果球在他的范围，那么改变转态为kickball
        if(player.BallWithinKickingRange())
        {
           
            player.GetFSM().ChangeState(PlayerControllerKickBall.Instance);
            return;
        }
        //如果球员离球最近，那么继续追球
        if(!player.isClosestTeamMemberToBall())
        {
          
             player.GetFSM().ChangeState(PlayerControllerMoveFreedom.Instance);
            return;
            
        }
       
       
    }
     public override void Exit(PlayerController player)
    {
        //这个开始时启动的效果 在kickball时结束
        
    }
    public override string GetName()
    {
        return Name;
    }
    private static PlayerControllerChaseBall instance;
     public static PlayerControllerChaseBall Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerControllerChaseBall();
    }
    return instance;
}
     }
 }

