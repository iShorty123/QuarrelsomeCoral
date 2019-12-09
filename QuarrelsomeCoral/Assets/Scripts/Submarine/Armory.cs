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

    // Start is called before the first frame update
    void Start()
    {
        m_TimeSinceLastReload = Time.realtimeSinceStartup;


        m_TopAmmo = GameObject.Find("topAmmoText").GetComponent<Text>();
        m_BottomAmmo = GameObject.Find("bottomAmmoText").GetComponent<Text>();
        m_PilotAmmo = GameObject.Find("frontAmmoText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();

        if (m_UserControlled && m_TimeSinceLastReload + m_RELOAD_SPEED < Time.realtimeSinceStartup)
        {
            int smallestCount = int.MaxValue;
            GameObject stationToReload = null;
            if (SubmarineManager.GetInstance().m_TopWeaponStation.m_AmmoCount < smallestCount && SubmarineManager.GetInstance().m_TopWeaponStation.m_AmmoCount < 100)
            {
                smallestCount = SubmarineManager.GetInstance().m_TopWeaponStation.m_AmmoCount;
                stationToReload = SubmarineManager.GetInstance().m_TopWeaponStation.gameObject;
            }
            if (SubmarineManager.GetInstance().m_BottomWeaponStation.m_AmmoCount <= smallestCount && SubmarineManager.GetInstance().m_BottomWeaponStation.m_AmmoCount < 100)
            {
                smallestCount = SubmarineManager.GetInstance().m_BottomWeaponStation.m_AmmoCount;
                stationToReload = SubmarineManager.GetInstance().m_BottomWeaponStation.gameObject;
            }
            if (SubmarineManager.GetInstance().m_PilotStation.m_AmmoCount <= smallestCount && SubmarineManager.GetInstance().m_PilotStation.m_AmmoCount < 50)
            {
                smallestCount = SubmarineManager.GetInstance().m_PilotStation.m_AmmoCount;
                stationToReload = SubmarineManager.GetInstance().m_PilotStation.gameObject;
            }
            if (stationToReload != null)
            {
                if (stationToReload.GetComponent<GunController>())
                {
                    if (stationToReload.GetComponent<GunController>().m_AmmoCount + 3 > 100) { stationToReload.GetComponent<GunController>().m_AmmoCount = 100; }
                    else { stationToReload.GetComponent<GunController>().m_AmmoCount += 3; }
                }
                else
                {
                    if (stationToReload.GetComponent<SubmarineController>().m_AmmoCount + 1 > 50) { stationToReload.GetComponent<SubmarineController>().m_AmmoCount = 50; }
                    else { stationToReload.GetComponent<SubmarineController>().m_AmmoCount += 1; }
                }
                m_TimeSinceLastReload = Time.realtimeSinceStartup;

            }
            
        }

    }

    private void UpdateText()
    {
        m_TopAmmo.text = "" + SubmarineManager.GetInstance().m_TopWeaponStation.m_AmmoCount;
        m_BottomAmmo.text = "" + SubmarineManager.GetInstance().m_BottomWeaponStation.m_AmmoCount;
        m_PilotAmmo.text = "" + SubmarineManager.GetInstance().m_Submarine.m_AmmoCount;
    }

    public void SetControls(bool _userControlled, string _reloadButton)
    {
        m_UserControlled = _userControlled;
        m_ReloadButton = _reloadButton;

    }
}
