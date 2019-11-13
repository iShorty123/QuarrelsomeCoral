using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IBaseEnemy, ITakeDamage
{
    private float m_LookAtSpeed;

    protected int m_Health;
    protected int m_MaxHealth;
    protected int m_AttackDamage;
    /// <summary>How long you have to wait before you can attack again</summary>
    protected float m_AttackSpeed;
    protected int m_AttackRange;
    protected float m_MoveSpeed;
    protected int m_ProjectileSpeed; 
    protected bool m_IsBoss;
    protected int m_MaxPursuitDistance;
    protected float m_ShieldPushBackForce;
    protected bool m_HitShieldFlag;
    protected float m_TimeWhenStunnedByShield;

    /// <summary>How long it has been since your last attack</summary>
    protected float m_AttackCoolDownTime;

    protected Rigidbody2D m_SubmarineRigidbody;
    protected Rigidbody2D m_Rigidbody;
    protected float m_DistanceToSubmarine;
    protected Vector3 m_DirectionToSubmarine;

    public abstract void HitSubmarine(ContactPoint2D _impactSpot);

    public virtual void HitShield(ContactPoint2D _impactSpot)
    {
        if (m_HitShieldFlag)
        {
            return;
        }
        m_HitShieldFlag = true;
        StartCoroutine(ShieldCooldown());

        //Reflection Method:
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

    public abstract void Attack();

    public abstract void Move();

    public abstract void TakeDamage(int _damage);

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_AttackCoolDownTime = 0;
        m_ShieldPushBackForce = 200;
        m_LookAtSpeed = 4;
        m_MaxPursuitDistance = 500;
        m_SubmarineRigidbody = SubmarineManager.GetInstance().m_Submarine.m_RigidBody;
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Vector3 direction = m_SubmarineRigidbody.transform.position - m_Rigidbody.transform.position;        
        m_DistanceToSubmarine = direction.magnitude;
        m_DirectionToSubmarine = direction.normalized;
    }

    protected void LookAtSubmarine()
    {
        float angle = Mathf.Atan2(m_DirectionToSubmarine.y, m_DirectionToSubmarine.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), Time.deltaTime * m_LookAtSpeed); 
        // -90 is specific to the Eel right now
        //if others need different adjustment then we'll take this function out of the base class and place into IBaseEnemy

    }

    protected IEnumerator ShieldCooldown()
    {
        //This wait is long enough the majority of the time to prevent multiple shield collisions occurring from 1 hit
        //Every now and then an extra collision will get processed, but that's ok
        yield return new WaitForFixedUpdate();
        m_HitShieldFlag = false;
    }
}
