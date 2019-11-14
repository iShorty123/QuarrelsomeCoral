using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{

    public bool m_PlayerControlled { get; private set; }
    public string m_PlayerControlScheme { get; private set; }
    
    public float m_XAxisRadius { get; private set; } //X Radius of the circles at the end of the sub we use as a reference to rotate the shield nicely
    public float m_YAxisRadius { get; private set; } //Y Radius of the circles at the end of the sub we use as a reference to rotate the shield nicely
    public const float m_END_SPEED = .011f; //The speed at which the shield travels when it is curved
    public float m_STRAIGHT_AWAY_SPEED = .17f; //As far as the eye can tell, matches the speed of the end speed (when curved) - NOT TRUE but good enough for now
    public Transform m_RightCircleLookAtPosition { get; private set; } //The middle of the circle we use on the right for looking at when rotating around it
    public Transform m_LeftCircleLookAtPosition { get; private set; } //The middle of the circle we use on the left for looking at when rotating around it

    private bool m_HitTerrainFlag;
    GameObject _23;
    GameObject _10;
    private void Start()
    {
        _23 = GameObject.Find("23");
        _10 = GameObject.Find("10");

        m_RightCircleLookAtPosition = GameObject.Find("RightCircleShieldLookAtPosition").transform;
        m_LeftCircleLookAtPosition = GameObject.Find("LeftCircleShieldLookAtPosition").transform;
        ShieldController[] m_ShieldPieces = FindObjectsOfType(typeof(ShieldController)) as ShieldController[];
        int startingAlpha = -148; //Allows semi circle on end of sub to be covered completely
        int shieldPiecesCount = 0;
        foreach (ShieldController shieldPiece in m_ShieldPieces)
        {
            shieldPiece.m_AlphaValue = startingAlpha += 6;
            //Update collision box to prevent anything getting in between the shield and the sub for the end shield pieces
            if (shieldPiecesCount == 0)
            {
                shieldPiece.GetComponent<BoxCollider2D>().size = new Vector2(2.5f, .2f);
                shieldPiece.GetComponent<BoxCollider2D>().offset = new Vector2(1.25f, 0);
            }
            else if (shieldPiecesCount == m_ShieldPieces.Length-1)
            {
                shieldPiece.GetComponent<BoxCollider2D>().size = new Vector2(2.5f, .2f);
                shieldPiece.GetComponent<BoxCollider2D>().offset = new Vector2(1.25f, 0);
            }
            shieldPiecesCount++;
        }
        m_XAxisRadius = 8;
        m_YAxisRadius = 8;
        m_HitTerrainFlag = false;
        StartCoroutine(InitializeShield());
    }

    IEnumerator InitializeShield()
    {
        m_PlayerControlled = true;
        m_PlayerControlScheme = "Horizontal_P1"; //Set to Player 1 control scheme for initialization
        yield return new WaitForEndOfFrame();
        m_PlayerControlled = false;
        m_PlayerControlScheme = string.Empty; //Set to Player 1 control scheme for initialization
        yield break;
    }

    public void SetControls(bool _underPlayerControl, string _controlScheme)
    {
        m_PlayerControlled = _underPlayerControl;
        m_PlayerControlScheme = _controlScheme;
    }

    public IEnumerator TerrainShieldImpactCooldown()
    {
        //Wait for 5 frames - short enough to prevent multiple collisions
        //Long enough to allow calculations
        for (int i = 0; i < 1; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        m_HitTerrainFlag = false;
    }

    public void ShieldCollidiedWithTerrain(Vector2 _shieldPiece)
    {
        if (m_HitTerrainFlag) { return; } //Prevent multiple shield collisions

        m_HitTerrainFlag = true;
        //Start collision cooldown
        StartCoroutine(TerrainShieldImpactCooldown());

        //Zero out velocity before we push back
        SubmarineManager.GetInstance().m_Submarine.m_RigidBody.velocity = Vector2.zero;

        //Get direction we should push in (approximately away from wall)
        Vector2 direction = -(_shieldPiece - new Vector2(
                SubmarineManager.GetInstance().m_Submarine.m_RigidBody.position.x,
                SubmarineManager.GetInstance().m_Submarine.m_RigidBody.position.y)).normalized;

        //Push back sub
        SubmarineManager.GetInstance().m_Submarine.m_RigidBody.AddForce(direction * SubmarineManager.GetInstance().m_SubmarineTerrianBounceBackForce);
    }

}
