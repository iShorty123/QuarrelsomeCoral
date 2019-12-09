using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmarineManager : MonoBehaviour
{
    private static SubmarineManager m_Instance;

    public SubmarineController m_Submarine;
    public int m_SubmarineTerrianBounceBackForce { get; private set; }

    public GunController m_TopWeaponStation;
    public GunController m_BottomWeaponStation;
    public SubmarineController m_PilotStation;
    public ShieldManager m_Shield;
    public MiniMap m_MiniMap;
    public Repair m_RepairStation;
    public Armory m_ArmoryStation;
    public GameObject m_TopAndBottomAmmo;
    public GameObject m_HomingAmmo;

    public GameOverScript m_GameOverScript;
    public MainMenu m_MainMenu;

    public bool m_Died;

    public const string PILOT_STATION = "PilotStation";
    public const string TOP_WEAPON_STATION = "TopWeaponStation";
    public const string BOTTOM_WEAPON_STATION = "BottomWeaponStation";
    public const string SHIELD_STATION = "ShieldStation";
    public const string MAP_STATION = "MapStation";
    public const string REPAIR_STATION = "RepairStation";
    public const string ARMORY_STATION = "ArmoryStation";

    private Text m_ScoreText;
    private bool m_CalledHighScoreList;
    public int m_Score;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        m_SubmarineTerrianBounceBackForce = 1000;
        m_TopWeaponStation.SetWeaponSpecificVariables(m_TopAndBottomAmmo, 0, -2.5f, .5f, 1.2f);
        m_BottomWeaponStation.SetWeaponSpecificVariables(m_TopAndBottomAmmo, 180, 2.5f, .5f, 1.2f);
        m_PilotStation.SetWeaponSpecificVariables(m_HomingAmmo, 1.5f, 1.2f);

        m_ScoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        m_Died = false;
        m_CalledHighScoreList = false;
    }

    private void Update()
    {
        m_ScoreText.text = "" + m_Score;
        if (m_Died && !m_CalledHighScoreList)
        {
            StartCoroutine(m_GameOverScript.GetComponentInChildren<HSController>().GetScores());
            m_CalledHighScoreList = true;
        }
    }

    public static SubmarineManager GetInstance()
    {
        if (m_Instance != null)
        {
            return m_Instance;
        }
        else
        {
            //Debug.LogError("Submarine Manager is not initialized.");
        }
        return null;
    }




        
}
