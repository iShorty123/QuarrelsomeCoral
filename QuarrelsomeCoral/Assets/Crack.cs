using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crack : MonoBehaviour
{

    SubmarineController m_Submarine;
    public int m_LocalHealth;
    public int m_MaxLocalHealth;

    // Start is called before the first frame update
    void Start()
    {
        m_Submarine = SubmarineManager.GetInstance().m_Submarine;
        m_MaxLocalHealth = 20;

        m_LocalHealth = 0;

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_LocalHealth == 0)
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }
        else if (m_LocalHealth <= m_MaxLocalHealth)
        {
            float newScale = (float)m_LocalHealth / (float)m_MaxLocalHealth * 4;
            transform.localScale = new Vector3(newScale, newScale, 1);
        }
        if (m_LocalHealth != m_MaxLocalHealth && m_LocalHealth!= 0) { gameObject.SetActive(true); }
    }

    public void Repair()
    {
        if (m_LocalHealth - 5 < 0)
        {
            m_Submarine.m_Health += Mathf.Abs(m_LocalHealth - 5);
            m_LocalHealth = 0;
        }
        else
        {
            m_LocalHealth -= 5;
            m_Submarine.m_Health += 5;
        }


    }
}
