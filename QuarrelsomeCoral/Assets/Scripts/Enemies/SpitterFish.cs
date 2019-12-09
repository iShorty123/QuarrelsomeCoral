using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterFish : BaseEnemy
{
    public GameObject m_Projectile;
    private float m_SubmarineContactPushBackForce;
    private Animator m_Animator;
    private float m_FiredProjectile;
    private Vector3 m_RandomDirection;
    private bool m_TooClose;
    private float m_TimeStartedRetreating;
    private float m_RetreatTime;
    private float m_RandomPathTimeStart;
    private float m_RandomPathMax;
    private float m_RestAfterFireTimeStart;
    private float m_RestBeforeFireTimeStart;
    private float m_MaxRestTime;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        m_SubmarineContactPushBackForce = 400;
        m_Health = m_MaxHealth = 50;
        m_AttackDamage = 1; //weak melee attack
        m_AttackSpeed = 5f;
        m_MoveSpeed = 5.5f;
        m_AttackRange = 15;
        m_ProjectileSpeed = 1500;
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("Hit", false);
        m_FiredProjectile = Time.realtimeSinceStartup;
        if (m_IsBoss)
        {
            m_Rigidbody.mass = 2;
            m_Health = m_MaxHealth = m_Health * 3;
            m_AttackDamage = m_AttackDamage * 3;
            m_SubmarineContactPushBackForce = m_SubmarineContactPushBackForce * 2;
            m_ProjectileSpeed = m_ProjectileSpeed * 2;
        }
        m_TimeStartedRetreating = Time.realtimeSinceStartup;
        m_RandomPathTimeStart = Time.realtimeSinceStartup;
        m_RestAfterFireTimeStart = Time.realtimeSinceStartup;
        m_MaxRestTime = 1.5f;
        m_RetreatTime = 3;
        m_RandomPathMax = 1;
        m_DistanceToSubmarine = 500; //For the first frame after creation, assume out of range
        SetRandomPosition();
    }

    private void SetRandomPosition()
    {
        m_RandomDirection = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)).normalized;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

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
        if (m_RestBeforeFireTimeStart + m_MaxRestTime > Time.realtimeSinceStartup) { return; } //Just chill
        if ( m_RestAfterFireTimeStart + m_MaxRestTime > Time.realtimeSinceStartup) { return; } //Just chill
        if (m_TimeStartedRetreating + m_RetreatTime > Time.realtimeSinceStartup
            && Physics2D.Raycast(transform.position, m_DirectionToSubmarine, m_AttackRange * 2.5f, 1 << LayerMask.NameToLayer("SubmarineExterior"))) { Retreat(); return; }
        else { m_TimeStartedRetreating = 0; }//If currently retreating - only do that
        if (m_RandomPathTimeStart + m_RandomPathMax > Time.realtimeSinceStartup) { RandomPath(); return; } //If currently on a random path - only do that
        RaycastHit2D hit = Physics2D.Raycast(transform.position, m_DirectionToSubmarine, m_AttackRange, 1 << LayerMask.NameToLayer("SubmarineExterior"));
        
        if (hit)
        {
            if (hit.distance < m_AttackRange / 2) { m_TooClose = true; }
            else { m_TooClose = false; }
        } else { m_TooClose = false; }

        if (m_DistanceToSubmarine < m_MaxPursuitDistance && m_DistanceToSubmarine > m_AttackRange)
        {
            m_Rigidbody.drag = 1;
            m_Rigidbody.velocity = m_DirectionToSubmarine * m_MoveSpeed;
            LookAtPosition(m_DirectionToSubmarine);
            SetRandomPosition();
        }
        else if (m_DistanceToSubmarine <= m_AttackRange && !m_TooClose) //If inside attack range but not too close
        {
            m_Rigidbody.drag = 3; //Come to a stop
            if (!CurrentlyAttacking() && FacingFireDirection()) //if not attacking
            {
                m_RestBeforeFireTimeStart = Time.realtimeSinceStartup;
                RangeAttack();
            }
            else //If not attacking
            {
                if (!CurrentlyAttacking()) { LookAtPosition(m_DirectionToSubmarine); }
                else
                {
                    m_RandomPathTimeStart = Time.realtimeSinceStartup;
                }
            }

        }
        else if(m_TooClose) //If too close
        {
            m_TimeStartedRetreating = Time.realtimeSinceStartup;
            Retreat();
            SetRandomPosition();
        }

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
        if (m_AttackRange >= m_DistanceToSubmarine) //if in range and not currently attacking
        {
            m_Animator.SetTrigger("Attack");
            FireProjectile();
            m_FiredProjectile = Time.realtimeSinceStartup;
            m_RestAfterFireTimeStart = Time.realtimeSinceStartup;
        }
    }

    private void FireProjectile()
    {
        GameObject projectile = Instantiate(m_Projectile, transform.position + m_DirectionToSubmarine *2, Quaternion.identity, transform);
        if (m_IsBoss) { projectile.GetComponent<SpitterFishProjectile>().m_IsBoss = true; }
    }

    private bool CurrentlyAttacking()
    {
        if (Time.realtimeSinceStartup - m_FiredProjectile > m_AttackSpeed) { return false; } //Am/can attack
        else { return true; } //Not/cannot attack
    }

    protected override void LookAtPosition(Vector3 _position)
    {
        float angle = Mathf.Atan2(_position.y, _position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle -180, Vector3.forward), Time.deltaTime * m_LookAtSpeed);

    }

    private bool FacingFireDirection()
    {
        //If ray cast hits submarine - then we are safe to fire
        if (Physics2D.Raycast(transform.position, -transform.right, m_AttackRange, 1<< LayerMask.NameToLayer("SubmarineExterior")))
        {
            return true;
        }
        return false;
    }

    private void Retreat()
    {
        m_Rigidbody.drag = 1;
        m_Rigidbody.velocity = -m_DirectionToSubmarine * (m_MoveSpeed * 2 +1f);
        LookAtPosition(-m_DirectionToSubmarine);
    }

    private void RandomPath()
    {
        m_Rigidbody.velocity = m_RandomDirection * m_MoveSpeed;
        Debug.DrawLine(transform.position, m_RandomDirection * m_MoveSpeed);
        LookAtPosition(m_RandomDirection);
    }
}
