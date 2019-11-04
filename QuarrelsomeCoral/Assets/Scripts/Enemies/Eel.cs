using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eel : BaseEnemy
{

    public float m_SubmarineContactPushBackForce;
    public float m_LungeSpeed;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        m_SubmarineContactPushBackForce = 300;
        m_Health = m_MaxHealth = 75;
        m_AttackDamage = 5; 
        m_AttackSpeed = 0.25f;
        m_MoveSpeed = 6f;
        m_AttackRange = 20;
        m_ProjectileSpeed = 400;
        m_IsBoss = false;

    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        LookAtSubmarine();
    }

    private void FixedUpdate()
    {
        if (m_DistanceToSubmarine < m_MaxPursuitDistance && m_DistanceToSubmarine > m_AttackRange) 
        {
            // m_Rigidbody.MovePosition(new Vector2(m_Rigidbody.position.x, m_Rigidbody.position.y) + 
            //     new Vector2(m_DirectionToSubmarine.x, m_DirectionToSubmarine.y) * m_MoveSpeed * Time.fixedDeltaTime);

            m_Rigidbody.velocity = m_DirectionToSubmarine * m_MoveSpeed;

            //m_Rigidbody.AddForce(m_DirectionToSubmarine * m_MoveSpeed);
        }
        else if (m_DistanceToSubmarine <= m_AttackRange)
        {
            m_Rigidbody.AddForce(m_DirectionToSubmarine * m_ProjectileSpeed);
        }
    }

    public override void HitSubmarine()
    {
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.AddForce(-m_DirectionToSubmarine * m_SubmarineContactPushBackForce);
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void Move()
    {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage()
    {
        throw new System.NotImplementedException();
    }
}
