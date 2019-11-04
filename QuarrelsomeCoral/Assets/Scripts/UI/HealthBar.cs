using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image m_HealthBarBackground;
    public Image m_HealthBarForeground;

    private RectTransform m_ForegroundRectTransform;
    private float m_ForegroundWidth;

    private void Start()
    {
        m_ForegroundRectTransform = m_HealthBarForeground.GetComponent<RectTransform>();
        m_ForegroundWidth = m_ForegroundRectTransform.sizeDelta.x;
    }

    private void Update()
    {
        m_ForegroundRectTransform.sizeDelta = 
            new Vector2(
                m_ForegroundWidth * (float)SubmarineManager.GetInstance().m_Submarine.m_Health / (float)SubmarineManager.GetInstance().m_Submarine.m_MaxHealth,
            m_ForegroundRectTransform.sizeDelta.y);
    }

}
