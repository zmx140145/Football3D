using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : actor
{
    public float radius;
    private SoccerBall ball;
    private bool  BallInRange=false;
    public Vector3 HitPoint;
    public Vector3 BallVelocity;
    public Vector3 pos;
    public Vector3 BallPos;
  
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
       B_radius=radius;
       ball=SoccerPitch.Instance.m_pBall;
       pos=transform.position;
       pos.y=0f;
       GameList.Instance.ObstaclesList.Add(this);
    }


    // Update is called once per frame
    void Update()
    {
 BallPos=ball.M_Pos();
 BallPos.y=0f;
        //TODO:这个要改成3d的不然球会被击飞
        //然后还要在player那改成二维的追随 不要去追空中目标
        if((BallPos-pos).magnitude<=Radius+ball.Radius)
        {
           HitPoint=(ball.M_Pos()-transform.position).normalized*Radius+transform.position;
            BallVelocity=ball.M_vVelocity();
           
            if(!BallInRange)
            {
                
            
            ball.TouchObstacle=this;
            }
             BallInRange=true;
         
        }
        else
        {
         
           if(BallInRange)
           {
               ball.TouchObstacle=null;
               BallVelocity=Vector3.zero;
               HitPoint=Vector3.zero;
           }
              
            BallInRange=false;
        }
    }
}
