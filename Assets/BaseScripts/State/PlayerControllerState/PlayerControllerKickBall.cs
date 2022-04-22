using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerKickBall: State<PlayerController>
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
    public override bool OnMessage(PlayerController entity_Type, Telegram telegram)
    {
        
      
        return false;
    }
    public override void Enter(PlayerController player)
    {
       Debug.Log("instate2enter");
    //让球队知道该球员正在控制球
    player.myTeam.SetControllingPlayer(player);
    //该球员每秒只能进行有限次数的踢球
    if(!player.isReadyForNextKick())
    {

        Debug.Log("exit");
        player.GetFSM().ChangeState(PlayerControllerChaseBall.Instance);
    }
    //判断是否按下shift
    //如果没按就设置速度比例为1；
     if(!player.LeftShiftKey)
            {
               
          player.LeftShiftWithBallPower=1f;
          player.LeftShiftKickBallPower=1f;
          player.LeftShiftTurnDirectionPower=1f;
            }
    }
     public override void Execute(PlayerController player)
    {
         
        //计算指向球的向量与球员自己的朝向向量的点积
        Vector3 ToBall=player.m_pBall.M_Pos()-player.M_Pos();
        float dot=Vector3.Dot(player.M_vHeading(),Vector3.Normalize(ToBall));
        //如果守门员控制了球，或者球在该球员的后方
        //或者已经分配了接球队员，就不能踢球，所以只是继续追球
        if((dot<0f))
        {
           
            
            
            player.GetFSM().ChangeState(PlayerControllerChaseBall.Instance);
            return;
            
        }
       
       else
       {
        if(player.PassKey)
            {
                player.GetFSM().ChangeState(PlayerControllerPassBall.Instance);
             return;
            }
          
        player.GetFSM().ChangeState(PlayerControllerDribbleBall.Instance);
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
    private static PlayerControllerKickBall instance;
     public static PlayerControllerKickBall Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerControllerKickBall();
    }
    return instance;
}
     }
}
