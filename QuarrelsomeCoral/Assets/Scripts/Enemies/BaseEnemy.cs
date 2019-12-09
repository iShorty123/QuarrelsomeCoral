using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public abstract class BaseEnemy : MonoBehaviour, IBaseEnemy, ITakeDamage
{

    private GameObject MapGrid = null;
    private Vector2Int[] adj = new[] { new Vector2Int(-1, 1), new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(-1,0), new Vector2Int(1,0), new Vector2Int(-1,-1),
        new Vector2Int(0,-1), new Vector2Int(1,-1), new Vector2Int(-1, 2), new Vector2Int(0,2), new Vector2Int(1,2), new Vector2Int(-2,0), new Vector2Int(2,0),
        new Vector2Int(-1,-2), new Vector2Int(0,-2), new Vector2Int(1,-2), new Vector2Int(0,3), new Vector2Int(0,-3), new Vector2Int(-3,0), new Vector2Int(3,0),
        new Vector2Int(-2,-1), new Vector2Int(-2,1), new Vector2Int(2,-1), new Vector2Int(2,1)};
    private Vector3 m_Target;
    private float m_TargetAdjustment;

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
    protected float m_LookAtSpeed;

    /// <summary>How long it has been since your last attack</summary>
    protected float m_AttackCoolDownTime;

    protected Rigidbody2D m_SubmarineRigidbody;
    protected Rigidbody2D m_Rigidbody;
    protected float m_DistanceToSubmarine;
    protected Vector3 m_DirectionToSubmarine;
    protected bool m_CanDestroyTerrainWithTouch;



    public UnityEvent m_TransformIntoBoss = new UnityEvent();

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

    protected abstract void LookAtPosition(Vector3 _position);

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_AttackCoolDownTime = 0;
        m_ShieldPushBackForce = 200;
        m_LookAtSpeed = 4;
        m_MaxPursuitDistance = 500;
        m_SubmarineRigidbody = SubmarineManager.GetInstance().m_Submarine.m_RigidBody;
        m_Rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(AimAtRandomSubmarinePosition());
    }

    protected abstract void TransformIntoBoss();

    public void Setup(GameObject mapGrid)
    {
        m_TransformIntoBoss.AddListener(TransformIntoBoss);
        MapGrid = mapGrid;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        m_Target = new Vector3(m_SubmarineRigidbody.transform.position.x + m_TargetAdjustment,
            m_SubmarineRigidbody.transform.position.y, m_SubmarineRigidbody.transform.position.z);
        Vector3 direction = m_Target - m_Rigidbody.transform.position;        
        m_DistanceToSubmarine = direction.magnitude;
        m_DirectionToSubmarine = direction.normalized;
    }



    private IEnumerator AimAtRandomSubmarinePosition()
    {
        m_TargetAdjustment = Random.Range(-4, 4);
        yield return new WaitForSeconds(1);
        StartCoroutine(AimAtRandomSubmarinePosition());
    }

    protected IEnumerator ShieldCooldown()
    {
        //This wait is long enough the majority of the time to prevent multiple shield collisions occurring from 1 hit
        //Every now and then an extra collision will get processed, but that's ok
        yield return new WaitForFixedUpdate();
        m_HitShieldFlag = false;
    }

    protected void OnDestroy()
    {
        if (SubmarineManager.GetInstance())
        {
            if (!m_IsBoss)
            {
                SubmarineManager.GetInstance().m_Score += 50;
            }
            else
            {
                SubmarineManager.GetInstance().m_Score += 500;
            }
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_IsBoss && m_CanDestroyTerrainWithTouch)
        {
            Tilemap map = collision.collider.GetComponent<Tilemap>();
            //print(map);
            if (map != null)
            {
                Vector3 collisionPoint = new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
                Vector3Int pos = Vector3Int.FloorToInt(collisionPoint - map.transform.position);

                char number = map.name[map.name.Length - 1];
                GameObject plantObject = GameObject.Find("Plant" + number);
                RandomPlant plant = plantObject.GetComponent<RandomPlant>();
                GameObject[,] plantArray = plant.GetPlants();

                //delete hit tile
                map.SetTile(pos, null);
                Vector2Int plantPos = new Vector2Int(pos.x + map.size.x / 2, pos.y + map.size.y / 2);
                if (plantArray[plantPos.x, plantPos.y] != null) Destroy(plantArray[plantPos.x, plantPos.y]);

                //delete surrounding tiles
                for (int i = 0; i < 24; i++)
                {
                    Vector3Int adjPos = new Vector3Int(pos.x + adj[i].x, pos.y + adj[i].y, 0);
                    map.SetTile(adjPos, null);
                    plantPos = new Vector2Int(adjPos.x + map.size.x / 2, adjPos.y + map.size.y / 2);
                    if (plantArray[plantPos.x, plantPos.y] != null) Destroy(plantArray[plantPos.x, plantPos.y]);
                }



            }
        }
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (m_IsBoss && m_CanDestroyTerrainWithTouch)
        {
            Tilemap map = collision.collider.GetComponent<Tilemap>();
            //print(map);
            if (map != null)
            {
                Vector3 collisionPoint = new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
                Vector3Int pos = Vector3Int.FloorToInt(collisionPoint - map.transform.position);

                char number = map.name[map.name.Length - 1];
                GameObject plantObject = GameObject.Find("Plant" + number);
                RandomPlant plant = plantObject.GetComponent<RandomPlant>();
                GameObject[,] plantArray = plant.GetPlants();

                //delete hit tile
                map.SetTile(pos, null);
                Vector2Int plantPos = new Vector2Int(pos.x + map.size.x / 2, pos.y + map.size.y / 2);
                if (plantArray[plantPos.x, plantPos.y] != null) Destroy(plantArray[plantPos.x, plantPos.y]);

                //delete surrounding tiles
                for (int i = 0; i < 24; i++)
                {
                    Vector3Int adjPos = new Vector3Int(pos.x + adj[i].x, pos.y + adj[i].y, 0);
                    map.SetTile(adjPos, null);
                    plantPos = new Vector2Int(adjPos.x + map.size.x / 2, adjPos.y + map.size.y / 2);
                    if (plantArray[plantPos.x, plantPos.y] != null) Destroy(plantArray[plantPos.x, plantPos.y]);
                }
            }
        }
    }

}
