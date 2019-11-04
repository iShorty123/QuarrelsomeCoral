using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject m_Submarine;
    // Start is called before the first frame update
    void Start()
    {
        m_Submarine = SubmarineManager.GetInstance().m_Submarine.m_RigidBody.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_Submarine.transform.position.x, m_Submarine.transform.position.y, -10 );
    }
}
