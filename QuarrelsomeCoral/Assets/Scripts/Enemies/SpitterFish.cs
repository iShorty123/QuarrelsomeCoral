using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterFish : BaseEnemy
{

    private float m_SubmarineContactPushBackForce;
    private Animator m_Animator;
    private float m_FiredProjectile;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        m_SubmarineContactPushBackForce = 400;
        m_Health = m_MaxHealth = 50;
        m_AttackDamage = 1; //weak melee attack
        m_AttackSpeed = 1f;
        m_MoveSpeed = 6f;
        m_AttackRange = 20;
        m_ProjectileSpeed = 1500;
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("Hit", false);
        m_FiredProjectile = Time.realtimeSinceStartup;
        if (m_IsBoss)
        {
            m_Rigidbody.mass = 2;
            m_Health = m_MaxHealth = m_Health * 3;
            m_AttackDamage = m_AttackDamage * 5;
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
        RangeAttack();

    }

    private void FixedUpdate()
    {
        Move();
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
        
    }

    public override void HitSubmarine(ContactPoint2D _impactSpot)
    {
        //Push Back Method:
        m_TimeWhenStunnedByShield = Time.realtimeSinceStartup;
        m_Rigidbody.AddForce(-m_DirectionToSubmarine * m_ShieldPushBackForce);

        Attack();
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

    private void RangeAttack()
    {
        if (m_AttackRange >= m_DistanceToSubmarine && !CurrentlyAttacking()) //if in range and not currently attacking
        {
            FireProjectile();
            m_FiredProjectile = Time.realtimeSinceStartup;
        }
    }

    private void FireProjectile()
    {
        Debug.Log("Fire!");
    }

    private bool CurrentlyAttacking()
    {
        if (Time.realtimeSinceStartup - m_FiredProjectile > m_AttackSpeed) { m_Animator.SetBool("Attack", true); return false; } //Am/can attack
        else { m_Animator.SetBool("Attack", false); return true; } //Not/cannot attack
    }

}
