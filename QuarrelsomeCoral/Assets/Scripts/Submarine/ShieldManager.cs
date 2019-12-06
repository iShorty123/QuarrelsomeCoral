using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{

    public bool m_PlayerControlled { get; private set; }
    public string m_PlayerControlScheme { get; private set; }   
    public float m_XAxisRadius { get; private set; } //X Radius of the circles at the end of the sub we use as a reference to rotate the shield nicely
    public float m_YAxisRadius { get; private set; } //Y Radius of the circles at the end of the sub we use as a reference to rotate the shield nicely
    public float m_Speed { get; private set; } //Shield speed

    private bool m_HitTerrainFlag;

    private void Start()
    {
        ShieldController[] m_ShieldPieces = FindObjectsOfType(typeof(ShieldController)) as ShieldController[];
        int startingAlpha = -38; //Allows semi circle on end of sub to be covered completely -147
        int shieldPiecesCount = 0;
        foreach (ShieldController shieldPiece in m_ShieldPieces)
        {
            shieldPiece.m_AlphaValue = startingAlpha += 1;
            //Update collision box to prevent anything getting in between the shield and the sub for the end shield pieces
            if (shieldPiecesCount == 0)
            {
                shieldPiece.GetComponent<CapsuleCollider2D>().size = new Vector2(3.15f, .5f);
                shieldPiece.GetComponent<CapsuleCollider2D>().offset = new Vector2(1.5f, 0);
                shieldPiece.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
                CreateShieldLight(shieldPiece.gameObject);
            }
            else if (shieldPiecesCount == m_ShieldPieces.Length-1)
            {
                shieldPiece.GetComponent<CapsuleCollider2D>().size = new Vector2(3.15f, .5f);
                shieldPiece.GetComponent<CapsuleCollider2D>().offset = new Vector2(1.5f, 0);
                shieldPiece.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
                CreateShieldLight(shieldPiece.gameObject);
            }
            else if (shieldPiecesCount % ((m_ShieldPieces.Length) /4) == 0) //if 1 of three points between 0 and 76
            {
                CreateShieldLight(shieldPiece.gameObject);
            }
            shieldPiecesCount++;
        }
        m_XAxisRadius = 30;
        m_YAxisRadius = 11;
        m_Speed = 25;
        m_HitTerrainFlag = false;
        StartCoroutine(InitializeShield());
    }

    IEnumerator InitializeShield()
    {
        m_PlayerControlled = true;
        m_PlayerControlScheme = "Horizontal_P1"; //Set to Player 1 control scheme for initialization
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        m_PlayerControlled = false;
        m_PlayerControlScheme = string.Empty; //Set to Player 1 control scheme for initialization
        yield break;
    }

    private void CreateShieldLight(GameObject _shieldPiece)
    {
        GameObject lightObject = new GameObject();
        lightObject.transform.parent = _shieldPiece.transform;
        lightObject.transform.localPosition = new Vector3(0, 0, -2);
        lightObject.transform.localScale = Vector3.one;
        lightObject.AddComponent<Light>().type = LightType.Point;
        lightObject.name = "ShieldLight";
        lightObject.GetComponent<Light>().intensity = 0.75f;
        lightObject.GetComponent<Light>().range = 6;
        lightObject.GetComponent<Light>().cullingMask ^= 1 << (LayerMask.NameToLayer("Player"));
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
