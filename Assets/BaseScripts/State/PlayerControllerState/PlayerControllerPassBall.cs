using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerPassBall : State<PlayerController>
{
     public string Name="PlayerControllerPassBall";
    Vector3 TargetPos;
    Vector3 playerDirction;
    PlayerBase Target;
    float force;
    bool flag=false;

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
        Target=null;
        TargetPos=Vector3.zero;
        force=player.KickPower;
        playerDirction=player.Dirction;
    flag=player.PlayerControllerFindPassWayFromDirection(player.playerData.limitDotDomainToPass,force,playerDirction,out TargetPos,out Target);
    player.PassKey=false;
    }
    public override void Execute(PlayerController player)
    {
       
if(flag)
{
player.m_pBall.Kick(TargetPos-player.m_pBall.M_Pos(),force);

player.InitKickCD();
//发消息给传球队员
MessageDispatcher.Instance.DispatchMessage(0,player.ID,Target.ID,MessageType.PlayerRececeiveBall,TargetPos);
player.GetFSM().ChangeState(PlayerControllerMoveFreedom.Instance);
return;
}
else
{
    player.m_pBall.Kick(playerDirction,force);
    
player.InitKickCD();
    player.GetFSM().ChangeState(PlayerControllerMoveFreedom.Instance);
    return;
}


    }
     public override void Exit(PlayerController player)
    {
        
    }
    public override string GetName()
    {
        return Name;
    }
    private static PlayerControllerPassBall instance;
     public static PlayerControllerPassBall Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerControllerPassBall();
    }
    return instance;
}
     }
}
