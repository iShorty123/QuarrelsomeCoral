using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    public GameObject m_EnterText;
    public bool m_PlayerControlled;

    // Start is called before the first frame update
    void Start()
    {
        m_EnterText.SetActive(false);
        m_PlayerControlled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!m_PlayerControlled) //If someone is in the station, don't show the enter text
            {
                m_EnterText.SetActive(true);
            }
            else //If no one is in the station, show the text to enter
            {
                m_EnterText.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            m_EnterText.SetActive(false);
        }
    }

    public void SetTutorialText()
    {
        m_PlayerControlled = !m_PlayerControlled;
        if (!m_PlayerControlled) //If someone is in the station, don't show the enter text
        {
            m_EnterText.SetActive(true);
        }
        else //If no one is in the station, show the text to enter
        {
            m_EnterText.SetActive(false);
        }
    }
}
