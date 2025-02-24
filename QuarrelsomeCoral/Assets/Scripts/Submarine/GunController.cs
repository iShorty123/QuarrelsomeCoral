﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{    
    public float m_Speed { get; private set; }
    private GameObject m_Ammo;
    private float m_TurretLength;
    private float m_RotationAngle;
    private float m_MinimumAngle;
    private float m_MaximumAngle;
    private string m_RotationControls;
    private string m_FireButton;
    private bool m_UserControlled;
    private float m_TimeAtLastShot;
    private float m_FireRate;

    public int m_AmmoCount;

    // Start is called before the first frame update
    void Start()
    {
        m_AmmoCount = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_UserControlled)
        {
            m_RotationAngle += Input.GetAxisRaw(m_RotationControls);
            //Lock rotation at -19 < m_RotationAngle < 19
            if (m_RotationAngle > m_MaximumAngle) m_RotationAngle = m_MaximumAngle;
            if (m_RotationAngle < m_MinimumAngle) m_RotationAngle = m_MinimumAngle;
            transform.rotation = Quaternion.AngleAxis(m_RotationAngle * m_Speed, Vector3.forward);


            if (CanFire() && m_AmmoCount > 0)
            {
                if (Input.GetButton(m_FireButton))
                {
                    Instantiate(m_Ammo, transform.position + transform.up * m_TurretLength, Quaternion.identity, transform);
                    m_TimeAtLastShot = Time.realtimeSinceStartup;
                    m_AmmoCount--;
                }
            }
        }

    }

    private bool CanFire()
    {
        if (Time.realtimeSinceStartup - m_TimeAtLastShot > m_FireRate)
        {
            return true;
        }
        return false;
    }

    public void SetControls(bool _userControlled, string _rotationControls, string _fireButton, float _minimumAngle, float _maximumAngle)
    {
        m_UserControlled = _userControlled;
        m_RotationControls = _rotationControls;
        m_FireButton = _fireButton;
        m_MinimumAngle = _minimumAngle;
        m_MaximumAngle = _maximumAngle;
    }

    public void SetWeaponSpecificVariables(GameObject _ammoToUse, float _startingRotationAngle, float _rotationSpeed, float _fireRate, float _turretLength)
    {
        m_Ammo = _ammoToUse;
        m_RotationAngle = _startingRotationAngle / _rotationSpeed;
#if UNITY_WEBGL
        m_Speed = _rotationSpeed * 4;
#else
        m_Speed = _rotationSpeed;
#endif
        m_FireRate = _fireRate;
        m_TurretLength = _turretLength;
    }
}
