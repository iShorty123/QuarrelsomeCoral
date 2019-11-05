﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public float m_Speed;
    private Vector2 m_Direction;
    private Rigidbody2D m_RigidBody;
    private float m_MaxSpeed;
    private int m_AttackDamage;

    // Start is called before the first frame update
    void Start()
    {
        m_MaxSpeed = 30;
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Speed = 15;
        m_Direction = new Vector2(transform.parent.transform.up.x, transform.parent.transform.up.y); //Use parent's orientation to determine bullet direction   
        transform.parent = null; //Break parenting so rotation can occur without effecting bullets
        m_AttackDamage = 25;
        m_RigidBody.velocity = m_Direction * m_Speed + SubmarineManager.GetInstance().m_Submarine.m_RigidBody.velocity; //Initial speed


        Destroy(gameObject, 5); //Despawn bullets after 5 seconds
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(m_RigidBody.velocity.x) < m_MaxSpeed && Mathf.Abs(m_RigidBody.velocity.y) < m_MaxSpeed)
        {
            m_RigidBody.AddForce(m_Direction * m_Speed); //Get faster over time till at max speed
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject); //Destroy bullet

        //Check to see if what we hit can take damage
        ITakeDamage damagable = collision.collider.GetComponent<ITakeDamage>();
        if (damagable != null)
        {
            //If we hit an enemy, deal damage to them
            damagable.TakeDamage(m_AttackDamage);
        }
    }
}