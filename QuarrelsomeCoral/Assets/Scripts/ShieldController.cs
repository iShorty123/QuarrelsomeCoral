using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public float m_MoveAmount; //Can change to private once we know these are the values we want

    private Vector3 m_NewPosition;  
    private float m_MovementInput;


    // Start is called before the first frame update
    void Start()
    {       
        m_MoveAmount = int.Parse(gameObject.name); //Use name to determine starting location
    }

    // Update is called once per frame
    void Update()
    {
        //If player controlled, then move around the submarine
        if (SubmarineManager.GetInstance().m_Shield.m_PlayerControlled)
        {
            m_MovementInput = Input.GetAxisRaw(SubmarineManager.GetInstance().m_Shield.m_PlayerControlScheme);
            //Need to test the logic behind this when using a controller - it may make more sense to have the y-axis come into place when using the controller
            //moving the shield to where ever the thumb stick is on the controller
            //TODO: Reset alpha some how? As in theory, overflow could eventually occur here
            m_MoveAmount += -1 * m_MovementInput; //Multiply by -1 to make left CCW and right CW            
            m_NewPosition = new Vector3(SubmarineManager.GetInstance().m_Shield.m_XAxisRadius * Mathf.Cos(m_MoveAmount * SubmarineManager.GetInstance().m_Shield.m_Speed),
                SubmarineManager.GetInstance().m_Shield.m_YAxisRadius * Mathf.Sin(m_MoveAmount * SubmarineManager.GetInstance().m_Shield.m_Speed), 1);

            transform.localPosition = m_NewPosition;
        }

        //Code to have this object always face the center of the Submarine
        //var dir = transform.parent.position - transform.position;
        //var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }
}
