using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChaseBall : State<FieldPlayer>
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
    public override bool OnMessage(FieldPlayer entity_Type, Telegram telegram)
    {
        
      
        return false;
    }
    public override void Enter(FieldPlayer player)
    {
        //seek和arrive二选一
    player.m_pSteering.seek=true;
    player.m_pSteering.arrive=false;

    }
     public override void Execute(FieldPlayer player)
    {
        //如果球在他的范围，那么改变转态为kickball
        if(player.BallWithinKickingRange())
        {
            player.GetFSM().ChangeState(PlayerKickBall.Instance);
            return;
        }
        //如果球员离球最近，那么继续追球
        if(player.isClosestTeamMemberToBall())
        {
            player.m_pSteering.SetArriveTarget(player.m_pBall.M_Pos(),Deceleration.fast);
            return;
        }
        //如果不是离球最近的
        //那么他就应该回到进功位置，进行相应的进攻操作
        player.GetFSM().ChangeState(PlayerAttacking.Instance);
    }
     public override void Exit(FieldPlayer player)
    {
         player.m_pSteering.InitBool();
        //这个开始时启动的效果 在kickball时结束
        
    }
    public override string GetName()
    {
        return Name;
    }
    private static PlayerChaseBall instance;
     public static PlayerChaseBall Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerChaseBall();
    }
    return instance;
}
     }
}