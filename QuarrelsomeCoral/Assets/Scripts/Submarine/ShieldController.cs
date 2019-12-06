using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public float m_AlphaValue; //Our initial starting angle -> then gets updated via AlphaStep
    
    private float m_AlphaStep; //How much to change each frame to maintain a constant speed
    private Vector3 m_NewPosition;
    private float m_MovementInput;
    private Vector3 m_LookDirection;
    private Rigidbody2D m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_AlphaStep = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //If player controlled, then move around the submarine
        if (SubmarineManager.GetInstance().m_Shield.m_PlayerControlled)
        {
            m_MovementInput = Input.GetAxisRaw(SubmarineManager.GetInstance().m_Shield.m_PlayerControlScheme);

            //Calculate new position for this frame
            m_NewPosition = new Vector3(SubmarineManager.GetInstance().m_Shield.m_XAxisRadius * Mathf.Cos(m_AlphaValue * Mathf.Deg2Rad),
                    (SubmarineManager.GetInstance().m_Shield.m_YAxisRadius) * Mathf.Sin(Mathf.Deg2Rad * m_AlphaValue), 1);

            //Set position
            transform.localPosition = m_NewPosition;

            //Calculate where to look
            m_LookDirection = SubmarineManager.GetInstance().m_Submarine.transform.position - transform.position;
            float angle = Mathf.Atan2(m_LookDirection.y, m_LookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //Calculate next step to maintain constant speed
            float nextX = SubmarineManager.GetInstance().m_Shield.m_XAxisRadius * Mathf.Sin((m_AlphaValue * Mathf.Deg2Rad) + .5f * m_AlphaStep);
            float nextY = SubmarineManager.GetInstance().m_Shield.m_YAxisRadius * Mathf.Cos((m_AlphaValue * Mathf.Deg2Rad) + .5f * m_AlphaStep);
            m_AlphaStep = 1 / Mathf.Sqrt((nextX * nextX) + (nextY * nextY));
            m_AlphaValue += -1f * m_MovementInput * m_AlphaStep * SubmarineManager.GetInstance().m_Shield.m_Speed;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If enemy made contact with the shield
        IBaseEnemy enemy = collision.collider.GetComponent<IBaseEnemy>();
        if (enemy != null)
        {
            enemy.HitShield(collision.GetContact(0));
        }

        if (collision.collider.GetComponent<UnityEngine.Tilemaps.Tilemap>() != null)
        {
            SubmarineManager.GetInstance().m_Shield.ShieldCollidiedWithTerrain(transform.position);
        }

    }
}
