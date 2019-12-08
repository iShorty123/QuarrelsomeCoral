using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image m_HealthBarBackground;
    public Image m_HealthBarForeground;
    public Image m_HealthBarForegroundFront;
    public Image m_HealthBarForegroundBack;

    private RectTransform m_ForegroundRectTransform;
    private float m_ForegroundWidth;
    private float m_ForegroundX;

    private RectTransform m_BackRectTransform;
    private float m_BackX;

    private void Start()
    {
        m_HealthBarBackground = GameObject.Find("HBbackground").GetComponent<Image>();
        m_HealthBarForeground = GameObject.Find("HBforeground").GetComponent<Image>();
        m_HealthBarForegroundBack = GameObject.Find("HBforegroundB").GetComponent<Image>();
        m_HealthBarForegroundFront = GameObject.Find("HBforegroundF").GetComponent<Image>();

        m_ForegroundRectTransform = m_HealthBarForeground.GetComponent<RectTransform>();
        m_ForegroundWidth = m_ForegroundRectTransform.sizeDelta.x;
        m_ForegroundX = m_ForegroundRectTransform.position.x;

        m_BackRectTransform = m_HealthBarForegroundBack.GetComponent<RectTransform>();
        m_BackX = m_BackRectTransform.position.x;
    }

    private void Update()
    {
        m_ForegroundRectTransform = m_HealthBarForeground.GetComponent<RectTransform>();
        m_BackRectTransform = m_HealthBarForegroundBack.GetComponent<RectTransform>();

        float iniWidth = m_ForegroundRectTransform.sizeDelta.x * m_ForegroundRectTransform.lossyScale.x;

        m_ForegroundRectTransform.sizeDelta = new Vector2(
                m_ForegroundWidth * (float)SubmarineManager.GetInstance().m_Submarine.m_Health / (float)SubmarineManager.GetInstance().m_Submarine.m_MaxHealth,
            m_ForegroundRectTransform.sizeDelta.y);

        float endWidth = m_ForegroundRectTransform.sizeDelta.x * m_ForegroundRectTransform.lossyScale.x;
        float diff = (iniWidth - endWidth);

        m_ForegroundRectTransform.position = new Vector2(m_ForegroundRectTransform.position.x - diff / 2.2f, m_ForegroundRectTransform.position.y);

       m_BackRectTransform.position = new Vector2(m_BackRectTransform.position.x - diff / 1.1f, m_BackRectTransform.position.y);

        if (endWidth == 0) {
            m_HealthBarForegroundBack.enabled = false;
            m_HealthBarForegroundFront.enabled = false;
        }
    }

}
