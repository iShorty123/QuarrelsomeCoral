using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeSnail : BaseEnemy
{
    public GameObject m_Projectile;
    private float m_SubmarineContactPushBackForce;
    private Animator m_Animator;
    private float m_FiredProjectile;
    private float m_TimeStartedRetreating;

    private float m_MaxRestTime;

    private Vector3 m_MyNormal;
    private float m_Gravity;
    private float m_DistanceToGround;
    private float m_NextTileDectectionDistance;
    private Vector3 m_SurfaceNormal;
    private bool m_Transitioning;
    private float m_OneDirectionLock;

    private bool m_Hiding;
    private bool m_JustWentLeft;
    private bool m_JustWentRight;
    private bool m_MoveRandomly;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        m_SubmarineContactPushBackForce = 400;
        m_Health = m_MaxHealth = 50;
        m_AttackDamage = 1; //weak melee attack
        m_AttackSpeed = 1f;
        m_MoveSpeed = 7;
        m_AttackRange = 30;
        m_ProjectileSpeed = 1500;
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("Hit", false);
        m_FiredProjectile = Time.realtimeSinceStartup;
        m_IsBoss = true;
        transform.localScale = new Vector3(transform.localScale.x * 3, transform.localScale.y * 3, 1);
        if (m_IsBoss)
        {
            m_Rigidbody.mass = 2;
            m_Health = m_MaxHealth = m_Health * 5;
            m_AttackDamage = m_AttackDamage * 3;
            m_AttackSpeed = m_AttackSpeed / 2;
            m_SubmarineContactPushBackForce = m_SubmarineContactPushBackForce * 2;
            m_ProjectileSpeed = m_ProjectileSpeed * 2;
        }
        m_TimeStartedRetreating = Time.realtimeSinceStartup;
        m_MaxRestTime = 10f;
        m_DistanceToSubmarine = 500; //For the first frame after creation, assume out of range


        m_Gravity = -9.8f;
        m_MyNormal = transform.up;
        m_DistanceToGround = transform.GetComponent<BoxCollider2D>().bounds.extents.y - transform.GetComponent<BoxCollider2D>().offset.y;
        m_DistanceToGround = 0.1f;
        m_NextTileDectectionDistance = 2f;
        m_Transitioning = false;
        m_OneDirectionLock = 0;
        m_JustWentRight = false;
        m_JustWentLeft = false;
        m_MoveRandomly = false;
    }

    private Vector3 GetMiddleOfSnail()
    {
        return transform.position + transform.up * .5f;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (m_Transitioning) { return; }

        if (Input.GetKeyDown(KeyCode.Space)) { RangeAttack(); }
        RaycastHit2D hit = Physics2D.Raycast(GetMiddleOfSnail(), transform.right, m_NextTileDectectionDistance, 1 << 0);
        if (hit && !m_JustWentLeft && !CurrentlyHiding()) 
        {
            m_OneDirectionLock = 0;
            StartCoroutine(JustWentRightCooldown());
            TransitionToWall(hit.point, hit.normal); 
        }
        else 
        { 
            hit = Physics2D.Raycast(GetMiddleOfSnail(), -transform.right, m_NextTileDectectionDistance, 1 << 0);
            if (hit && !m_JustWentRight && !CurrentlyHiding())
            {
                m_OneDirectionLock = 0;
                StartCoroutine(JustWentLeftCooldown());
                TransitionToWall(hit.point, hit.normal);
            }
        }

        hit = Physics2D.Raycast(transform.position, -m_MyNormal, m_NextTileDectectionDistance, 1 << 0);
        if (hit)
        {
            m_SurfaceNormal = hit.normal;
        }
        else { m_SurfaceNormal = Vector3.up; }

        m_MyNormal = Vector3.Lerp(m_MyNormal, m_SurfaceNormal, Time.deltaTime * 10);

        //Rotate to align 
        float angle = Mathf.Atan2(m_MyNormal.y, m_MyNormal.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), Time.deltaTime * m_LookAtSpeed);

    }

    private IEnumerator JustWentRightCooldown()
    {
        m_JustWentRight = true;
        while (m_OneDirectionLock < 2)
        {
            m_OneDirectionLock += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        m_JustWentRight = false;

    }

    private IEnumerator JustWentLeftCooldown()
    {
        m_JustWentLeft = true;
        while (m_OneDirectionLock < 2)
        {
            m_OneDirectionLock += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        m_JustWentLeft = false;

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void TransitionToWall(Vector2 _point, Vector2 _normal)
    {
        m_Transitioning = true;
        Vector3 originalPosition = transform.position;
        Vector3 target = _point + _normal * (m_DistanceToGround * .5f);
        StartCoroutine(Transition(originalPosition, target, _normal));        
    }

    private IEnumerator Transition(Vector3 _orginalPosition, Vector3 _target, Vector2 _normal)
    {
        float time = 0;
        while (time <= .25f)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(_orginalPosition, _target, time * 4);
            float angle = Mathf.Atan2(m_MyNormal.y, m_MyNormal.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), Time.deltaTime * m_LookAtSpeed * 4);

            yield return new WaitForEndOfFrame();
        }

        m_Transitioning = false;
        m_MyNormal = _normal;
        yield return new WaitForEndOfFrame();
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
        m_Rigidbody.AddForce(m_MyNormal * m_Gravity);
        if (m_TimeStartedRetreating + m_MaxRestTime < Time.realtimeSinceStartup) //if can hide - hide
        {
            m_Animator.SetTrigger("Hide");
            m_Hiding = true;
            m_TimeStartedRetreating = Time.realtimeSinceStartup;
        }
        else if (m_AttackRange < m_DistanceToSubmarine && !CurrentlyHiding()) //if not hiding and out of range - move to be in range
        {
            if (m_DirectionToSubmarine.x >= 0)
            { m_Rigidbody.AddForce(transform.right * m_MoveSpeed); } //Move right until within range
            else if (m_DirectionToSubmarine.x < 0) { m_Rigidbody.AddForce(-transform.right * m_MoveSpeed); } //Move left until within range
        }
        else if (!CurrentlyAttacking() && !CurrentlyHiding() && !m_MoveRandomly) //if not attacking and not hiding and not on cooldown - move left or right
        {
            StartCoroutine(RandomMove());
        }
        else if (!CurrentlyAttacking() && !CurrentlyHiding())
        {
            RangeAttack();
        }

    }

    private IEnumerator RandomMove()
    {
        m_MoveRandomly = true;
        int direction = Random.Range(-100, 100);
        if (direction < 0) { direction = -1; }
        else { direction = 1; }
        float moveRandomlyTime = 0;
        while (moveRandomlyTime < 1)
        {
            moveRandomlyTime += Time.deltaTime;
            m_Rigidbody.AddForce(transform.right * m_MoveSpeed * direction);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(5);
        m_MoveRandomly = false;
    }

    public override void HitSubmarine(ContactPoint2D _impactSpot)
    {
        Attack();
    }

    public override void TakeDamage(int _damage)
    {
        //m_TimeWhenStunned = Time.realtimeSinceStartup; //Put into the stun state upon taking damage
        if (!m_Hiding)
        {
            m_Health -= _damage;
            if (m_Health <= 0)
            {
                Destroy(gameObject);
            }
            StartCoroutine(HitAnimationTimer());
        }
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
        //m_CanDestroyTerrainWithTouch = true; Don't want snail to destroy terrain
    }

    private void RangeAttack()
    {
        if (m_AttackRange >= m_DistanceToSubmarine) //if in range and not currently attacking
        {
            m_Animator.SetTrigger("Attack");
            FireProjectile();
            m_FiredProjectile = Time.realtimeSinceStartup;
        }
    }

    private void FireProjectile()
    {
        GameObject projectile = Instantiate(m_Projectile, transform.position + new Vector3(0, .5f, 0), Quaternion.identity, transform);
        if (m_IsBoss) { projectile.GetComponent<ConeSnailProjectile>().m_IsBoss = true; }
    }

    private bool CurrentlyAttacking()
    {
        if (Time.realtimeSinceStartup - m_FiredProjectile > m_AttackSpeed) { return false; } //Am/can attack
        else { return true; } //Not/cannot attack
    }

    private bool CurrentlyHiding()
    {
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ConeSnailHide") && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            m_Hiding = true;
            return true;
        }
        m_Hiding = false;
        m_Animator.ResetTrigger("Hide");
        return false;

        //if (Time.realtimeSinceStartup - m_Lasthide > m_AttackSpeed) { return false; } //Am/can attack
        //else { return true; } //Not/cannot attack
    }

    protected override void LookAtPosition(Vector3 _position)
    {
        float angle = Mathf.Atan2(_position.y, _position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 180, Vector3.forward), Time.deltaTime * m_LookAtSpeed);

    }


}
