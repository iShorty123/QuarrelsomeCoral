using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineManager : MonoBehaviour
{
    private static SubmarineManager m_Instance;

    public SubmarineController m_Submarine;

    public GunController m_TopWeaponStation;
    public GunController m_BottomWeaponStation;
    public ShieldManager m_Shield;
    public GameObject m_MapStation;
    public GameObject m_ArmoryStation;
    public GameObject TopAndBottomAmmo;

    public const string PILOT_STATION = "PilotStation";
    public const string TOP_WEAPON_STATION = "TopWeaponStation";
    public const string BOTTOM_WEAPON_STATION = "BottomWeaponStation";
    public const string SHIELD_STATION = "ShieldStation";
    public const string MAP_STATION = "MapStation";
    public const string ARMORY_STATION = "ArmoryStation";

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        m_TopWeaponStation.SetWeaponSpecificVariables(TopAndBottomAmmo, 0, -1, .5f, 1.2f);
        m_BottomWeaponStation.SetWeaponSpecificVariables(TopAndBottomAmmo, 180, 1, .5f, 1.2f);
    }

    public static SubmarineManager GetInstance()
    {
        if (m_Instance != null)
        {
            return m_Instance;
        }
        else
        {
            Debug.LogError("Submarine Manager is not initialized.");
        }
        return null;
    }




        
}
