using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerTeam : BaseGameEntity
{
    //关键队员
public int TeamNum;
 public PlayerBase m_pReceivingPlayer;
 public PlayerBase m_pPlayerClosestToBall;
 public PlayerBase m_pControllingPlayer;
 public HashSet<PlayerBase> m_pSupportingPlayers;
 public PlayerBase m_pDefendSupportingPlayer;
 public SoccerBall m_pBall;
 public float SearchPlayerTime;
  public List<PlayerBase> OurTeamMembersList;
  public SoccerTeam OpponentsTeam;
  
  public Goal OurGoal;

  public Goal OpponentsGoal;
  //球队状态机
  private StateMachine<SoccerTeam> m_pStateMachine;
 public string StateName;
  public  void Awake() {
      
    m_pStateMachine= new StateMachine<SoccerTeam>(this);
    m_pSupportingPlayers=new HashSet<PlayerBase>();
    
  
    }
 public  override void Start() {
       base.Start();
      if( SoccerPitch.Instance.m_pRedTeam!=this)
      {
           OpponentsTeam=SoccerPitch.Instance.m_pRedTeam;
           TeamNum=1;
      }
      else
       {
          OpponentsTeam=SoccerPitch.Instance.m_pBlueTeam;
          TeamNum=0;
       }
       foreach(PlayerBase pb in OurTeamMembersList)
       {
           pb.myTeam=this;
       }
       m_pBall=SoccerPitch.Instance.m_pBall;
       m_pStateMachine.SetCurrentState(StartGameState.Instance);
       m_pStateMachine.SetGlobalState(TeamGlobalState.Instance);
  }
  //更新状态机
  private void Update() {
      m_pStateMachine.StateMachineUpdate();
      if(SearchPlayerTime<0f)
      {
           JudgeM_pPlayer();
          SearchPlayerTime=WorldObject.Instance.worldObject_SO.SearchPlayerTime;
         
      }
      SearchPlayerTime-=Time.deltaTime;
      StateName=GetFSM().m_pCurrentState.GetName();
  }

  //设置球队重要的球员指针
  public void SetControllingPlayer(PlayerBase pb)
  {
      m_pControllingPlayer=pb;
  }
  public void SetPlayerClosestToBall(PlayerBase pb)
  {
      m_pPlayerClosestToBall=pb;
  }
  public void SetReceivingPlayer(PlayerBase pb)
  {
      m_pReceivingPlayer=pb;
  }
  public void AddSupportingPlayer(PlayerBase pb)
  {
      if(!m_pSupportingPlayers.Contains(pb))
      {
      m_pSupportingPlayers.Add(pb);
      }
  }
  public void RemoveSupportingPlayer(PlayerBase pb)
  {
      if(m_pSupportingPlayers.Contains(pb))
      {
          m_pSupportingPlayers.Remove(pb);
      }
  }
   public void SetDefendSupportingPlayer(PlayerBase pb)
  {
     m_pDefendSupportingPlayer=pb;
  }

public void JudgeM_pPlayer()
{
    float distance1=float.MaxValue;
    float distance2=float.MaxValue;

    PlayerBase One=null;
    PlayerBase Two=null;
   foreach(PlayerBase pb in OurTeamMembersList)
   {
      //如果有球员距离小于dis1
      
   if((pb.M_Pos()-m_pBall.M_Pos()).magnitude<distance1)
   { 
       if(One!=null)
       {
           Two=One;
           //因为第一现在变成了第二
           //所以把之前记录的位置给到2
           distance2=distance1;
       }
      One=pb;
      distance1=(pb.M_Pos()-m_pBall.M_Pos()).magnitude;
   }
   else
   {
    if((pb.M_Pos()-m_pBall.M_Pos()).magnitude<distance2)
    {
         Two=pb;
         distance2=(pb.M_Pos()-m_pBall.M_Pos()).magnitude;
    }
      }
   } 
   if(One)
   {
       m_pPlayerClosestToBall=One;
   }
   if(Two)
   {
       m_pDefendSupportingPlayer=Two;
   }
  
 
}

//返回离球员最近的队友
public PlayerBase NearestMemberForPlayerInOurTeam(PlayerBase pb)
{


    PlayerBase target=null;
float dist=float.MaxValue;
foreach(PlayerBase member in OurTeamMembersList)
{
    if(member!=pb)
    {
        float d= (member.M_Pos()-pb.M_Pos()).magnitude;
        if(d<dist)
        {
            dist=d;
            target=member;

        }
       
    }
}
return target;
}

  //关于踢球的计算
 

  public bool isPassSafeFromOpponents(Vector3 from,Vector3 target,PlayerBase receiver,float PassingForce)
  {
   foreach(var member in OpponentsTeam.OurTeamMembersList)
   {
       if(!isPassSafeFromOpponent(from,target,receiver,member,PassingForce))
       {
           return false;
       }
       
   }
   return true;
  }
  public bool isPassSafeFromOpponent(Vector3 from,Vector3 target,PlayerBase receiver,PlayerBase opp,float PassingForce)
  {
      //把对手的位置转化为自己的本地空间
      Vector3 ToTarget=target-from;
      Vector3 ToTargetNormalized=ToTarget.normalized;
      Vector3 right=-Vector3.Cross( ToTargetNormalized,Vector3.up);
      Vector3 LocalPosOpp=WorldSelfTransform.Instance.WorldToSelf(from,ToTargetNormalized,right,Vector3.up,opp.M_Pos());
     //如果对手在踢球者后面，那么可以传球
     //要假设球踢出的速度远大于对手的最大速度
     if(LocalPosOpp.z<0)
     {
         return true;
     }
     //如果对手到目标的距离更远
     //那么我们需要考虑是否对手可以比接球队员先到达该位置
     if(Vector3.Distance(from,target)<Vector3.Distance(opp.M_Pos(),from))
     {
        //看是否有接球者
         if(receiver)
         {
         if(Vector3.Distance(receiver.M_Pos(),target)<Vector3.Distance(opp.M_Pos(),target))
         {
             return true;
         }
         }
         //没有接受者直接返回真
         else
         {
             return true;
         }
     }
     //计算球多久到达与对手正交的位置
     float TimeForBall=SoccerPitch.Instance.m_pBall.TimeToCoverDistance(Vector3.zero,new Vector3(0,0,LocalPosOpp.z),PassingForce);
     //计算这段时间里对手能跑多远
     float reach=opp.M_MaxSpeed()*TimeForBall+SoccerPitch.Instance.m_pBall.Radius+opp.Radius;
     //如果到对手的局部x距离小于他能跑到的最远距离，那么会被截球
     if(Mathf.Abs(LocalPosOpp.x)<reach)
     {
         return false;
     }
     return true;

     //TODO:后续会考虑转向的问题

  }
 //是否能射门
 public bool CanShoot(Vector3 BallPos,float power,ref Vector3 ShotTarget)
 {
  //这个方法要测试的随机选取的射门目标的数目
  int NumAttempts=WorldObject.Instance.worldObject_SO.NumAttemptsToFindValidStrike;
  //射门位置的y值应该在两个门柱之间(考虑球的半径)
  //左右的随机
  Vector3 halfV=(OpponentsGoal.m_vLeftPost-OpponentsGoal.m_vRightPost)/2-OpponentsGoal.m_vRightPost.normalized*SoccerPitch.Instance.m_pBall.Radius;
  Vector3 halfH=Vector3.up.normalized*(OpponentsGoal.height/2-SoccerPitch.Instance.m_pBall.Radius);
  while(NumAttempts-->0)
  {
//沿着对方的球门口选择一个随机的位置（确保考虑了球的半径）
ShotTarget=OpponentsGoal.m_vCenter;
float randomV=Random.Range(-1,1);
float randomH=Random.Range(-1,1);
ShotTarget+=randomV*halfV+halfH*randomH;
//先调整为二维的
ShotTarget.y=0f;
//确保踢球力足够越过球门线
double time =SoccerPitch.Instance.m_pBall.TimeToCoverDistance(BallPos,ShotTarget,power);
//如果时间大于零再测试是否会被对手截球
if(time>0)
{
    if(isPassSafeFromOpponents(BallPos,ShotTarget,null,power))
    {
        return true;
    }
      
    
}
  }   
  return false;
 }
 //是否能安全传球 是返回目标和传球的位置
//寻找是否可以传球
public bool FindPass(PlayerBase passer,ref PlayerBase receiver,ref Vector3 PassTarget,float power,float MinPassingDistance)
{
    IEnumerator<PlayerBase> curPlyr=OurTeamMembersList.GetEnumerator();
    float ClosestToGoalSoFar=float.MaxValue;
    Vector3 BallTarget;
    //遍历所有自己球队的球员
    while(curPlyr.MoveNext())
    {
//确保潜在的接球队员不是自己，且他所在的位置与传球的队员的距离大于最小的传球距离
if((curPlyr.Current!=passer)&&(Vector3.Distance(passer.M_Pos(),curPlyr.Current.M_Pos())>MinPassingDistance))
{
if(GetBestPassToReceiver(passer,curPlyr.Current,out BallTarget,power))
{
//如果传球目标是到目前为止找到的离对方球门线最近的
//记录它
float DistToGoal=Mathf.Abs(BallTarget.z-OpponentsGoal.m_vCenter.z);
if(DistToGoal<ClosestToGoalSoFar)
{
    ClosestToGoalSoFar=DistToGoal;
    //记录这个球员
    receiver=curPlyr.Current;
    //记录位置
    PassTarget=BallTarget;
}
}
}
    }
 if(receiver!=null)
 {
     return true;
 }
 else
 {
     return false;
 }
}


//寻找最优路径
public bool GetBestPassToReceiver(PlayerBase passer,PlayerBase receiver,out Vector3 PassTarget,float power)
{
    //首先计算球到达这个接球队员要花多少时间
    PassTarget=Vector3.zero;
    float time=SoccerPitch.Instance.m_pBall.TimeToCoverDistance(SoccerPitch.Instance.m_pBall.M_Pos(),receiver.M_Pos(),power);
    //如果在给点的力的作用下无法使球到达接球队员那里，返回假
    if(time<=0)
    {
        PassTarget=Vector3.zero;
        return false;
    }
    //接球队员这段时间能够覆盖的最大的距离
    float InterceptRange=time*receiver.M_MaxSpeed();
    //使用一个系数来模拟加速所消耗时间
    //TODO:后期可以改进
    const float ScalingFactor=0.3f;
    InterceptRange*=ScalingFactor;
    //计算在球到接球队员范围圈的切线范围的传球目标
    Vector3 ip1,ip2;
    GetTangentPoints(receiver.M_Pos(),InterceptRange,SoccerPitch.Instance.m_pBall.M_Pos(),out ip1,out ip2);
    
   List<Vector3> Passes=new List<Vector3>(){ip1,receiver.M_Pos(),ip2};
   float ClosestSoFar =float.MaxValue;
   bool bResult =false;
   foreach(var pass in Passes)
   {
       float dist=Mathf.Abs(pass.z-OpponentsGoal.m_vCenter.z);
       if((dist<ClosestSoFar)&&SoccerPitch.Instance.IsInPitch(pass)&&isPassSafeFromOpponents(SoccerPitch.Instance.m_pBall.M_Pos(),pass,receiver,power))
       {
           ClosestSoFar=dist;
           PassTarget=pass;
           bResult=true;
       }
   }
return bResult;
   

    
}
//获得圆外一点到圆的切点坐标
public void GetTangentPoints(Vector3 target,float range,Vector3 StartPoint,out Vector3 ip1,out Vector3 ip2)
{
    Vector3 ToTarget=target-StartPoint;
    float DistanceToTarget=Vector3.Distance(target,StartPoint);
    float Z=DistanceToTarget-range*range/DistanceToTarget;
    float X=range/DistanceToTarget*Mathf.Sqrt(DistanceToTarget*DistanceToTarget-range*range);
    Vector3 p1=new Vector3(X,0,Z);
     Vector3 p2=new Vector3(-X,0,Z);
     Vector3 right=Vector3.right;
     Vector3 up=Vector3.up;
     Vector3.OrthoNormalize(ref ToTarget,ref up,ref right);
    ip1=WorldSelfTransform.Instance.SelfToWorld(StartPoint,ToTarget,right,up,p1);
     ip2=WorldSelfTransform.Instance.SelfToWorld(StartPoint,ToTarget,right,up,p2);
}

/*
一些关于判断的bool方法
*/

//判断player是不是我们球队的队员
public bool IsOurTeamMember(PlayerBase pb)
{
    if(pb)
    {
    if(pb.TeamNum==TeamNum)
    {
        return true;
    }
    else
    {
        return false;
    }
    }
    else{
        return false;
    }
}
//判断是否所有队员都已经回到初始位置
public bool AllPlayersAtHome()
{
    IEnumerator<PlayerBase> playerIE=OurTeamMembersList.GetEnumerator();
    while(playerIE.MoveNext())
    {
     if(playerIE.Current.BaseAreaNum!=playerIE.Current.EnterAreaNum)
     {
         return false;
     }
    }
    return true;
}
public bool BallInControll()
{
    if(m_pControllingPlayer!=null)
    {
        return true;
    }
    else
    {
        return false;
    }
}

public Vector3 GetSupportSpot(PlayerBase pb)
{
  return SupportSpotCalculator.Instance.DetermineBestSupportingPostion(this,pb);
}



/*状态*/
public override bool HandleMessage(Telegram msg)
{
    return m_pStateMachine.HandleMessage(msg);
}






//消息的发送
//对自己队的队员
//这是基于带球队员的判断 
//要是带球队员觉得请求传球的队员的被威胁值小 就执行传球  
//这函数把球踢了出去
public bool RequestPass(PlayerBase player)
{
    //这里的isThreatened是对于带球队员而言的 他以自己的标准来认为
    if(Player.Instance.isThreatened(m_pControllingPlayer,m_pControllingPlayer.playerData.OutsideRange,m_pControllingPlayer.playerData.InsideRange)<m_pControllingPlayer.playerData.ThreatenedPassFactor)

    {
    //计算指向球的向量与球员自己的朝向向量的点积
      
        Vector3 ToBall=m_pControllingPlayer.m_pBall.M_Pos()-m_pControllingPlayer.M_Pos();
        float dot=Vector3.Dot(m_pControllingPlayer.M_vHeading(),Vector3.Normalize(ToBall));
    
    float power=Player.Instance.player_SO.MaxPassingForce*dot;
    Vector3 BallTarget=Vector3.zero;
    GetBestPassToReceiver(m_pControllingPlayer,player,out BallTarget,power);
    //给踢球增加干扰
BallTarget=BallBehaviorsCalculate.Instance.AddNoiseToKick(player.m_pBall.M_Pos(),BallTarget);
Vector3 KickDirection=BallTarget-player.m_pBall.M_Pos();
player.m_pBall.Kick(KickDirection,power);
//让接球的队员知道要传球了
MessageDispatcher.Instance.DispatchMessage(0,m_pControllingPlayer.ID,player.ID,MessageType.PlayerRececeiveBall,BallTarget);
//因为mpcontrollerplayer不是fieldplayer而是他的父类 所以没有具体的statrmachine;
m_pControllingPlayer.GetComponent<FieldPlayer>()?.GetFSM().ChangeState(PlayerWait.Instance);
m_pControllingPlayer.FindSupport();
return true;
    }
    else
    {
        return false;
    }
}
public void AllFieldPlayersToDefendingArea()
{
     IEnumerator<PlayerBase> PlayerIE=OurTeamMembersList.GetEnumerator();
    while(PlayerIE.MoveNext())
    {
        
        MessageDispatcher.Instance.DispatchMessage(0,this.ID,PlayerIE.Current.ID,MessageType.PlayerDefending,null);
    }
}
public void AllFieldPlayersToAttackingArea()
{
     IEnumerator<PlayerBase> PlayerIE=OurTeamMembersList.GetEnumerator();
    while(PlayerIE.MoveNext())
    {
        
        MessageDispatcher.Instance.DispatchMessage(0,this.ID,PlayerIE.Current.ID,MessageType.PlayerAttacking,null);
    }
}
public void ReturnAllFieldPlayersToHome()
{
   
    IEnumerator<PlayerBase> PlayerIE=OurTeamMembersList.GetEnumerator();
    while(PlayerIE.MoveNext())
    {
        
        MessageDispatcher.Instance.DispatchMessage(0,this.ID,PlayerIE.Current.ID,MessageType.PlayerGoHome,null);
    }
}
public void DetermineBestSupportingPostion()
{
    foreach(PlayerBase pb in m_pSupportingPlayers)
    {
    SupportSpotCalculator.Instance.DetermineBestSupportingPostion(this,pb);
    }
}

 public StateMachine<SoccerTeam> GetFSM(){return m_pStateMachine;}





//准备开球

}


