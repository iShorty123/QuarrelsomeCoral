using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{


    public Vector3 m_Direction;
    public float m_Speed;

    // Start is called before the first frame update
    void Start()
    {
        m_Speed = 5;
        m_Direction = transform.parent.transform.up; //Use parent's orientation to determine bullet direction
        transform.parent = null; //Break parenting so rotation can occur without effecting bullets
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += m_Direction * m_Speed * Time.deltaTime;
    }
}
