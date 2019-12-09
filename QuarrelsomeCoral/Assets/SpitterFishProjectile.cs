using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpitterFishProjectile : MonoBehaviour
{

    public float m_Speed;
    private Vector2 m_Direction;
    private Rigidbody2D m_RigidBody;
    private float m_MaxSpeed;
    private int m_AttackDamage;
    public bool m_IsBoss;

    Vector2Int[] adj = new[] { new Vector2Int(-1, 1), new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(-1,0), new Vector2Int(1,0), new Vector2Int(-1,-1),
        new Vector2Int(0,-1), new Vector2Int(1,-1), new Vector2Int(-1, 2), new Vector2Int(0,2), new Vector2Int(1,2), new Vector2Int(-2,0), new Vector2Int(2,0),
        new Vector2Int(-1,-2), new Vector2Int(0,-2), new Vector2Int(1,-2), new Vector2Int(0,3), new Vector2Int(0,-3), new Vector2Int(-3,0), new Vector2Int(3,0),
        new Vector2Int(-2,-1), new Vector2Int(-2,1), new Vector2Int(2,-1), new Vector2Int(2,1)};

    // Start is called before the first frame update
    void Start()
    {
        m_MaxSpeed = 30;
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Speed = 3f;
        m_Direction = new Vector2(-transform.parent.transform.right.x, -transform.parent.transform.right.y); //Use parent's orientation to determine bullet direction          
        transform.up = transform.parent.transform.up;      
        m_AttackDamage = 5;
        m_RigidBody.velocity = m_Direction * m_Speed + transform.parent.GetComponent<Rigidbody2D>().velocity; //Initial speed
        float parentRotZ = transform.parent.localEulerAngles.z;
        transform.parent = null; //Break parenting so movement can occur without effecting projectile
        transform.rotation = Quaternion.AngleAxis(90+parentRotZ, Vector3.forward);     
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

        Tilemap map = collision.collider.GetComponent<Tilemap>();
        if (map != null && m_IsBoss)
        {

            Vector3 collisionPoint = new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
            Vector3Int pos = Vector3Int.FloorToInt(collisionPoint - map.transform.position);

            if (collisionPoint.y < -50) return;

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

            return;
        }
        if (collision.collider.name == "SubmarineRigidbody")
        {
            SubmarineManager.GetInstance().m_Submarine.TakeDamage(m_AttackDamage);
        }

    }
}
