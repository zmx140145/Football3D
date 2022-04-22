using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefending : State<FieldPlayer>
{
    public string Name="PlayerDefending";
   
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
        //设置player在防守
        player.isDefending=true;
        player.isAttacking=false;
         player.InitOppRecord();
    //到时如果改成可以变阵型的就靠这个来不断更新region 而不是playerbase一开始就初始化
  SoccerPitch.Instance.m_RegionsList.TryGetValue(player.DefendAreaNum,out player.DefendRegion);
    }
     public override void Execute(FieldPlayer player)
    {
       
        
       
         //判断是否需要再等待（因为刚追完人，如果自己的位置有队友那么就等待）
        if(player.IsRegionAreaHaveMyTeamMember(player.DefendAreaNum))
        {
        player.m_pSteering.SetArriveTarget(player.M_Pos(),Deceleration.normal);
        player.GetFSM().ChangeState(PlayerWait.Instance);
        }
       
          //TODO:现在回到区域不在defending里面做 而是在defendinArea里面
        player.GetFSM().ChangeState(PlayerDefendInArea.Instance);
        
    }
     public override void Exit(FieldPlayer player)
    {
         player.m_pSteering.InitBool();
    }
    public override string GetName()
    {
        return Name;
    }
    private static PlayerDefending instance;
     public static PlayerDefending Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerDefending();
    }
    return instance;
}
     }
}
