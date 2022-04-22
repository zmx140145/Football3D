using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    //左下角
   public Vector3 m_vLeftPost;
   //右上角
   public  Vector3 m_vRightPost;
   public Vector3 m_vCenter;
   public Vector3 m_pos;
   public float height;
   //球门朝向向量
   public  Vector3 m_vFacing;
   //进球数
   private int m_iNumGoalsScored;
  private void Start() {
       m_vLeftPost=transform.GetChild(0).transform.position;
        m_vRightPost=transform.GetChild(1).transform.position;
        height= m_vRightPost.y- m_vLeftPost.y;
        m_vCenter=(m_vLeftPost+m_vRightPost)/2;
        m_pos=new Vector3(m_vCenter.x,m_vLeftPost.y,m_vCenter.z);
          m_iNumGoalsScored=0;
        m_vFacing=Vector3.Cross(m_vRightPost-m_vLeftPost,Vector3.up).normalized;
        
   }
   public Goal(Vector3 left,Vector3 right)
   {
       m_vLeftPost=left;
       m_vRightPost=right;
        m_vCenter=(m_vLeftPost+m_vRightPost)/2;
        m_iNumGoalsScored=0;
        m_vFacing=Vector3.Cross(right-left,Vector3.up).normalized;
        
   }
   //TODO:proble！！！
   public bool Scored(SoccerBall ball)
   {
       return true;
   } 
}
