using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    private bool m_PlayerControlled;
    private string m_HorizontalControls;
    private string m_VerticalControls;
    private string m_FullScreenButton;
    private Camera m_MiniMapCamera;
    private Camera m_MiniMapBorderCamera;
    private bool m_CurrentlyFullScreen;
    private Rect m_MiniMapRect = new Rect(.75f, .8f, .25f, .2f);
    private Rect m_FullMapRect = new Rect(.05f, .05f, .9f, .9f);
    private float m_MapSpeed;
    private Vector3 m_Offset;

    private float m_FullMapMinimumY;
    private float m_MiniMapMinimumY;
    private float m_FullMapMaximumY;
    private float m_MiniMapMaximumY;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentlyFullScreen = false;
        m_MiniMapCamera = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
        m_MiniMapBorderCamera = GameObject.Find("MiniMapCameraBorder").GetComponent<Camera>();
        m_MapSpeed = 5;
        TurnOffMiniMap();

        m_FullMapMinimumY = -10.5F;
        m_FullMapMaximumY = 50;
        m_MiniMapMinimumY = -40;      
        m_MiniMapMaximumY = 79.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_PlayerControlled)
        {
            TurnOnMiniMap();
            if (Input.GetButtonDown(m_FullScreenButton))
            {
                m_CurrentlyFullScreen = !m_CurrentlyFullScreen;
                SetFullScreen();
            }

            m_Offset += new Vector3(Input.GetAxis(m_HorizontalControls) * Time.deltaTime * m_MapSpeed, Input.GetAxis(m_VerticalControls) * Time.deltaTime * m_MapSpeed, 0);
            SetPosition();
            
        }
        else
        {
            TurnOffMiniMap();
            m_CurrentlyFullScreen = false;
            SetFullScreen();
        }
    }

    public void SetControls(bool _playerControlled, string _horizontalControls, string _verticalControls, string _fullScreenButton)
    {
        m_PlayerControlled = _playerControlled;
        m_HorizontalControls = _horizontalControls;
        m_VerticalControls = _verticalControls;
        m_FullScreenButton = _fullScreenButton;
    }

    private void TurnOnMiniMap()
    {
        m_MiniMapCamera.enabled = true;
        m_MiniMapBorderCamera.enabled = true;
    }

    private void TurnOffMiniMap()
    {
        m_MiniMapCamera.enabled = false;
        m_MiniMapBorderCamera.enabled = false;
    }

    private void SetFullScreen()
    {
        if (m_CurrentlyFullScreen) 
        {
            //Adjust mini map
            m_MiniMapCamera.rect = m_FullMapRect;
            m_MiniMapCamera.orthographicSize = 50;

            //Keep border aligned
            m_MiniMapBorderCamera.rect = m_FullMapRect;
            m_MiniMapBorderCamera.orthographicSize = 50;
        }
        else 
        {
            //Adjust mini map
            m_MiniMapCamera.rect = m_MiniMapRect;
            m_MiniMapCamera.orthographicSize = 20;

            //Keep border aligned
            m_MiniMapBorderCamera.rect = m_MiniMapRect;
            m_MiniMapBorderCamera.orthographicSize = 20;
        }
        //Reset Position to avoid confusion
        m_Offset = Vector3.zero;
        SetPosition();

    }

    private void SetPosition()
    {
        //Set camera position to sub location
        m_MiniMapCamera.transform.position = new Vector3(SubmarineManager.GetInstance().m_Submarine.transform.position.x, SubmarineManager.GetInstance().m_Submarine.transform.position.y, -10)
            + m_Offset;

        //Clamp camera position to bottom of the world (bottom is currently hard coded for both map versions)
        if (m_CurrentlyFullScreen)
        {
            //if offset + pos is below minimum, set to offset accordingly
            if (m_Offset.y + Mathf.Clamp(m_MiniMapCamera.transform.position.y, m_FullMapMinimumY, m_FullMapMaximumY) < m_FullMapMinimumY)
            {
                m_Offset.y = m_FullMapMinimumY - Mathf.Clamp(m_MiniMapCamera.transform.position.y, m_FullMapMinimumY, m_FullMapMaximumY);
            }
            //if off + pos is above maximum, set offset accordingly
            if (m_Offset.y + Mathf.Clamp(m_MiniMapCamera.transform.position.y, m_FullMapMinimumY, m_FullMapMaximumY) > m_FullMapMaximumY)
            {
                m_Offset.y = m_FullMapMaximumY - Mathf.Clamp(m_MiniMapCamera.transform.position.y, m_FullMapMinimumY, m_FullMapMaximumY);
            }
            //Set new position based on any clamping that needed to occur
            m_MiniMapCamera.transform.position = new Vector3(m_MiniMapCamera.transform.position.x, Mathf.Clamp(m_MiniMapCamera.transform.position.y, m_FullMapMinimumY, m_FullMapMaximumY), -10)
                + m_Offset;
        }
        else if (!m_CurrentlyFullScreen)
        {
            //if offset + pos is below minimum, set to offset accordingly
            if (m_Offset.y + Mathf.Clamp(m_MiniMapCamera.transform.position.y, m_MiniMapMinimumY, m_MiniMapMaximumY) < m_MiniMapMinimumY)
            {
                m_Offset.y = m_MiniMapMinimumY - Mathf.Clamp(m_MiniMapCamera.transform.position.y, m_MiniMapMinimumY, m_MiniMapMaximumY);
            }
            //if off + pos is above maximum, set offset accordingly
            if (m_Offset.y + Mathf.Clamp(m_MiniMapCamera.transform.position.y, m_MiniMapMinimumY, m_MiniMapMaximumY) > m_MiniMapMaximumY)
            {
                m_Offset.y = m_MiniMapMaximumY - Mathf.Clamp(m_MiniMapCamera.transform.position.y, m_MiniMapMinimumY, m_MiniMapMaximumY);
            }
            //Set new position based on any clamping that needed to occur
            m_MiniMapCamera.transform.position = new Vector3(m_MiniMapCamera.transform.position.x, Mathf.Clamp(m_MiniMapCamera.transform.position.y, m_MiniMapMinimumY, m_MiniMapMaximumY), -10)
                + m_Offset;
        }

        //Keep border aligned
        m_MiniMapBorderCamera.transform.position = m_MiniMapCamera.transform.position;
    }
}
