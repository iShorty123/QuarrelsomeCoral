using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationLightAdjuster : MonoBehaviour
{
    public SpriteRenderer m_StationLightGuts;
    public Light m_GlowLight;
    public Light m_SpotLight;

    private List<Color> m_LightColors = new List<Color>();
    private List<Color> m_StationLightColor = new List<Color>();

    private int m_CurrentColor;
    private int m_NextColor;
    private float m_TimeElapsed;
    private bool m_AtPeakColor;
    // Start is called before the first frame update
    void Start()
    {
        AddColor(0); //Add turned-off color
        m_CurrentColor = 0;
        m_NextColor = 0;
        m_AtPeakColor = false;
    }

    // Update is called once per frame
    void Update()
    {
        CycleLights();
    }

    public void AddColor(int _player)
    {
        switch (_player)
        {
            case 1:
                //Green-ish 
                m_StationLightColor.Add(new Color(0.397032f, 1, 0, 1));
                m_LightColors.Add(new Color(0, 1, 0.4171932f, 1));
                break;
            case 2:
                //Blue-ish
                m_StationLightColor.Add(new Color(0, 0.6313722f, 1, 1));
                m_LightColors.Add(new Color(0, 0.6233468f, 1, 1));
                break;
            case 3:
                //Red-ish
                m_StationLightColor.Add(new Color(1, 0, 0, 1));
                m_LightColors.Add(new Color(0.6424093f, 0, 1, 1));
                break;
            case 4:
                //White-ish
                m_StationLightColor.Add(new Color(1, 1, 1, 1));
                m_LightColors.Add(new Color(1, 1, 1, 1));
                break;
            default:
                //Black/turned off
                m_StationLightColor.Add(new Color(0.5566038f, 0.4125934f, 0.2704254f, 1));
                m_LightColors.Add(new Color(0, 0, 0, 1));
                break;
        }
        m_NextColor = m_StationLightColor.Count - 1;
        m_CurrentColor = m_StationLightColor.Count - 1;
        m_TimeElapsed = 0;
        //m_AtPeakColor = true;
    }

    public void RemoveColor(int _player)
    {
        switch (_player)
        {
            case 1:
                //Green-ish 
                m_StationLightColor.Remove(new Color(0.397032f, 1, 0, 1));
                m_LightColors.Remove(new Color(0, 1, 0.4171932f, 1));
                break;
            case 2:
                //Blue-ish
                m_StationLightColor.Remove(new Color(0, 0.6313722f, 1, 1));
                m_LightColors.Remove(new Color(0, 0.6233468f, 1, 1));
                break;
            case 3:
                //Red-ish
                m_StationLightColor.Remove(new Color(1, 0, 0, 1));
                m_LightColors.Remove(new Color(0.6424093f, 0, 1, 1));
                break;
            case 4:
                //White-ish
                m_StationLightColor.Remove(new Color(1, 1, 1, 1));
                m_LightColors.Remove(new Color(1, 1, 1, 1));
                break;
            default:
                //Black/turned off
                m_StationLightColor.Remove(new Color(0.5566038f, 0.4125934f, 0.2704254f, 1));
                m_LightColors.Remove(new Color(0, 0, 0, 1));
                break;
        }
    }

    private void CycleLights()
    {
        //if (m_CurrentColor == m_NextColor) { m_NextColor++; }
        if (m_CurrentColor >= m_StationLightColor.Count) { m_CurrentColor = 0; }
        if (m_NextColor >= m_StationLightColor.Count || m_NextColor < 0) { m_NextColor = 0; }

        if (!m_AtPeakColor)
        {
            m_StationLightGuts.color = Color.Lerp(m_StationLightColor[m_CurrentColor], m_StationLightColor[m_NextColor], m_TimeElapsed);
            m_SpotLight.color = Color.Lerp(m_LightColors[m_CurrentColor], m_LightColors[m_NextColor], m_TimeElapsed);
            m_GlowLight.color = Color.Lerp(m_LightColors[m_CurrentColor], m_LightColors[m_NextColor], m_TimeElapsed);
            m_TimeElapsed += Time.deltaTime;
            if (m_TimeElapsed > 1f)
            {
                m_TimeElapsed = 0;
                m_CurrentColor = m_NextColor;
                m_NextColor++;

                m_AtPeakColor = true;
            }
        }
        else
        {
            //m_CurrentColor = m_NextColor;
            //m_NextColor++;
            m_TimeElapsed += Time.deltaTime;
            if (m_TimeElapsed > .5f)
            {
                m_TimeElapsed = 0;
                m_AtPeakColor = false;
            }
            m_StationLightGuts.color = m_StationLightColor[m_CurrentColor];
            m_SpotLight.color = m_LightColors[m_CurrentColor];
            m_GlowLight.color = m_LightColors[m_CurrentColor];
        }
    }

    public void RemoveAllButOwnerLight(int _owner)
    {
        m_StationLightColor.Clear();
        m_LightColors.Clear();
        AddColor(_owner);

    }

    public bool ContainsColor(int _player)
    {
        switch (_player)
        {
            case 1:
                //Green-ish 
                if (m_StationLightColor.Contains(new Color(0.397032f, 1, 0, 1)))
                {
                    return true;
                }
                break;
            case 2:
                //Blue-ish
                if (m_StationLightColor.Contains(new Color(0, 0.6313722f, 1, 1)))
                {
                    return true;
                }
                break;
            case 3:
                //Red-ish
                if (m_StationLightColor.Contains(new Color(1, 0, 0, 1)))
                {
                    return true;
                }
                break;
            case 4:
                //White-ish
                if (m_StationLightColor.Contains(new Color(1, 1, 1, 1)))
                {
                    return true;
                }
                break;
            default:
                break;
        }
        return false;
    }
}
