using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Armory : MonoBehaviour
{

    public Text m_TopAmmo;
    public Text m_BottomAmmo;
    public Text m_PilotAmmo;

    private bool m_UserControlled;
    private string m_ReloadButton;
    private float m_TimeSinceLastReload;
    private const float m_RELOAD_SPEED = 1;

    private Image m_TopAmmoImage;
    private Image m_BottomAmmoImage;
    public Image m_PilotAmmoImage;

    private GameObject m_Player;

    // Start is called before the first frame update
    void Start()
    {
        m_TimeSinceLastReload = Time.realtimeSinceStartup;

        m_TopAmmo = GameObject.Find("topAmmoText").GetComponent<Text>();
        m_TopAmmoImage = GameObject.Find("TopAmmoForeground").GetComponent<Image>();

        m_BottomAmmo = GameObject.Find("bottomAmmoText").GetComponent<Text>();
        m_BottomAmmoImage = GameObject.Find("BottomAmmoForeground").GetComponent<Image>();

        m_PilotAmmo = GameObject.Find("frontAmmoText").GetComponent<Text>();
        m_PilotAmmoImage = GameObject.Find("PilotAmmoForeground").GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();

        if (m_UserControlled && m_Player != null)
        { 
            //Only carry 1 at a time
            m_Player.GetComponent<CharacterMovement>().m_HasRepairPanel = false;
            m_Player.GetComponent<CharacterMovement>().m_RepairPanel.SetActive(false);

            m_Player.GetComponent<CharacterMovement>().m_HasAmmo = true;
            m_Player.GetComponent<CharacterMovement>().m_AmmoCrate.SetActive(true);
            m_Player.GetComponent<CharacterMovement>().ExitStation();
        }

    }

    private void UpdateText()
    {
        m_TopAmmo.text = "" + SubmarineManager.GetInstance().m_TopWeaponStation.m_AmmoCount;
        m_TopAmmoImage.fillAmount = (float)SubmarineManager.GetInstance().m_TopWeaponStation.m_AmmoCount / 100f;

        m_BottomAmmo.text = "" + SubmarineManager.GetInstance().m_BottomWeaponStation.m_AmmoCount;
        m_BottomAmmoImage.fillAmount = (float)SubmarineManager.GetInstance().m_BottomWeaponStation.m_AmmoCount / 100f;

        m_PilotAmmo.text = "" + SubmarineManager.GetInstance().m_Submarine.m_AmmoCount;
        m_PilotAmmoImage.fillAmount = (float)SubmarineManager.GetInstance().m_Submarine.m_AmmoCount / 50f;
    }

    public void SetControls(bool _userControlled, string _reloadButton, GameObject _player)
    {
        m_UserControlled = _userControlled;
        m_ReloadButton = _reloadButton;
        m_Player = _player;
    }
}
