using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : MonoBehaviour
{


    private bool m_UserControlled;
    private float m_TimeSinceLastRepair;
    private const float m_REPAIR_SPEED = 1;
    private string m_GrabScrapButton;
    private GameObject m_Player;

    // Start is called before the first frame update
    void Start()
    {
        m_TimeSinceLastRepair = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_UserControlled && m_Player != null)
        {
            //Only carry 1 at a time
            m_Player.GetComponent<CharacterMovement>().m_HasAmmo = false;
            m_Player.GetComponent<CharacterMovement>().m_AmmoCrate.SetActive(false);

            m_Player.GetComponent<CharacterMovement>().m_HasRepairPanel = true;
            m_Player.GetComponent<CharacterMovement>().m_RepairPanel.SetActive(true);
            m_Player.GetComponent<CharacterMovement>().ExitStation();
        }
    }

    public void SetControls(bool _userControlled, string _grabScrapButton, GameObject _player)
    {
        m_UserControlled = _userControlled;
        m_GrabScrapButton = _grabScrapButton;
        m_Player = _player;
    }
}
