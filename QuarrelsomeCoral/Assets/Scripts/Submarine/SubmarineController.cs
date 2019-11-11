﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineController : MonoBehaviour, ITakeDamage
{
    public Rigidbody2D m_RigidBody;
    public float m_Speed;
    public int m_Health;
    public int m_MaxHealth;

    private bool m_HasPilot;
    private string m_HorizontalControls;
    private string m_VerticalControls;

    private float m_MaxSpeed;
    private float m_MaxY = 92f;

    // Start is called before the first frame update
    void Start()
    {
        m_MaxSpeed = 5;
        m_MaxHealth = m_Health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_RigidBody.position; //Move sub via the SubmarineRigibody object
    }

    private void FixedUpdate()
    {
        if (m_HasPilot)
        {
            MoveSubmarine();
        }
    }

    private void MoveSubmarine()
    {

        if (m_RigidBody.transform.position.y > m_MaxY) {
            Vector3 vel = m_RigidBody.velocity;
            vel.y = 0;
            m_RigidBody.velocity = vel;
        }

        float moveHorizontally = Input.GetAxis(m_HorizontalControls);
        float moveVertically = Input.GetAxis(m_VerticalControls);
        //Move submarine via MovePosition(world position translation)

        if (moveHorizontally < 0 && m_RigidBody.velocity.x > -m_MaxSpeed) //If want to go left and can go left
        {
            m_RigidBody.AddForce(new Vector2(moveHorizontally * m_Speed, 0)); //Move left
        }
        else if (moveHorizontally > 0 && m_RigidBody.velocity.x < m_MaxSpeed) //If want to go right and can go right
        {
            m_RigidBody.AddForce(new Vector2(moveHorizontally * m_Speed, 0));
        }

        if (moveVertically < 0 && m_RigidBody.velocity.y > -m_MaxSpeed) //If want to go down and can go down
        {
            m_RigidBody.AddForce(new Vector2(0, (moveVertically * m_Speed)));            
        }
        else if (moveVertically > 0 && m_RigidBody.velocity.y < m_MaxSpeed && m_RigidBody.transform.position.y <= m_MaxY) //If want to go up and can go up
        {
            m_RigidBody.AddForce(new Vector2(0, (moveVertically * m_Speed)));
        }
    }

    public void SetControls(bool _hasPilot, string _horizontalControls, string _verticalControls)
    {
        m_HasPilot = _hasPilot;
        m_HorizontalControls = _horizontalControls;
        m_VerticalControls = _verticalControls;
    }



    public void TakeDamage(int _damage)
    {
        m_Health -= _damage;
        if (m_Health <= 0)
        {
            Debug.Log("DEAD");
        }
    }
}
