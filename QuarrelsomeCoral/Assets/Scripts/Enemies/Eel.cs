using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eel : BaseEnemy
{

    private float m_SubmarineContactPushBackForce;
    private float m_TimeSinceReadyToLunge;
    private bool m_ReadyToLunge;
    private float m_RestTimeAfterLunge;
    private float m_TimeWhenStunned;
    private float m_TimeWhenStunnedByShield;
    private Vector3 m_ReflectionDirection;
    private Vector3 m_SubmarineDirectionAtImpact;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        m_ReadyToLunge = false;
        m_SubmarineContactPushBackForce = 400;
        m_Health = m_MaxHealth = 75;
        m_AttackDamage = 5; 
        m_AttackSpeed = 1f;
        m_MoveSpeed = 6f;
        m_AttackRange = 20;
        m_ProjectileSpeed = 1500;
        
        m_IsBoss = false;

        m_DistanceToSubmarine = 500; //For the first frame after creation, assume out of range

    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (!CurrentlyLunging()) //&& !CurrentlyStunned()) //Only change direction we look in if not lunging
        {
            LookAtSubmarine();
        }
        if (CurrentlyStunned()) //If currently stunned, look towards the reflection angle
        {
            LookTowardsReflectionAngle();
        }
    }

    private void FixedUpdate()
    {
        m_Rigidbody.angularVelocity = 0; //No spinning allowed
        Move();

    }

    private bool CurrentlyLunging()
    {
        if (Time.realtimeSinceStartup - m_RestTimeAfterLunge > 1) { return false; }
        else { return true; }
    }

    private bool CurrentlyStunned()
    {
        if (Time.realtimeSinceStartup - m_TimeWhenStunned > 1.5) { return false; }
        else { return true; }
    }

    private bool CurrentlyShieldStunned()
    {
        if (Time.realtimeSinceStartup - m_TimeWhenStunnedByShield > 1.5) { return false; }
        else { return true; }
    }


    public override void HitSubmarine(ContactPoint2D _impactSpot)
    {
        m_RestTimeAfterLunge += 1; //if 

        //Reflection Method:

        Debug.Log("hit");

        m_ReflectionDirection = Vector3.Reflect(m_DirectionToSubmarine, _impactSpot.normal);
        m_SubmarineDirectionAtImpact = m_DirectionToSubmarine;
        m_TimeWhenStunned = Time.realtimeSinceStartup;
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.AddForce(m_ReflectionDirection * m_SubmarineContactPushBackForce);

        //Push Back Method:

        //m_TimeWhenStunned = Time.realtimeSinceStartup;
        //m_Rigidbody.velocity = Vector3.zero;
        //m_Rigidbody.AddForce(-m_DirectionToSubmarine * m_SubmarineContactPushBackForce);

        Attack();
    }

    private void LookTowardsReflectionAngle()
    {
        float dot = Vector3.Dot(m_SubmarineDirectionAtImpact, m_ReflectionDirection);
        //if (!(dot < -0.995 && dot > -1.005)) //Did we hit "head on" - if so, don't change rotation
        //{
            float angle = Mathf.Atan2(m_ReflectionDirection.y, m_ReflectionDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), Time.deltaTime * 10);
        //}
    }

    public override void Attack()
    {
        if (Time.realtimeSinceStartup - m_AttackCoolDownTime > m_AttackSpeed)
        {
            m_AttackCoolDownTime = Time.realtimeSinceStartup;
            //Let the sub handle how it takes this damage
            SubmarineManager.GetInstance().m_Submarine.TakeDamage(m_AttackDamage); 
        }
        
    }

    public override void Move()
    {
        if (CurrentlyLunging() || CurrentlyStunned() || CurrentlyShieldStunned()) { return; } //Don't change movement when lunging or stunned from successful attack

        //If inside pursuit range but outside of range and not preparing to lunge
        if (m_DistanceToSubmarine < m_MaxPursuitDistance && m_DistanceToSubmarine > m_AttackRange && !m_ReadyToLunge)
        {
            //Move towards the submarine
            m_Rigidbody.velocity = m_DirectionToSubmarine * m_MoveSpeed;
        }
        else if (m_DistanceToSubmarine <= m_AttackRange && !m_ReadyToLunge) //If inside attack range and not preparing to attack
        {
            //Prepare to lunge
            m_ReadyToLunge = true;
            m_TimeSinceReadyToLunge = Time.realtimeSinceStartup;
            m_Rigidbody.drag = 2; //Come to a stop
        }
        if (m_ReadyToLunge && Time.realtimeSinceStartup - m_TimeSinceReadyToLunge > 1.5) //If been ready to lunge for 1 second
        {
            m_Rigidbody.drag = 1;
            m_Rigidbody.AddForce(m_DirectionToSubmarine * m_ProjectileSpeed);
            m_RestTimeAfterLunge = Time.realtimeSinceStartup;
            m_ReadyToLunge = false;
        }
    }



    public override void TakeDamage(int _damage)
    {
        m_TimeWhenStunned = Time.realtimeSinceStartup; //Put into the stun state upon taking damage
        m_Health -= _damage;
        if (m_Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public override void HitShield(ContactPoint2D _impactSpot)
    {
        //Reflection Method:
        Debug.Log("Hit Shield");
        //m_ReflectionDirection = Vector3.Reflect(m_DirectionToSubmarine, _impactSpot.normal);
        //m_SubmarineDirectionAtImpact = m_DirectionToSubmarine;
        //m_TimeWhenStunned = Time.realtimeSinceStartup;
        //m_Rigidbody.velocity = Vector3.zero;
        //m_Rigidbody.AddForce(m_ReflectionDirection * m_SubmarineContactPushBackForce);

        //Push Back Method:

        m_TimeWhenStunnedByShield = Time.realtimeSinceStartup;
        //m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.AddForce(-m_DirectionToSubmarine * m_ShieldPushBackForce);
        
    }
}
