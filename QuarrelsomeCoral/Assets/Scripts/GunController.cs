using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{    
    public float m_Speed;
    public GameObject m_Bullet;

    private float m_RotationAngle;
    private float m_MinimumAngle;
    private float m_MaximumAngle;
    private string m_RotationControls;
    private string m_FireButton;
    private bool m_UserControlled;
    // Start is called before the first frame update
    void Start()
    {
        m_Speed = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //    GunController.m_Fire = Input.GetButton(m_Action2);

        if (m_UserControlled)
        {
            m_RotationAngle += Input.GetAxisRaw(m_RotationControls);

            //Lock rotation at -19 < m_RotationAngle < 19
            if (m_RotationAngle > m_MaximumAngle) m_RotationAngle = m_MaximumAngle;
            if (m_RotationAngle < m_MinimumAngle) m_RotationAngle = m_MinimumAngle;

            transform.rotation = Quaternion.AngleAxis(-m_RotationAngle * m_Speed, Vector3.forward);

            if (Input.GetButton(m_FireButton))
            {
                Instantiate(m_Bullet, transform.position + transform.forward * 1, Quaternion.identity, transform);
            }
        }
    }

    public void SetControls(bool _userControlled, string _rotationControls, string _fireButton, float _minimumAngle, float _maximumAngle)
    {
        m_UserControlled = _userControlled;
        m_RotationControls = _rotationControls;
        m_FireButton = _fireButton;
        m_MinimumAngle = _minimumAngle;
        m_MaximumAngle = _maximumAngle;
    }
}
