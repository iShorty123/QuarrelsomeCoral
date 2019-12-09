using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private Light m_MyLight;
    private float m_InitialIntensity;
    private float m_IntensityRatio;
    private float m_StartingHeight;
    // Start is called before the first frame update
    void Start()
    {
        m_MyLight = transform.GetComponent<Light>();
        m_InitialIntensity = m_MyLight.intensity;
        m_StartingHeight = SubmarineManager.GetInstance().m_Submarine.transform.position.y;
        if (m_StartingHeight == 0) { m_StartingHeight = float.Epsilon; }
        m_IntensityRatio = m_InitialIntensity / m_StartingHeight;
#if UNITY_WEBGL
        m_InitialIntensity += .5f;
#endif

    }

    // Update is called once per frame
    void Update()
    {
        if (SubmarineManager.GetInstance().m_Submarine.transform.position.y > 0)
        {
            m_MyLight.intensity = m_InitialIntensity - (m_IntensityRatio * SubmarineManager.GetInstance().m_Submarine.transform.position.y);
        }
    }
}
