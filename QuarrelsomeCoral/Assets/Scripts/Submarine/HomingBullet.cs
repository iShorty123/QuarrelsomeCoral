using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour
{

    public float m_Speed;
    private Vector2 m_Direction;
    private Rigidbody2D m_RigidBody;
    private int m_AttackDamage;
    public float m_AngleChangingSpeed;
    private GameObject m_Target;
    private List<GameObject> m_NearbyEnemies = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0,-90);
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Speed = 5;
        m_AngleChangingSpeed = 500;
        m_Direction = new Vector2(transform.parent.transform.up.x, transform.parent.transform.up.y); //Use parent's orientation to determine bullet direction   
        transform.parent = null; //Break parenting so rotation can occur without effecting bullets
        m_AttackDamage = 25;
        m_RigidBody.velocity = m_Direction * m_Speed + SubmarineManager.GetInstance().m_Submarine.m_RigidBody.velocity; //Initial speed

        Destroy(gameObject, 7); //Despawn bullets after 7 seconds

        StartCoroutine(SeekEnemy());
    }

    private void Update()
    {
        if (m_Target != null)
        {
            Vector3 direction = m_Target.transform.position - m_RigidBody.transform.position;
            m_Direction = direction.normalized;
        }
    }

    private void FixedUpdate()
    {
        if (m_Target != null)
        {
            float rotateAmount = Vector3.Cross(m_Direction, transform.up).z;
            m_RigidBody.angularVelocity = -rotateAmount * m_AngleChangingSpeed;
            m_Speed += Time.fixedDeltaTime * 1.5f;
            m_RigidBody.velocity = transform.up * m_Speed;
        }
        else
        {
            m_RigidBody.AddForce(m_Direction * m_Speed);
        }
    }

    IEnumerator SeekEnemy()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        if (m_Target == null)
        {
            FindEnemy();
        }

        StartCoroutine(SeekEnemy());

    }

    private void FindEnemy()
    {
        float closestEnemy = float.MaxValue;
        GameObject target = null;

        foreach (GameObject enemy in m_NearbyEnemies)
        {
            if ((enemy.transform.position - transform.position).sqrMagnitude < closestEnemy)
            {
                target = enemy;
            }
        }

        if (target != null)
        {
            m_Target = target;
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        IBaseEnemy enemyInRange = collision.GetComponent<IBaseEnemy>();
        if (enemyInRange != null)
        {
            m_NearbyEnemies.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IBaseEnemy enemyInRange = collision.GetComponent<IBaseEnemy>();
        if (enemyInRange != null)
        {
            m_NearbyEnemies.Remove(collision.gameObject);
        }
    }

}
