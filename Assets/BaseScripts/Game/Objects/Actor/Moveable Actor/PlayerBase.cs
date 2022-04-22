using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CollInfo
{
   public PlayerBase HitPlayer;
   public Vector3 HitPlayerV3;
   public Vector3 Hitpoint;
   public Vector3 ToHitPoint;
   public Vector3 MyV3;
   public float angle;
   public float myAngle;
   public bool isDeal;
   public CollInfo(PlayerBase pb,Vector3 HpV3,Vector3 myV3,Vector3 pos,Vector3 Tohit,float angle,float myAngle)
   {
    HitPlayer=pb;
    HitPlayerV3=HpV3;
    MyV3=myV3;
    Hitpoint=pos;
    ToHitPoint=Tohit.normalized;
    this.angle=angle;
    this.myAngle=myAngle;

    isDeal=false;
   }
}
public class PlayerBase : Vehicle
 {  
    public Dictionary<int,CollInfo> HitPoints;
   
    public string StateName;
    //这里都是记录着对方队员的信息
    //记录正在被谁盯着
    public PlayerBase Opp_DefendedByPlayer;
     public PlayerBase Opp_DefendedSupportByPlayer;
    public PlayerBase Opp_PursuitedByPlayer;
    //这个控制和代码的运行频率
    public bool allowCodeFlow=false;
    private float codeFlowTime=0.1f;
    //储存支援点
    //记录support的点
    public SupportSpot m_pBestSupportSpot;
    //记录球员要前往的rigion
   public Region AttackRegion;
     public Region DefendRegion;
       public Region BaseRegion;
     //防守的目标
     public PlayerBase DefendTarget;
       public PlayerBase DefendSupportTarget;
       public PlayerBase DefendPrsuitTarget;
     
     //用来判断是在进攻状态还是防守状态  方便全局转态操作
     public bool isDefending=false;
     public bool isAttacking=false;
     //这个是方向
     public Vector3 Dirction;
     public Vector3 playerLastPos;
     
    //判断是否加Shift的倍率
    public bool LeftShiftKey;
    public float LeftShiftPower=1f;
    public float LeftShiftWithBallPower=1f;
    public float LeftShiftKickBallPower=1f;
    public float LeftShiftTurnDirectionPower=1f;
    //用于辨别是哪个球队的
    public int TeamNum;
   public bool calculate=false;
    public Player_SO playerData;
      
   
    //所属球队
    public SoccerTeam myTeam;
    public SoccerBall m_pBall;
    public float KickBallCD;
    
    //加速度
    public float Force;
    //当前所在位置的序号 由Regionx检测到player的时候调用更改
    public int EnterAreaNum=-1;
    //初始位置
      public int BaseAreaNum;
      public int DefendAreaNum;
      public int AttackAreaNum;
      public bool InHotArea;
      //判断是否有对手进入自己的范围
   
      
    
      // public PlayerBase(SoccerTeam myTeam)
      // {
      // this.myTeam=myTeam;
      // }
   public override void Awake()
    {
        base.Awake();
        
      
    }
    public override void Start()
    {
        base.Start();
        
        
        m_dMaxTurnRate=90f;
        m_dMaxForce=playerData.MaxForce;
        m_MaxSpeed=playerData.MaxSpeed;
        B_radius=playerData.BodyRadius;
        m_dMass=playerData.weight;
       m_pBall=SoccerPitch.Instance.m_pBall;
      TeamNum=myTeam.TeamNum;
      KickBallCD=0f;
      m_pBestSupportSpot=new SupportSpot(Vector3.zero,1f);
      HitPoints=new Dictionary<int, CollInfo>();
    }
    public override void  Update() {
       
       base.Update();
       m_vVelocity.y=0f;
         SteerUpdate(Time.deltaTime);
       if(m_pSteering!=null)
       {
         
          m_pSteering.EnterAreaNum=this.EnterAreaNum;
       }
       else
       {
           Debug.Log("warning");
       }
      
       KickBallCD-=Time.deltaTime;
       JudgeBallOwner();
       JudgeCodeFlow();
     
        //set maxspeed在golbal状态里
   
      
    }
   
  
    public void JudgeShiftKeyValue()
    {
       if(this.LeftShiftKey)
       {
          LeftShiftPower=playerData.LeftShiftPower;
         LeftShiftWithBallPower=playerData.LeftShiftWithBallPower;
         LeftShiftKickBallPower=playerData.LeftShiftKickBallPower*Mathf.Max(playerData.LeftShiftFirstKickBallBall/playerData.LeftShiftKickBallPower,M_vVelocity().magnitude/M_MaxSpeed());
         LeftShiftTurnDirectionPower=playerData.LeftShiftTurnDirectionPower;
       }
       else
       {
          LeftShiftPower=1f;
          //为了让球员能自动调整速度跟上球 不会因为放开shift而减慢速度
      //其他与带球相关的leftshiftpower在kickball中设置1f;
      //就是再次碰球时重置回初始速度
       }
    }
    public void JudgeCodeFlow()
    {
       if(codeFlowTime<=0)
       {
          allowCodeFlow=true;
          codeFlowTime=playerData.KickBallCD/2f;
       }
       else
       {
          allowCodeFlow=false;
          codeFlowTime-=Time.deltaTime;
       }
    }
    public void JudgeInPitch()
    {
    if(M_Pos().x<0||M_Pos().z<0||M_Pos().x>SoccerPitch.Instance.PitchWidth||M_Pos().z>SoccerPitch.Instance.PitchLength)
    {
       EnterAreaNum=-1;
    }
    }
    //判断是不是还控制着球 然后
    public void JudgeBallOwner()
{
    if(myTeam.m_pControllingPlayer==this)
    {
       //不是player 
        if(m_pBall.m_pOwner!=this)
        {
           //不是自己队员  一般设置为null 都因为不是自己的队员控球
           if(m_pBall.m_pOwner!=myTeam.OpponentsTeam.m_pControllingPlayer)
           {
              
             
              
           }
          else
          {
              if(m_pBall.m_pOwner==null)
              {
            if(BallOutOfViewRange())
            {
                myTeam.m_pControllingPlayer=null;
            }
              }
              else
              {
                 myTeam.m_pControllingPlayer=null;
              }
          }
        }
        else
        {
           myTeam.m_pControllingPlayer=this;
        }
    }
    //这是防守的状态
    if(myTeam.m_pControllingPlayer==null)
    {
       //如果控球队员是自己 那么说明正在截球 
       if(m_pBall.m_pOwner==this)
       {
          //如果对方认为已经失去对球的控制
       
          myTeam.m_pControllingPlayer=this;
       
       }
    }
    
}
//清空自己身上记录的对方防守自己的队员
public void InitOppRecord()
{
    Opp_DefendedByPlayer=null;
      Opp_PursuitedByPlayer=null;
}
    //看着球
    public void TrackBall()
    {
    m_vHeading=(m_pBall.M_Pos()-M_Pos()).normalized;
    }
//判断region是否有自己的队员
public bool IsRegionAreaHaveMyTeamMember(int areaNum)
{
foreach(PlayerBase pb in myTeam.OurTeamMembersList)
{
   if(pb!=this)
   {
      if( pb.EnterAreaNum==areaNum)
      {
         return true;
      }
     
   }
}
return false;
}
//判断region是否有对面的队员
public bool IsRegionAreaHaveOppTeamMember(int areaNum)
{
foreach(PlayerBase pb in myTeam.OpponentsTeam.OurTeamMembersList)
{
  
      if( pb.EnterAreaNum==areaNum)
      {
         return true;
      }
     
   
}
return false;
}
    //对手是否在判断范围之内 
    public bool OpponentWithinRadius()
    {
       Region region=null;
       //通过球员自身所在区域编号 在足球场内寻找region 
       //TODO:是在每个函数调用时遍历regionList 还是每次更新区域时更新队员携带region ？？？哪个更省时间
       if( SoccerPitch.Instance.m_RegionsList.TryGetValue(EnterAreaNum,out region))
       {
          List<PlayerBase> OppsList=new List<PlayerBase>();
        foreach(var obj in region.entitysList)
        {
           //如果不是队友
           if(obj.GetComponent<PlayerBase>()?.TeamNum!=this.TeamNum)
           {
              //TODO:问题 不知道fieldplayer在放进去playerbaseList里再拿出来 会不会有fieldplayer的信息  还是要重新gameobject.getcompent<>();
           OppsList.Add(obj.GetComponent<PlayerBase>());
           }
        }
        TagWithInViewRange<PlayerBase,PlayerBase>(this,OppsList,playerData.TouchLength,playerData.TouchAngle);
        IEnumerator<PlayerBase> ListIE=OppsList.GetEnumerator();
        while(ListIE.MoveNext())
        {
           if(ListIE.Current.m_Tag)
           {
              return true;
           }
        }
       }
      
       return false;
    }
    //判断自己是不是控球队员
    public bool isControllingPlayer()
    {
       if(this==myTeam.m_pControllingPlayer)
       {
          return true;
       }
       return false;
    }
    //判断自己比进攻队员更靠近对方半场
    public bool isAheadOfAttacker()
    {
       if(Mathf.Abs(M_Pos().z-myTeam.OurGoal.m_vCenter.z)>(Mathf.Abs(myTeam.m_pControllingPlayer.M_Pos().z-myTeam.OurGoal.m_vCenter.z)))
       {
return true;
       }
       return false;
    }
    //判断自己是不是离球最近的球员
    public bool isClosestTeamMemberToBall()
    {
       float ClosestDistanceSqToBall=float.MaxValue;
       float selfToBallSq=float.MaxValue;
      foreach(PlayerBase  pb in myTeam.OurTeamMembersList)
      {
         if(pb==this)
         {
         selfToBallSq=(pb.M_Pos()-m_pBall.M_Pos()).sqrMagnitude;
         }
         else
         {
    if((pb.M_Pos()-m_pBall.M_Pos()).sqrMagnitude<ClosestDistanceSqToBall)
    {
       ClosestDistanceSqToBall=(pb.M_Pos()-m_pBall.M_Pos()).sqrMagnitude;
    }
         }
      }
      if(selfToBallSq<ClosestDistanceSqToBall)
      {
         myTeam.m_pPlayerClosestToBall=this;
         return true;
      }
      else
      {
         return false;
      }
    }
    //球在可以踢的范围
    public bool BallWithinKickingRange()
    {
       if((m_pBall.M_Pos()-M_Pos()).magnitude<playerData.KickBallRange)
       {
          Debug.Log("yes distance");
       return true;
       }
       return false;
    
    }
    //球在控制范围
    public bool BallWithinControllRange()
    {
       if((m_pBall.M_Pos()-M_Pos()).magnitude<playerData.ControllBallRange)
       {
          return true;
       }
       else
       {
          return false;
       }
    }
    //球在接球范围 
    public bool BallWithinReceiverRange()
    {
       if((m_pBall.M_Pos()-M_Pos()).magnitude<playerData.ReceiveBallRange)
       {
       return true;
       }
       return false;
    }
    //设置一个时间 每次踢球之后让CD冷却
   
    public bool isReadyForNextKick()
    {
       if(KickBallCD<0)
       {
         
          return true;
      
       }
       else
       {
        
          return false;
       }
       
    }
    public void InitKickCD()
    {
        KickBallCD=playerData.KickBallCD;
    }
    //判断自己是否已经到达了目标
    public bool AtTarget()
    {
      if((M_Pos()-m_pSteering.GetArriveTarget()).magnitude<Radius)
      {
         return true;
      }
      else
      {
         return false;
      }
    }
    //是否受到威胁

//0不受威胁 1威胁1级 2威胁2级 3威胁3级 递增
public  int isThreatened()
{

  
  List<Region> regionsList=new List<Region>();
   Region m_region=null;
   int widthNum=SoccerPitch.Instance.WidthNum;
  
   List<PlayerBase> list=new List<PlayerBase>();
   //首先的到附近范围内的所以region
   SoccerPitch.Instance.m_RegionsList.TryGetValue(EnterAreaNum,out m_region);
   IEnumerator<Region> regionIE =m_region.NearbyRegionsList.GetEnumerator();
 
  while(regionIE.MoveNext())
  {
  regionsList.Add(regionIE.Current);
  }
   regionsList.Add(m_region);
   //要找到储存着player的list 从rigion里面拿 要拿上附近的8个rigion的player
   foreach(Region region in regionsList)
   {
   foreach(var obj in region.pbsList)
   {
     if( obj.TeamNum!=this.TeamNum)
     {
     list.Add(obj);
     }
   }
   }
   //然后用tagNeiberhour 标记外圈范围player 再判断是不是对手 是的话 outsideOppNum++
   int outsideOppNum=0;
   TagNeighbors<PlayerBase,PlayerBase>(this,list,playerData.OutsideRange);
   IEnumerator<PlayerBase> playerBaseIE=list.GetEnumerator();
   while(playerBaseIE.MoveNext())
   {
   if(playerBaseIE.Current.m_Tag)
   {
      outsideOppNum++;
   }
   }
   //然后同理是 insideOppNum++
   int insideOppNum=0;
   TagNeighbors<PlayerBase,PlayerBase>(this,list,playerData.InsideRange);
    playerBaseIE=list.GetEnumerator();
   while(playerBaseIE.MoveNext())
   {
   if(playerBaseIE.Current.m_Tag)
   {
      insideOppNum++;
   }
   
  
   }
   //注意外圈是包含内圈的
   //根据 两个num的关系 返回相应的数
    //0级是外圈都没有敌人
   //1级是外圈有敌人少于3个然后内圈没有敌人
   //2级是外圈多于等于三个敌人然后内圈没有敌人或者外圈敌人少于3个而且内圈有一个敌人
   //3级是内圈有多个敌人
   Debug.Log(outsideOppNum);
 if(outsideOppNum==0)
   {
      return 0;
   }
   if(outsideOppNum<3&&insideOppNum==0)
   {
      return 1;
   }
   if((outsideOppNum>=3&&insideOppNum==0)||(outsideOppNum<3&&insideOppNum==1))
   {
      return 2;
   }
   return 3;
}
//:寻找支援   因为传球后 有可能支援和控球是同一个人。
public bool FindSupport()
{
   PlayerBase supportPlayer=null;
   float distance=0f;
   foreach(PlayerBase pb in myTeam.OurTeamMembersList)
   {
      if(pb!=myTeam.m_pControllingPlayer)
      {
        if( Mathf.Abs(pb.M_Pos().z-myTeam.OurGoal.m_vCenter.z)>distance)
        {
           distance= Mathf.Abs(pb.M_Pos().z-myTeam.OurGoal.m_vCenter.z);
           supportPlayer=pb;
        }
      }
   }
   if(supportPlayer!=null)
   {

myTeam.AddSupportingPlayer(supportPlayer);
MessageDispatcher.Instance.DispatchMessage(0,ID,supportPlayer.ID,MessageType.PlayerSupportAttacker,null);
return true;
   }
return false;
}



/*关于PlayerController调用的方法*/
public bool BallOutOfViewRange()
{
if((m_pBall.M_Pos()-M_Pos()).magnitude>playerData.ViewLength)
{
   return true;
}
else
{
   return false;
}
}
public bool PlayerControllerFindPassWayFromDirection(float limitDotDomain,float power,Vector3 direction ,out Vector3 targetPos,out PlayerBase target)
{
   IEnumerator<PlayerBase> pbIE=myTeam.OurTeamMembersList.GetEnumerator();
   //设置一个浮点数来记录dot值 如果dot越接近1那么就说明队友离我传球的方向约接近
   float currentDot=-1;
   float currentlimitDot=-1;
   //用来记录找到的最接近direction的接球队员
   PlayerBase receiver=null;
   //这个用来记录是否找到合适力度的传球队员
   bool flag=false;
   target=null;
   targetPos=Vector3.zero;
   //遍历自己队的成员
   while(pbIE.MoveNext())
   {
      if(pbIE.Current!=this)
      {
      float dot=Vector3.Dot( direction,(pbIE.Current.M_Pos()-M_Pos()));
      
      //朝向向量和要的方向向量进行点积
      //limitDotDomain是限制判断角度 比如 0 就是要在方向的前面才找
  if(dot>limitDotDomain)
  {
    if(dot>currentlimitDot)
    {
       if(myTeam.GetBestPassToReceiver(this,pbIE.Current,out targetPos,power))
       {
         target=pbIE.Current;
        flag=true;
        currentlimitDot=dot;
       }
    }
    
    
     
     if(dot>currentDot)
     {
        receiver=pbIE.Current;
        currentDot=dot;
         
     }
     
     
  }
      }
   }
  if(flag)
     {
        
        return true; 
     
     }
     else
     {
        if(receiver!=null)
        {
        target=receiver;
        targetPos=target.M_Pos();
        return true;
        }
        return false;
     }
}



 public override void OnTriggerEnter(Collider collision) {
      base.OnTriggerEnter(collision);
   
    
        
  
 }
 
 public void OnTriggerStay(Collider collision) {
 if(collision.CompareTag("Player"))
 {
    PlayerBase pb=collision.GetComponent<PlayerBase>();
    if((pb.M_Pos()-M_Pos()).magnitude<pb.playerData.BodyRadius+playerData.BodyRadius)
    {
   if(!HitPoints.ContainsKey(pb.ID))
   {
      Vector3 hitpoint=(pb.M_Pos()-M_Pos()).normalized*playerData.BodyRadius+M_Pos();
      Vector3 ToHit=hitpoint-M_Pos();
      float angle=Vector3.SignedAngle(pb.M_vVelocity(),(M_Pos()-pb.M_Pos()),Vector3.up);
      float myAngle=Vector3.SignedAngle(M_vVelocity(),(pb.M_Pos()-M_Pos()),Vector3.up);
      HitPoints.Add(pb.ID,new CollInfo(pb,pb.M_vVelocity(),M_vVelocity(),hitpoint,ToHit,angle,myAngle));
   }
   else
   {
      CollInfo info;
      HitPoints.TryGetValue(pb.ID,out info);
      info.isDeal=false;
      
   }
 }
 else
 {
    if(HitPoints.ContainsKey(pb.ID))
   {
     
      HitPoints.Remove(pb.ID);
   }
 }
 }
 
 }
 
  public override void OnTriggerExit(Collider collision) {
    base.OnTriggerEnter(collision);
   
 }
    }
