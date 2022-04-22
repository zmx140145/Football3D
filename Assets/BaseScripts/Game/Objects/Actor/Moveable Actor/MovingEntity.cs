using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : actor
{
  protected Vector3 m_Pos;
  protected Vector3 m_vVelocity;
  //一个标准向量，是实体的朝向
  protected Vector3 m_vHeading;
  protected Vector3 m_vSmoothedHeading;
  //垂直于朝向的向量和向右向量的值
  protected Vector3 m_vSide;
  //向上的向量
  protected Vector3 m_vUpward;
  //质量
  protected float m_dMass;
  //最大速度
  protected float m_MaxSpeed;
  //最大的动力
  protected float m_dMaxForce;
  //最大的转向力
  protected float m_dMaxTurnRate;
  //TODO:拥有世界物品的集合
  public Vector3 M_vVelocity()
  {
    return m_vVelocity;
  }
  public void SetVelocity(Vector3 v3)
  
  {
m_vVelocity=v3;
  }
  public void SetMaxSpeed(float speed)
  {
    m_MaxSpeed=speed;
  }
  public Vector3 M_Pos()
  {
    return m_Pos;
  }
  public Vector3 M_vHeading()
  {
    return m_vHeading;
  }
   public Vector3 M_vSmoothedHeading()
  {
    return m_vSmoothedHeading;
  }
  public Vector3 M_vSide()
  {
    return m_vSide;
  }
  public Vector3 M_vUpward()
  {
    return m_vUpward;
  }
  public float M_dMass()
  {
    return m_dMass;
  }
  public float M_MaxSpeed()
  {
    return m_MaxSpeed;
  }
  public float M_dMaxForce()
  {
    return m_dMaxForce;
  }
  public  float M_dMaxTurnRate()
  {
    return m_dMaxTurnRate;
  }
 public virtual void Update() {
   //TODO:problem
   m_Pos=transform.position;
   
 }

}
