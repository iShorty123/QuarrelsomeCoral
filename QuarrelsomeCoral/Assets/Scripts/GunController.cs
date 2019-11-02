using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{

    public static float m_RotationAngle;
    public static bool m_Fire;
    public float m_Speed;
    public GameObject m_Bullet;

    // Start is called before the first frame update
    void Start()
    {
        m_Speed = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //Lock rotation at -19 < m_RotationAngle < 19
        if (m_RotationAngle > 95) m_RotationAngle = 95;
        if (m_RotationAngle < -95) m_RotationAngle = -95;

        transform.rotation = Quaternion.AngleAxis(-m_RotationAngle * m_Speed, Vector3.forward);

        if (m_Fire)
        {
            Instantiate(m_Bullet, transform.position + transform.forward * 1, Quaternion.identity, transform);
        }

    }
}
