using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBall : Vehicle
{
    //用于判断球是否出了场外
    //出场后只更新一次速度 然后直达回到场后才会
    private bool OutOfRangeAndVelocityIsReverse=false;
    private bool TouchWithObstacleAndVelocityIsReverse=false;
   //用这个有无来判断是否与障碍物接触   由障碍物进行绑定操作
    public Obstacle TouchObstacle=null;
    //上一次更新的位置
  private Vector3 m_vOldPos;
  private float BallFirction;
  //持球队员
  public PlayerBase m_pOwner;
  //组成足球场边界的墙的本地引用
  private List<Wall> m_PitchBoundary;
  
  //球的直径
  public float BallSize;
  public List<Wall> PitchBoundary;
  public float BallMass;
  //测试球是否和墙碰撞，相应地把速度反向
  private void TestCollisionWithWalls(List<Wall> walls)
  {

  }
public override void Start()
{
    base.Start();
    B_radius=BallSize/2;
    m_Pos=transform.position;
    m_vVelocity=Vector3.zero;
    m_dMass=BallMass;
    m_dMaxForce=0f;
    m_dMaxTurnRate=0f;
    m_MaxSpeed=1000f;
    m_pOwner=null;
    m_PitchBoundary=PitchBoundary;
    BallFirction=WorldObject.Instance.worldObject_SO.BallFirction;

}
public override void Update() {
    base.Update();
    BallUpdate(Time.deltaTime);
    m_vVelocity.y=0f;
    m_Pos.y=0f;
}
public void BallUpdate(float time_elapsed)
{
    JudgeOwner();
  OutOfPitchInverseForceCalculate(); 
  ReboundWhenTouchObstacle();
  if(m_vVelocity.magnitude>0.00001f)
         {
            if(smoothedTime>0)
            {
           m_vHeading+=m_vVelocity.normalized;
           m_vHeading.Normalize();
           smoothedTime-=Time.deltaTime;
            }
            else
            {
            m_vHeading+=m_vVelocity.normalized;
           m_vHeading.Normalize();
           m_vSmoothedHeading=m_vHeading;
           smoothedTime=0.01f;
            }




         }
 Vector3 acceleration=m_vVelocity.normalized*BallFirction/m_dMass;
 if(m_vVelocity!=Vector3.zero)
 {
      m_vVelocity-=acceleration*time_elapsed;
 }
  m_vVelocity=Vector3.ClampMagnitude(m_vVelocity,m_MaxSpeed);
      //执行动作
    transform.position+=m_vVelocity*time_elapsed;
   transform.forward=m_vSmoothedHeading;
   Vector3.OrthoNormalize(ref m_vSmoothedHeading,ref m_vUpward,ref m_vSide);    
    
}
//这个方法可以使球在碰到边界时反弹回来
public void OutOfPitchInverseForceCalculate()
{
     if(!SoccerPitch.Instance.IsInPitch(M_Pos()))
    {
        Vector3 ToConer=SoccerPitch.Instance.PitchCenterPoint().normalized;
        Vector3 ToPitch;
        ToPitch=(SoccerPitch.Instance.PitchCenterPoint()-M_Pos()).normalized;
        if(!OutOfRangeAndVelocityIsReverse)
        {
        
     
     if(Mathf.Abs(ToPitch.x)>Mathf.Abs(ToConer.x))
     {
         m_vVelocity.x=-m_vVelocity.x;
     }
     else
     {
         if(ToPitch==ToConer)
         {
             m_vVelocity=-m_vVelocity;
         }
         else
         {
 m_vVelocity.z=-m_vVelocity.z;
         }
        
     }
     OutOfRangeAndVelocityIsReverse=true;
    }
    else
    {
        m_vVelocity+=ToPitch;
    }
    }
    else
    {
        
        OutOfRangeAndVelocityIsReverse=false;
    }
}
//碰到障碍物偏转
public void ReboundWhenTouchObstacle()
{
     if(TouchObstacle!=null)
    {
        Debug.Log("reverse");
         Vector3 c=(TouchObstacle.HitPoint-TouchObstacle.transform.position).normalized;
         Vector3 a=TouchObstacle.BallVelocity.normalized;
         float Angle=Vector3.SignedAngle(-a,c,Vector3.up);
         Vector3 b=2f*Mathf.Abs(Mathf.Cos(Angle))*c+a;
         b=b.normalized;
        
         m_vVelocity=b*TouchObstacle.BallVelocity.magnitude;
         
         
        
    }
   
}
//这方法对球施加一个有方向的力
public void Kick(Vector3 direction,float force)
{
m_vVelocity=direction.normalized*force;
}


//给定一个踢球力和通过起点和终点定义的移动距离
//求出球移动需要的时间
//默认踢球时触球时间为1s
public float TimeToCoverDistance(Vector3 from,Vector3 to, float force)
{
    //如果队员传球了，那么这将是球下一步的速度
    float speed=force/m_dMass;
    //使用公式计算b处的速度
    //v^2=u^2+2ax;
    float DistanceToCover=Vector3.Distance(from,to);
    float term=speed*speed+2f*DistanceToCover*BallFirction/m_dMass;
    //如果（u^2+2ax）是负数，那么就无法到达目标点
    if(term<=0) return -1.0f;
    float v=Mathf.Sqrt(term);
    return (v-speed)/BallFirction/m_dMass;
}

//TODO:这里有个futruepostion的方法可以去修改 
//计算给定时间后球的位置
public Vector3 FuturePosition(float time)
{
    //使用公式 x=ut+1/2at^2,其中x为距离，a为摩擦加速度，u为初始初度
    //计算ut项，这是个向量
    Vector3 ut =m_vVelocity*time;
    //计算1/2at^2
    float half_a_t_squared=0.5f*BallFirction*time*time;
    Vector3 ScalarToVector=half_a_t_squared*m_vVelocity.normalized;
    //返回预测位置为球的位置加上这两项
    return m_Pos+ut+ScalarToVector;

}
//这被场上队员用于停球，停球队员认为控制了球
//所以相应调整m_pOwner
public void Trap(PlayerBase Owner)
{
m_vVelocity=Vector3.zero;
m_pOwner=Owner;
}
//判断是否owner已经不能控制球
public bool OwnerOutOfControll()
{
    if((M_Pos()-m_pOwner.M_Pos()).magnitude<WorldObject.Instance.worldObject_SO.BallOwnerRange)
    {
        return false;
    }
    else
    {
       return true;  
    }
}
public void JudgeOwner()
{
    if(m_pOwner!=null)
    {
if(OwnerOutOfControll())
    {
        m_pOwner=null;
    }
    }
    
}
public Vector3 OldPos()
{
    return m_vOldPos;
}
//设置球的位置，并重设速度为0
public void PlaceAtPosition(Vector3 NewPos)
{
    transform.position=NewPos;
    m_vVelocity=Vector3.zero;
}
}
