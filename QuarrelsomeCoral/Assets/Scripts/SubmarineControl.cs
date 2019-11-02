using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineControl : MonoBehaviour
{
    public Rigidbody2D m_RigidBody;
    public GameObject m_Shield;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_RigidBody.position; //Move sub via the SubmarineRigibody object
    }

}
