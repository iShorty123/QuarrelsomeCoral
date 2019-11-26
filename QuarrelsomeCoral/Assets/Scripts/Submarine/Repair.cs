using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : MonoBehaviour
{


    private bool m_UserControlled;
    private float m_TimeSinceLastRepair;
    private const float m_REPAIR_SPEED = 1;
    private string m_GrabScrapButton;

    // Start is called before the first frame update
    void Start()
    {
        m_TimeSinceLastRepair = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_UserControlled && m_TimeSinceLastRepair + m_REPAIR_SPEED < Time.realtimeSinceStartup)
        {
            if (SubmarineManager.GetInstance().m_Submarine.m_Health < SubmarineManager.GetInstance().m_Submarine.m_MaxHealth)
            {
                m_TimeSinceLastRepair = Time.realtimeSinceStartup;
                SubmarineManager.GetInstance().m_Submarine.m_Health += 1;
            }
        }
    }

    public void SetControls(bool _userControlled, string _grabScrapButton)
    {
        m_UserControlled = _userControlled;
        m_GrabScrapButton = _grabScrapButton;

    }
}
