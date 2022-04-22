using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSupportAttacker : State<FieldPlayer>
{
      public string Name="PlayerSupportAttacker";
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
    if(!player.myTeam.m_pSupportingPlayers.Contains(player))
    {
        player.myTeam.m_pSupportingPlayers.Add(player);
    }
   
      
    player.m_pSteering.arrive=true;
    player.m_pSteering.SetArriveTarget(player.myTeam.GetSupportSpot(player),Deceleration.fast);

    }
     public override void Execute(FieldPlayer player)
    {
        //如果自己球队失去了控球，队员去到防守状态
        if(!player.myTeam.BallInControll())
        {
            player.GetFSM().ChangeState(PlayerDefending.Instance);
        }
        //如果最佳接应点改变了，那么改变操控目标
        if(player.myTeam.GetSupportSpot(player)!=player.m_pSteering.GetArriveTarget())
        {
            player.m_pSteering.SetArriveTarget(player.myTeam.GetSupportSpot(player),Deceleration.fast);
            player.m_pSteering.arrive=true;
        }
      //TODO:我把所有请求传球都放在wait里面
        //如果这名队员可以射门，且进攻队员可以传球给他
        //那么把球传到这个球员
        // Vector3 ShootTarget=Vector3.zero;
        // if(player.myTeam.CanShoot(player.M_Pos(),Player.Instance.player_SO.MaxShootingForce,ref ShootTarget))
        // {
        //     if(player.isThreatened()<player.playerData.ThreatenedPassFactor)
        //     {
        //   player.myTeam.RequestPass(player);
        //     }
           
    
        // }
        //如果这名队员在接应点，且它的球队仍然控制着球
        //他应该停在那么，转向面对球
        if(player.AtTarget())
        {
            player.m_pSteering.arrive=false;
            //球员盯着球
            player.TrackBall();
            player.SetVelocity(Vector3.zero);
            //如果没有受到其他队员威胁，那么请求传球
            //TODO:个人觉得这个受到威胁不要简单地true和false 应该评等级
           player.GetFSM().ChangeState(PlayerWait.Instance);
        }
    }
     public override void Exit(FieldPlayer player)
    {
         player.m_pSteering.InitBool();
        //要保证退出状态之后myteam的支援队员是空的
        //让其他队员可以进行支援
        if(player.myTeam.m_pSupportingPlayers.Contains(player))
        {
        player.myTeam.m_pSupportingPlayers.Remove(player);
        }
    }
    public override string GetName()
    {
        return Name;
    }
    private static PlayerSupportAttacker instance;
     public static PlayerSupportAttacker Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerSupportAttacker();
    }
    return instance;
}
     }
}
