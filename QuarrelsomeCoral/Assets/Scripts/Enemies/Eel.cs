﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Eel : BaseEnemy
{

    private float m_SubmarineContactPushBackForce;
    private float m_TimeSinceReadyToLunge;
    private bool m_ReadyToLunge;
    private float m_RestTimeAfterLunge;
    private float m_TimeWhenStunned;
    private Vector3 m_ReflectionDirection;
    private Vector3 m_SubmarineDirectionAtImpact;
    private Animator m_Animator;
    private bool m_CanLunge;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        m_ReadyToLunge = false;
        m_SubmarineContactPushBackForce = 400;
        m_Health = m_MaxHealth = 50;
        m_AttackDamage = 3;
        m_AttackSpeed = 1f;
        m_MoveSpeed = 6f;
        m_AttackRange = 20;
        m_ProjectileSpeed = 1500;
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("Hit", false);
        if (m_IsBoss)
        {
            m_Rigidbody.mass = 2;
            m_Health = m_MaxHealth = m_Health * 3;
            m_AttackDamage = m_AttackDamage * 2;
            m_MoveSpeed = m_MoveSpeed * 3;
            m_SubmarineContactPushBackForce = m_SubmarineContactPushBackForce * 2;
            m_ProjectileSpeed = m_ProjectileSpeed * 2;

        }

        m_DistanceToSubmarine = 500; //For the first frame after creation, assume out of range

    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (!CurrentlyLunging()) //&& !CurrentlyStunned()) //Only change direction we look in if not lunging
        {
            m_CanLunge = true;
            LookAtPosition(m_DirectionToSubmarine);
        }
        if (CurrentlyStunned()) //If currently stunned, look towards the reflection angle
        {
            LookTowardsReflectionAngle();
        }
        if (CurrentlyShieldStunned())
        {
            LookAtPosition(m_DirectionToSubmarine);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private bool CurrentlyLunging()
    {
        if (Time.realtimeSinceStartup - m_RestTimeAfterLunge > 1) { m_Animator.speed = 1; return false; }
        else { if (!CurrentlyShieldStunned() && m_CanLunge) { m_Animator.speed = 3; } else { m_Animator.speed = 1; } return true; }
    }

    private bool CurrentlyStunned()
    {
        if (Time.realtimeSinceStartup - m_TimeWhenStunned > 1.5) { return false; }
        else
        { return true; }
    }

    private bool CurrentlyShieldStunned()
    {
        if (Time.realtimeSinceStartup - m_TimeWhenStunnedByShield > 1.5) { return false; }
        else { m_Animator.speed = 1; return true; }
    }


    public override void HitSubmarine(ContactPoint2D _impactSpot)
    {

        // m_RestTimeAfterLunge
        m_RestTimeAfterLunge += 1; //if 

        //Reflection Method:
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
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle + 90, Vector3.forward), Time.deltaTime * 10);
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
        //m_TimeWhenStunned = Time.realtimeSinceStartup; //Put into the stun state upon taking damage
        m_Health -= _damage;
        if (m_Health <= 0)
        {
            Destroy(gameObject);
        }
        StartCoroutine(HitAnimationTimer());
    }

    private IEnumerator HitAnimationTimer()
    {
        m_Animator.SetBool("Hit", true);
        yield return new WaitForSeconds(1);
        m_Animator.SetBool("Hit", false);
    }

    protected override void TransformIntoBoss()
    {
        m_IsBoss = true;
        m_CanDestroyTerrainWithTouch = true;
    }

    new protected void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        m_CanLunge = false;
        Tilemap map = collision.collider.GetComponent<Tilemap>();
        if (map != null)
        {
            m_RestTimeAfterLunge -= 1;
        }
    }

    new protected void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);
        m_CanLunge = false;
        Tilemap map = collision.collider.GetComponent<Tilemap>();
        if (map != null)
        {
            m_RestTimeAfterLunge -= 1;
        }
    }

    protected override void LookAtPosition(Vector3 _position)
    {
        float angle = Mathf.Atan2(_position.y, _position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle + 90, Vector3.forward), Time.deltaTime * m_LookAtSpeed);
    }
}