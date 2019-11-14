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
    private bool m_CurrentlyFullScreen;
    private Rect m_MiniMapRect = new Rect(.75f, .8f, .25f, .2f);
    private Rect m_FullMapRect = new Rect(.05f, .05f, .9f, .9f);
    // Start is called before the first frame update
    void Start()
    {
        m_CurrentlyFullScreen = false;
        m_MiniMapCamera = GetComponentInChildren<Camera>();
        TurnOffMiniMap();
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
    }

    private void TurnOffMiniMap()
    {
        m_MiniMapCamera.enabled = false;
    }

    private void SetFullScreen()
    {
        if (m_CurrentlyFullScreen) 
        {
            m_MiniMapCamera.rect = m_FullMapRect;
            m_MiniMapCamera.orthographicSize = 50;
        }
        else 
        { 
            m_MiniMapCamera.rect = m_MiniMapRect;
            m_MiniMapCamera.orthographicSize = 20;
        }
    }
    private void SetPosition()
    {
        //Set camera position to sub location
        transform.position = new Vector3(SubmarineManager.GetInstance().m_Submarine.transform.position.x, SubmarineManager.GetInstance().m_Submarine.transform.position.y, transform.position.z);
        //Clamp camera position to bottom of the world (bottom is currently hard coded for both map versions)
        if (m_CurrentlyFullScreen)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -9, 100), transform.position.z);
        }
        else if (!m_CurrentlyFullScreen)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -40, 100), transform.position.z);         
        }
    }
}
