using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Deceleration{slow=3,normal=2,fast=1};
//TODO:x和z轴要全部对调!!!!!!
public class SteeringBehaviors
{
    //Arrive
public int EnterAreaNum=-1;
private Vector3 ArriveTarget;
public Vector3 GetArriveTarget()
{
    return ArriveTarget;
}
private Deceleration deceleration;
public void SetArriveTarget(Vector3 target,Deceleration s)
{
this.ArriveTarget=target;
deceleration=s;
}
//pursuit
private Vehicle PursuitTarget;
public Vehicle GetPursuitTarget()
{
    return PursuitTarget;
}
private Vector3 PursuitOffset;
public void SetPursuitTarget(Vehicle target,Vector3 offset)
{
    PursuitTarget=target;
    PursuitOffset=offset;
}
//Evade
private Vehicle EvadeTarget;
public Vehicle GetEvadeTarget()
{
    return EvadeTarget;
}

public void SetEvadeTarget(Vehicle target)
{
    EvadeTarget=target;
   
}
//Flee
private Vector3 FleeTarget;
public Vector3 GetFleeTarget()
{
    return FleeTarget;
}

public void SetFleeTarget(Vector3 target)
{
    FleeTarget=target;
   
}

 //每秒加在目标的随机位移的最大值
    private float m_dWanderJitter;
    //wander圈的半径
    private float m_dWanderRadius;
    //这是wander圈凸出在智能体前面的范围

      
    private float m_dWanderDistance;
    private Vector3 WanderTarget=Vector3.zero;
    private float wanderUpdateTime=3f;
    public void SetWander(float radius,float distance,float value)
    {
     m_dWanderRadius=radius;
     m_dWanderDistance=distance;
     m_dWanderJitter=value;
    
    }

//bool
public bool seek=false;
public bool arrive=false;

public bool flee=false;

public bool pursuit=false;

public bool evade=false;

public bool wander=false;
public bool wanderTogether=false;
public bool obstacleAvoidance=false;
public bool wallAvoidance=false;
public bool offsetPursuit=false;
    //所有方法的bool
  
  
    //用于判断是否开启这个方法
  

    
 
    private Vector3 m_dWanderTarget;
    private Vector3 m_dWanderTogetherTarget;
    public WorldObject_SO So=WorldObject.Instance.worldObject_SO;
    void Awake()
    {
        
    }
   public  SteeringBehaviors(Vehicle vehicle)
    {
m_Vehicle=vehicle;

    }
   private Vehicle m_Vehicle;
   //具体方法
   //确保无重叠
public Vector3 EnforceNonPenetrationConstraint<conT,T>(conT entity,List<T> ContainerOfEntities) where T:Vehicle where conT:Vehicle
{
 if(ContainerOfEntities.Count>0)
     {
     //迭代所有的实体，检查范围
     IEnumerator<T> curEntity=ContainerOfEntities.GetEnumerator();
     Vector3 SteerForce=Vector3.zero;
    
       while(curEntity.MoveNext())
     {
     //检测不是自己
     if(curEntity!=entity)
     {
     //计算实体间的距离
     Vector3 ToEntity=entity.M_Pos()-curEntity.Current.M_Pos();
     float DistFromEachOther=ToEntity.magnitude;
     //如果这个距离小于他们的半径的总和
     //那么这个实体要沿ToEntity向量方向移动
     float AmountOfOverLap=curEntity.Current.Radius+entity.Radius-DistFromEachOther;
     if(AmountOfOverLap>=0)
     {
         //按重叠的距离大小移开实体
         if(DistFromEachOther>0)
        SteerForce=(ToEntity.normalized)*(AmountOfOverLap);
      return SteerForce;
     }
     }
     } //下一个实体
    
     }
     return Vector3.zero;
}

public void InitBool()
{
   
       arrive=false;
        seek=false;
        wander=false;
        pursuit=false;
       wanderTogether=false;
}
   //靠近
  public  Vector3 Seek(Vector3 TargetPos)
  {
      
      Vector3 DesiredVelocity=Vector3.Normalize(TargetPos-m_Vehicle.M_Pos())*m_Vehicle.M_MaxSpeed();
      return DesiredVelocity-m_Vehicle.M_vVelocity();
  }
  //离开
  public  Vector3 Flee(Vector3 TargetPos)
  {
   const float PanicDistanceSq=50f;
   if((m_Vehicle.M_Pos()-TargetPos).sqrMagnitude>PanicDistanceSq)
   {
       return new Vector3(0,0,0);
   }
   Vector3 DesiredVelocity=Vector3.Normalize(m_Vehicle.M_Pos()-TargetPos)*m_Vehicle.M_MaxSpeed();
   return(DesiredVelocity-m_Vehicle.M_vVelocity());
  }
  //抵达
  public   Vector3 Arrive(Vector3 TargetPos,Deceleration deceleration)
  {
    
  Vector3 ToTarget=TargetPos-m_Vehicle.M_Pos();
  //计算到目标的距离
  float dist=ToTarget.magnitude;
  if(dist>0f)
  {
      //因为枚举Deceleration是整数int，所以需要调整减速度
      const float DecelerationTweaker=0.3f;
      float speed=dist/((float)deceleration*DecelerationTweaker);
      speed=Mathf.Min(speed,m_Vehicle.M_MaxSpeed());
      Vector3 DesiredVelocity=ToTarget*speed/dist;
      return (DesiredVelocity-m_Vehicle.M_vVelocity());
  }
  return Vector3.zero;
  }
  public Vector3 Pursuit(Vehicle evader)
  {
     
//如果逃避着在前面，而且面对着智能体
//那么就靠近当前位置
Vector3 ToEvader=evader.M_Pos()-m_Vehicle.M_Pos();
float RelativeHeading=Vector3.Dot(m_Vehicle.M_vSmoothedHeading(),evader.M_vSmoothedHeading());
//两个人的几乎对着 而且追的人的朝向正对着被追的人
if((Vector3.Dot(ToEvader,m_Vehicle.M_vSmoothedHeading())>0)&&(RelativeHeading<-0.95f))
{
return Seek(evader.M_Pos());
} 
//预判逃避着的位置
//预测的时间正比于逃避者和追随者的距离，反比于智能体的速度和？？？
float LookAheadTime=ToEvader.magnitude/(m_Vehicle.M_MaxSpeed()+evader.M_vVelocity().magnitude);
LookAheadTime+=TurnaroundTime(m_Vehicle,evader.M_Pos());
return Seek(evader.M_Pos()+evader.M_vVelocity()*LookAheadTime);
  }
//计算转向时间
float TurnaroundTime(Vehicle pAgnet,Vector3 TargetPos)
{
    Vector3 toTarget=Vector3.Normalize(TargetPos-pAgnet.M_Pos());
    float dot=Vector3.Dot(pAgnet.M_vSmoothedHeading(),toTarget);
    //此值与转弯率有关
    //如果自己朝着目标的反方向，那么0.5的值就能返回1秒的转弯时间
    const float coefficent=0.5f;
    //点积<0那么目标在后面
    return(dot-1.0f)*-coefficent;
}

//逃避
public Vector3 Evade(Vehicle Pursuer)
{
    //不用检测朝向
    Vector3 ToPursuer=Pursuer.M_Pos()-m_Vehicle.M_Pos();
    //TODO:problem
    float LookAheadTime=ToPursuer.magnitude/(m_Vehicle.M_MaxSpeed()+Pursuer.M_MaxSpeed());
    return Flee(Pursuer.M_Pos()+Pursuer.M_vVelocity()*LookAheadTime);
}
//徘徊
public Vector3 Wander()
{
if(wanderUpdateTime<0)
{
   //取随机数
   m_dWanderTarget=new Vector3(Random.Range(-1,1)*m_dWanderJitter,0,Random.Range(-1,1)*m_dWanderJitter);
   m_dWanderTarget.Normalize();
   m_dWanderTarget*=m_dWanderRadius;
   //TODO:problem
   Vector3 targetLocal=m_dWanderTarget+new Vector3(0,0,m_dWanderDistance);
   Vector3 targetWorld=m_Vehicle.transform.TransformPoint(targetLocal);
   WanderTarget=targetWorld;
   wanderUpdateTime=3.0f;
}
else
{
    wanderUpdateTime-=Time.deltaTime;
}
   return Arrive(WanderTarget,Deceleration.normal);
   
}
//协同的漫游
public Vector3 WanderTogether()
{
Region rg;
Vector3 targetWorld=m_Vehicle.M_Pos();
if(SoccerPitch.Instance.m_RegionsList.TryGetValue(EnterAreaNum,out rg))
{
  targetWorld=WorldSelfTransform.Instance.SelfToWorld(rg.areaPos,Vector3.forward,Vector3.right,Vector3.up,WorldObject.Instance.WanderTogetherTargetLocal);
 
}

   return Arrive(targetWorld,Deceleration.normal);
   
}
//避开障碍
public Vector3 ObstacleAvoidance()
{
 
float m_dDViewLength= So.DetectionBoxLength+(m_Vehicle.M_vVelocity().magnitude/m_Vehicle.M_MaxSpeed())*So.DetectionBoxLength;
m_Vehicle.TagWithInViewRange<Vehicle,BaseGameEntity>(m_Vehicle,GameList.Instance.ObstaclesList,m_dDViewLength,So.DetectionViewAngle);
//跟踪最近的相交的障碍物
 BaseGameEntity ClosestIntersectingObstacle=null;
 //跟踪CIB的距离
    float DistToClosestIP=float.MaxValue;
    //转换为本地坐标
    Vector3 LocalPosOfClostestObstacle=Vector3.zero;
if(GameList.Instance.ObstaclesList.Count>0)
{
    IEnumerator<BaseGameEntity> ListIE=GameList.Instance.ObstaclesList.GetEnumerator();

    while(ListIE.MoveNext())
   {
       
    if(ListIE.Current.m_Tag)
    {
        
          Vector3 LocalPos=m_Vehicle.transform.InverseTransformPoint(ListIE.Current.transform.position);
           //局部坐标x小于0说明在物体后面可以排除
        if(LocalPos.z>=0f)
        {
          float ExpandedRadius=ListIE.Current.Radius+m_Vehicle.Radius;
          //radius要和collider的size保持一致
          //保证一定相交
          if(LocalPos.x<ExpandedRadius)
          {
              //圆周相交的测试
              float cX=LocalPos.x;
              float cZ=LocalPos.z;
              float SqrtPart=Mathf.Sqrt(ExpandedRadius*ExpandedRadius-cX*cX);
              float ip;
              //保证结果点在x轴前
             if(SqrtPart>cZ)
             {
             ip=cZ+SqrtPart;
             }
             else
             {
             ip=cZ-SqrtPart;
             }
             //如果是目前最短的就记录下来
             if(ip<DistToClosestIP)
             {
                 DistToClosestIP=ip;
                 ClosestIntersectingObstacle=ListIE.Current;
                 LocalPosOfClostestObstacle=LocalPos;
             }
          }
        }
    }
   
   }
Vector3 SteeringForce=Vector3.zero;
    if(ClosestIntersectingObstacle)
    {
          //智能体离物体越近，操控力越强
        float multiplier=1.0f+(m_dDViewLength-LocalPosOfClostestObstacle.z)/m_dDViewLength;
        //侧向力
        SteeringForce.x=-(Mathf.Sign(LocalPosOfClostestObstacle.x)*ClosestIntersectingObstacle.Radius-LocalPosOfClostestObstacle.x)*multiplier;
        //施加制动力，正比于障碍物到物体的距离
        const float BreakingWeight=0.1f;
        SteeringForce.z=(ClosestIntersectingObstacle.Radius-LocalPosOfClostestObstacle.z)*BreakingWeight;
         Debug.Log("SteeringForce.x");
        Debug.Log(SteeringForce.x);
        Debug.Log("SteeringForce.z");
        Debug.Log(SteeringForce.z);
        Debug.Log("位置");
        Debug.Log(m_Vehicle.M_Pos());
    }
    else
    {
        return Vector3.zero;
    }
   
  return m_Vehicle.transform.TransformPoint(SteeringForce);
}
 else
    {
        return Vector3.zero;
    }
   
//     if(m_Vehicle.obstaclesHashSet.Count>0)
//     {
//     foreach(var item in m_Vehicle.obstaclesHashSet)
//     {
        
//         Vector3 LocalPos=m_Vehicle.transform.InverseTransformPoint(item.transform.position);
//         //局部坐标x小于0说明在物体后面可以排除
//         if(LocalPos.z>=0)
//         {
//           float ExpandedRadius=item.Radius+m_Vehicle.Radius;
//           //radius要和collider的size保持一致
//           if(LocalPos.x<ExpandedRadius)
//           {
//               //圆周相交的测试
//               float cX=LocalPos.z;
//               float cZ=LocalPos.x;
//               float SqrtPart=Mathf.Sqrt(ExpandedRadius*ExpandedRadius-cZ*cZ);
//               float ip;
//               //保证结果点在x轴前
//              if(SqrtPart>cX)
//              {
//              ip=cX+SqrtPart;
//              }
//              else
//              {
//              ip=cX-SqrtPart;
//              }
//              if(ip<DistToClosestIP)
//              {
//                  DistToClosestIP=ip;
//                  ClosestIntersectingObstacle=item;
//                  LocalPosOfClostestObstacle=LocalPos;
//              }
//           }
//         }
//     }
    
//     Vector3 SteeringForce=Vector3.zero;
//     if(ClosestIntersectingObstacle)
//     {
//         //智能体离物体越近，操控力越强
//         float multiplier=1.0f+(m_Vehicle.boxCollider.size.x-LocalPosOfClostestObstacle.x)/m_Vehicle.boxCollider.size.x;
//         //侧向力
//         SteeringForce.x=(ClosestIntersectingObstacle.Radius-LocalPosOfClostestObstacle.y)*multiplier;
//         //施加制动力，正比于障碍物到物体的距离
//         const float BreakingWeight=0.2f;
//         SteeringForce.z=(ClosestIntersectingObstacle.Radius-LocalPosOfClostestObstacle.x)*BreakingWeight;
//     }
   
// return m_Vehicle.transform.TransformPoint(SteeringForce);
//     }
    // else
    // {
    //     return Vector3.zero;
    // }
}
//躲墙
public Vector3 WallAvoidance()
{
  
    Vector3 SteerForce=Vector3.zero;
    //
    RaycastHit HitInfo;
    float DisancePerview=1f*m_Vehicle.M_vVelocity().magnitude+1f;
    //发射正方体射线
    //1
  Physics.BoxCast(m_Vehicle.M_Pos(),m_Vehicle.transform.localScale/2,m_Vehicle.M_vSmoothedHeading(),out HitInfo,m_Vehicle.transform.rotation,DisancePerview,LayerMask.GetMask("Walls"));
  if(HitInfo.collider!=null)
  {
      float OverShoot=m_Vehicle.M_MaxSpeed()*DisancePerview/m_Vehicle.M_vVelocity().magnitude-HitInfo.distance;
      //墙的向里面的法向
   SteerForce=HitInfo.collider.gameObject.GetComponent<Wall>().ToMeNormal(m_Vehicle.transform)*OverShoot;
   return SteerForce;
  }

 
  return SteerForce;
}
//插入
public Vector3 Interpose(Vehicle AgentA,Vehicle AgentB)
{
    //首先，我们需要算出在未来世界T时，这个两个智能体的位置
    //交通工具以最大速度到达中点所花的时间近似于T
    Vector3 MidPoint=(AgentA.M_Pos()+AgentB.M_Pos())*1/2;
    float TimeToReachMidPoint=Vector3.Distance(m_Vehicle.M_Pos(),MidPoint)/m_Vehicle.M_MaxSpeed();
    //现在有了T,假设智能体A和智能体B将继续直线行驶
    //判断得到他们的预期位置
    Vector3 APos=AgentA.M_Pos()+AgentA.M_vVelocity()*TimeToReachMidPoint;
      Vector3 BPos=AgentB.M_Pos()+AgentA.M_vVelocity()*TimeToReachMidPoint;
      MidPoint=(APos+BPos)/2f;
      //到达那里
      return Arrive(MidPoint,Deceleration.fast);

} 
//隐藏
Vector3 GetHidingPostion(Vector3 posOb,float radiusOb,Vector3 posTarget)
{
   const float DistanceFromBoundary=30f;
   float DistAway=radiusOb+DistanceFromBoundary;
   Vector3 ToOb=Vector3.Normalize(posOb-posTarget);
   return (ToOb*DistAway)+posOb;
}
public Vector3 Hide(Vehicle target)
{
    float DistToClost=float.MaxValue;
    //检测Obstacles
     
    Vector3 BestHidingSpot=Vector3.zero;
    Collider[] hits=Physics.OverlapBox(m_Vehicle.M_Pos(),m_Vehicle.transform.localScale*2f,m_Vehicle.transform.rotation,LayerMask.GetMask("Obstacles"));
     
    foreach(var collider in hits)
    {
        //计算这个障碍物的隐藏点
        Vector3 HidingSpot=GetHidingPostion(collider.transform.position,collider.GetComponent<BaseGameEntity>().Radius,target.M_Pos());
        //用距离找到离智能体最近的的隐藏点
        float dist=Vector3.Distance(HidingSpot,m_Vehicle.M_Pos());
        if(dist<DistToClost)
        {
            DistToClost=dist;
            BestHidingSpot=HidingSpot;
        }
    }
    if(DistToClost==float.MaxValue)
    {
        return Evade(target);
    }
    else
    {
         return Arrive(BestHidingSpot,Deceleration.fast);
    }
}
 
  public void AddPath(BaseGameEntity point)
  {
m_Vehicle.PathList.Add(point);
  }
  public void DelPath(BaseGameEntity point)
  {
m_Vehicle.PathList.Remove(point);
  }
  public void ClearPath(BaseGameEntity point)
  {
      m_Vehicle.PathList.Clear();
  }
  public Vector3 FollowingPath()
  {
      float WayPointSeekDistance=2f;
       if(m_Vehicle.PathList.Count==0)
      {
        return Vector3.zero;
      }

      if(Vector3.Distance(m_Vehicle.M_Pos(),m_Vehicle.currentPath.Current.transform.position)<WayPointSeekDistance)
      {
          //判断是不是指向最后一个元素
      if(m_Vehicle.currentPath.Current!=m_Vehicle.PathList[m_Vehicle.PathList.Count-1])
          {
              //不是就向下一个移动
          m_Vehicle.currentPath.MoveNext();
          }
          else
          {
              //是就重新指向头元素
             //TODO:清空原来的赋值新的
              m_Vehicle.currentPath=m_Vehicle.PathList.GetEnumerator();
              m_Vehicle.currentPath.MoveNext();
          }
      }
      //TODO:needdo
      //不是最后一个
      if(m_Vehicle.currentPath.Current!=m_Vehicle.PathList[m_Vehicle.PathList.Count-1])
      {
      return Seek(m_Vehicle.currentPath.Current.transform.position);
      }
      //是最后一个
      else
      {
          return Arrive(m_Vehicle.currentPath.Current.transform.position,Deceleration.normal);
      }
  }


  //保持一定偏移的追逐
  public Vector3 OffsetPursuit(Vehicle leader,Vector3 offset)
  {
      Vector3 WorldOffsetPos=leader.transform.TransformPoint(offset);
      Vector3 ToOffset=WorldOffsetPos-m_Vehicle.M_Pos();
      //预测的时间正比于领队与追随者的距离
      //反比于两个智能体的速度之和
      float  LookAheadTime=ToOffset.magnitude/(m_Vehicle.M_MaxSpeed()+leader.M_vVelocity().magnitude);
      //到达偏移的位置
      return Arrive(WorldOffsetPos+leader.M_vVelocity()*LookAheadTime,Deceleration.fast);
  }
                 //组行为
//
//
//
public  Vector3 Separation(List<Vehicle> neighbors)
{
    //需要个静态的全局变量来存放所以的vehicle
    //然后给这个函数做形参
    Vector3 SteeringForce=Vector3.zero;
    for(int a=0;a<neighbors.Count;a++)
    {
        //确保计算中没有包含这个智能体
        //确保正在被检查的智能体足够近
        if((neighbors[a]!=m_Vehicle)&&neighbors[a].m_Tag)
        {
            Vector3 ToAgent=m_Vehicle.M_Pos()-neighbors[a].M_Pos();
            //力的大小反比于智能体到它邻居的距离
            SteeringForce+=Vector3.Normalize(ToAgent)/ToAgent.magnitude;
        }
    }
    return SteeringForce;
}
public Vector3 Alignment(List<Vehicle> neighbors)
{
    //记录neighbors的朝向的平均值
    Vector3 AverageHeading=Vector3.zero;
    //用来计数邻近的交通工具数目
    int NeighborCount=0;
    //迭代所有标记的交通工具，计算朝向向量的总和
    for(int a=0;a<neighbors.Count;a++)
    {
        //确保计算中没有包含这个智能体
         //确保正在被检查的智能体足够近
          if((neighbors[a]!=m_Vehicle)&&neighbors[a].m_Tag)
        {
            AverageHeading+=neighbors[a].M_vSmoothedHeading();
            ++NeighborCount;
            
        }
    }
    if(NeighborCount>0)
    {
        AverageHeading/=(float)NeighborCount;
        AverageHeading-=m_Vehicle.M_vSmoothedHeading();
    }
    return AverageHeading;

}

//聚集
public Vector3 Cohesion(List<Vehicle> neighbors)
{
     //找到所有智能体的质心
    Vector3 CenterOfMass=Vector3.zero;
    Vector3 SteerForce=Vector3.zero;
    //用来计数邻近的交通工具数目
    int NeighborCount=0;
    //迭代所有标记的交通工具，计算朝向向量的总和
    for(int a=0;a<neighbors.Count;a++)
    {
        //确保计算中没有包含这个智能体
         //确保正在被检查的智能体足够近
          if((neighbors[a]!=m_Vehicle)&&neighbors[a].m_Tag)
        {
          CenterOfMass+=neighbors[a].M_Pos();
            ++NeighborCount;
            
        }
    }
    if(NeighborCount>0)
    {
        //计算质心平均值
        CenterOfMass/=(float)NeighborCount;
        //靠近质心位置
        SteerForce=Seek(CenterOfMass);
    }
    return SteerForce;
}
//群集
//计算所有力
public bool On(bool func)
{
    return func;
}
public bool AccumulateForce(ref Vector3 Target,Vector3 AddToTarget)
{
    float HadUsedForce=Target.magnitude;
    float NoUsedForce=m_Vehicle.M_dMaxForce()-HadUsedForce;
if(NoUsedForce<=0)
{
return false;
}
float ForceWantToAdd=AddToTarget.magnitude;
//如果没有加满直接加进去
//如果剩余的可以加的力小于要加的力那么把要加的力截断
if(ForceWantToAdd<NoUsedForce)
{
    Target+=AddToTarget;
}
else
{
    Target+=AddToTarget.normalized*NoUsedForce;
}
return true;
}
     public  Vector3 Calculate()
     {
         
        
         //采用带优先级的加权
       Vector3 SteeringForce=Vector3.zero;
       Vector3 resultForce=Vector3.zero;
       //躲墙
       
       
       if(wallAvoidance)
       {
             Debug.Log("wall");
           resultForce=WallAvoidance()*So.Mult_WallAvoidance;
           if(!AccumulateForce(ref SteeringForce,resultForce)) return SteeringForce;
       }
       //躲开障碍物
       if(obstacleAvoidance)
       {
            Debug.Log("obstacle");
           resultForce=ObstacleAvoidance()*So.Mult_ObstacleAvoidance*So.Mult_ObstacleAvoidance;
           if(!AccumulateForce(ref SteeringForce,resultForce)) return SteeringForce;
       }
       //躲避追逐
       if(EvadeTarget!=null&&evade)
       {
            Debug.Log("evade");
        resultForce=Evade(EvadeTarget)*So.Mult_Evade;
           if(!AccumulateForce(ref SteeringForce,resultForce))return SteeringForce;
       }
       //逃离
       if(FleeTarget!=Vector3.zero&&flee)
       {
            Debug.Log("flee");
           resultForce=Flee(FleeTarget)*So.Mult_Flee;
           if(!AccumulateForce(ref SteeringForce,resultForce))return SteeringForce;
       }
       //带偏移的追逐
       if(PursuitTarget!=null&&offsetPursuit)
       {
            Debug.Log("offsetPursuit");
           resultForce=OffsetPursuit(PursuitTarget,PursuitOffset)*So.Mult_Pursuit;
           if(!AccumulateForce(ref SteeringForce,resultForce))return SteeringForce;
       }
       //正常的追逐和带偏移的追逐二选一
         if(PursuitTarget!=null&&pursuit)
       {
            Debug.Log("pursuit");
           resultForce=Pursuit(PursuitTarget)*So.Mult_Pursuit;
           if(!AccumulateForce(ref SteeringForce,resultForce))return SteeringForce;
       }
       //到达某处 优先级低于追逐
       if(ArriveTarget!=Vector3.zero&&arrive)
       {
            Debug.Log("Arrive");
           resultForce=Arrive(ArriveTarget,deceleration)*So.Mult_Arrive;
            if(!AccumulateForce(ref SteeringForce,resultForce))return SteeringForce;
       }
         if(ArriveTarget!=Vector3.zero&&seek)
       {
            Debug.Log("Seek");
           resultForce=Seek(ArriveTarget)*So.Mult_Seek;
            if(!AccumulateForce(ref SteeringForce,resultForce))return SteeringForce;
       }

       if(wander&&!wanderTogether)
       {
          
           
           resultForce=Wander()*So.Mult_Wander;
        
             if(!AccumulateForce(ref SteeringForce,resultForce))return SteeringForce;
       }
     if(wanderTogether&&!wander)
       {
            
           
           resultForce=WanderTogether()*So.Mult_Wander;
        //    Debug.Log(resultForce);
        
             if(!AccumulateForce(ref SteeringForce,resultForce))return SteeringForce;
       }
    
         return SteeringForce ;
     }
}
