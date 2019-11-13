using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public float m_AlphaValue; //Can change to private once we know these are the values we want

    private Vector3 m_NewPosition;
    private float m_MovementInput;
    private Vector3 m_LookDirection;

    // Update is called once per frame
    void Update()
    {
        //If player controlled, then move around the submarine
        if (SubmarineManager.GetInstance().m_Shield.m_PlayerControlled)
        {            
            m_MovementInput = Input.GetAxisRaw(SubmarineManager.GetInstance().m_Shield.m_PlayerControlScheme);
            if (transform.localPosition.x < SubmarineManager.GetInstance().m_Shield.m_LeftCircleLookAtPosition.localPosition.x) //Left semi circle
            {
                m_LookDirection = SubmarineManager.GetInstance().m_Shield.m_LeftCircleLookAtPosition.position - transform.position;

                m_AlphaValue += -2 * m_MovementInput; //Multiply by -1 to make left CCW and right CW   

                m_NewPosition = new Vector3(SubmarineManager.GetInstance().m_Shield.m_LeftCircleLookAtPosition.localPosition.x + SubmarineManager.GetInstance().m_Shield.m_XAxisRadius * Mathf.Cos(m_AlphaValue * ShieldManager.m_END_SPEED),
                SubmarineManager.GetInstance().m_Shield.m_YAxisRadius * Mathf.Sin(m_AlphaValue * ShieldManager.m_END_SPEED), 1);

                transform.localPosition = m_NewPosition;
            }
            else if (transform.localPosition.x > SubmarineManager.GetInstance().m_Shield.m_RightCircleLookAtPosition.localPosition.x) //right semi circle
            {
                m_LookDirection = SubmarineManager.GetInstance().m_Shield.m_RightCircleLookAtPosition.position - transform.position;

                m_AlphaValue += -2 * m_MovementInput; //Multiply by -1 to make left CCW and right CW   

                m_NewPosition = new Vector3(SubmarineManager.GetInstance().m_Shield.m_RightCircleLookAtPosition.localPosition.x 
                    + SubmarineManager.GetInstance().m_Shield.m_XAxisRadius * Mathf.Cos(m_AlphaValue * ShieldManager.m_END_SPEED),
                      SubmarineManager.GetInstance().m_Shield.m_YAxisRadius * Mathf.Sin(m_AlphaValue * ShieldManager.m_END_SPEED), 1);
                

                transform.localPosition = m_NewPosition;
            }
            else //Straightaways
            {              
                if (transform.localPosition.y < 0)
                {
                    m_MovementInput = -m_MovementInput;
                    m_AlphaValue = -Mathf.Abs(m_AlphaValue % (int)(Mathf.PI / ShieldManager.m_END_SPEED));
                }
                else { m_AlphaValue = Mathf.Abs(m_AlphaValue % (int)(Mathf.PI / ShieldManager.m_END_SPEED)); }

                transform.localPosition = new Vector3(transform.localPosition.x + m_MovementInput * SubmarineManager.GetInstance().m_Shield.m_STRAIGHT_AWAY_SPEED,
                    transform.localPosition.y,
                    transform.localPosition.z);
            }


            //Code to have this object always face the center of the Submarine
            var angle = Mathf.Atan2(m_LookDirection.y, m_LookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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

        if (collision.collider.name == "Tilemap")
        {
            SubmarineManager.GetInstance().m_Shield.ShieldCollidiedWithTerrain(transform.position);
        }

    }

}
