using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    public bool m_PlayerControlled;
    public StationLightAdjuster m_StationLight;


    // Start is called before the first frame update
    void Start()
    {
        m_PlayerControlled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !m_PlayerControlled)
        {
            if (collision.name.Contains("1")) //Player 1
            {
                m_StationLight.AddColor(1);
            }
            else if (collision.name.Contains("2")) //Player 2
            {
                m_StationLight.AddColor(2);
            }
            else if (collision.name.Contains("3")) //Player 3
            {
                m_StationLight.AddColor(3);
            }
            else if (collision.name.Contains("4")) //Player 4
            {
                m_StationLight.AddColor(4);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.tag == "Player" && !m_PlayerControlled)
        {
            if (collision.name.Contains("1")) //Player 1
            {
                if (!m_StationLight.ContainsColor(1))
                {
                    m_StationLight.AddColor(1);
                }               
            }
            else if (collision.name.Contains("2")) //Player 2
            {
                if (!m_StationLight.ContainsColor(2))
                {
                    m_StationLight.AddColor(2);
                }
            }
            else if (collision.name.Contains("3")) //Player 3
            {
                if (!m_StationLight.ContainsColor(3))
                {
                    m_StationLight.AddColor(3);
                }
            }
            else if (collision.name.Contains("4")) //Player 4
            {
                if (!m_StationLight.ContainsColor(4))
                {
                    m_StationLight.AddColor(4);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.name.Contains("1")) //Player 1
            {
                m_StationLight.RemoveColor(1);
            }
            else if (collision.name.Contains("2")) //Player 2
            {
                m_StationLight.RemoveColor(2);
            }
            else if (collision.name.Contains("3")) //Player 3
            {
                m_StationLight.RemoveColor(3);
            }
            else if (collision.name.Contains("4")) //Player 4
            {
                m_StationLight.RemoveColor(4);
            }
        }
    }

    public void RemoveAllPlayerLightsExceptControllers(int _player)
    {
        m_StationLight.RemoveAllButOwnerLight(_player);
    }
    public void AddNoPlayerLight(int _player)
    {
        //m_StationLight.AddColor(_player);
        m_StationLight.AddColor(0);
        
    }

}
