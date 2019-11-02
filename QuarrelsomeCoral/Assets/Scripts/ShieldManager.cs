using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    private static ShieldManager m_ShieldManagerInstance = null;
    public bool m_PlayerControlled = false;
    public string m_PlayerControlScheme;
    public static ShieldManager GetShieldManagerInstance()
    {
        if (m_ShieldManagerInstance != null)
        {
            return m_ShieldManagerInstance;
        }
        else
        {
            Debug.LogError("ShieldManager is not initialized!");
        }
        return null;
    }

    private void Awake()
    {
        m_ShieldManagerInstance = this; 
        //If we ever add levels, we will need DontDestroyOnLoad(gameObject);
    }
}
