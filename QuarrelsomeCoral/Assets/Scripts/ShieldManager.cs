using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{

    public bool m_PlayerControlled { get; private set; }
    public string m_PlayerControlScheme { get; private set; }

    public float m_XAxisRadius; //Can change to private once we know these are the values we want
    public float m_YAxisRadius; //Can change to private once we know these are the values we want
    public float m_Speed; //Can change to private once we know these are the values we want

    private void Start()
    {
        m_PlayerControlled = false;
        m_PlayerControlScheme = string.Empty;
        m_XAxisRadius = 12;
        m_YAxisRadius = 6;
        m_Speed = .01f;
    }

    public void SetControls(bool _underPlayerControl, string _controlScheme)
    {
        m_PlayerControlled = _underPlayerControl;
        m_PlayerControlScheme = _controlScheme;
    }

}
