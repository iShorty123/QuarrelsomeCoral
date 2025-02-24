﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineController : MonoBehaviour, ITakeDamage
{
    public Rigidbody2D m_RigidBody;
    public float m_Speed;
    public int m_Health;
    public int m_MaxHealth;
    private bool m_HitTerrainFlag;
    private bool m_HasPilot;
    private string m_HorizontalControls;
    private string m_VerticalControls;
    private string m_FireButton;
    private GameObject m_PilotWeapon;
    public GameObject m_Bubbles;
    private float m_MaxSpeed;

    private GameObject m_Ammo;
    private float m_TurretLength;
    private float m_TimeAtLastShot;
    private float m_FireRate;

    public int m_AmmoCount;

    public List<GameObject> m_Cracks = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        m_PilotWeapon = GameObject.Find("PilotWeapon");
        m_MaxSpeed = 5;
        m_HitTerrainFlag = false;
        m_MaxHealth = m_Health = 100;
        m_AmmoCount = 50;
        m_Bubbles.SetActive(false);

        foreach (var go in GameObject.FindGameObjectsWithTag("ActualCrack"))
        {
            m_Cracks.Add(go);
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_RigidBody.position; //Move sub via the SubmarineRigibody object
        //Kelvin Mode:
        if (Input.GetKeyUp(KeyCode.B)) { m_Health = m_MaxHealth = 1000; }

    }


    private void FixedUpdate()
    {
        if (m_HasPilot)
        {
            MoveSubmarine();

            if (CanFire() && m_AmmoCount > 0)
            {
                if (Input.GetButton(m_FireButton))
                {
                    Instantiate(m_Ammo, m_PilotWeapon.transform.position + m_PilotWeapon.transform.up * m_TurretLength, Quaternion.identity, m_PilotWeapon.transform);
                    m_TimeAtLastShot = Time.realtimeSinceStartup;
                    m_AmmoCount--;
                }
            }

        }
    }

    private void MoveSubmarine()
    {

        float moveHorizontally = Input.GetAxis(m_HorizontalControls);
        float moveVertically = Input.GetAxis(m_VerticalControls);
        //Move submarine via MovePosition(world position translation)

        if (moveHorizontally < 0 && m_RigidBody.velocity.x > -m_MaxSpeed) //If want to go left and can go left
        {
            m_RigidBody.AddForce(new Vector2(moveHorizontally * m_Speed, 0)); //Move left
            m_Bubbles.SetActive(true);
            m_Bubbles.transform.localPosition = new Vector3(28.45f, 0, 0);
            m_Bubbles.transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
        }
        else if (moveHorizontally > 0 && m_RigidBody.velocity.x < m_MaxSpeed) //If want to go right and can go right
        {
            m_RigidBody.AddForce(new Vector2(moveHorizontally * m_Speed, 0));
            m_Bubbles.SetActive(true);
            m_Bubbles.transform.localPosition = new Vector3(-28.45f, 0, 0);
            m_Bubbles.transform.localRotation = Quaternion.AngleAxis(-90, Vector3.forward);
            
        }
        else
        {
            m_Bubbles.SetActive(false);
        }

        if (moveVertically < 0 && m_RigidBody.velocity.y > -m_MaxSpeed) //If want to go down and can go down
        {
            m_RigidBody.AddForce(new Vector2(0, (moveVertically * m_Speed)));            
        }
        else if (moveVertically > 0 && m_RigidBody.velocity.y < m_MaxSpeed) //If want to go up and can go up
        {
            m_RigidBody.AddForce(new Vector2(0, (moveVertically * m_Speed)));
        }
    }

    private bool CanFire()
    {
        if (Time.realtimeSinceStartup - m_TimeAtLastShot > m_FireRate)
        {
            return true;
        }
        return false;
    }




    public void SetControls(bool _hasPilot, string _horizontalControls, string _verticalControls, string _fireButton)
    {
        m_HasPilot = _hasPilot;
        m_HorizontalControls = _horizontalControls;
        m_VerticalControls = _verticalControls;
        m_FireButton = _fireButton;
    }

    public void SetWeaponSpecificVariables(GameObject _ammoToUse, float _fireRate, float _turretLength)
    {
        m_Ammo = _ammoToUse;
        m_FireRate = _fireRate;
        m_TurretLength = _turretLength;
    }

    public void TakeDamage(int _damage)
    {
        foreach (GameObject _crack in m_Cracks)
        {
            if (!_crack.activeSelf)
            {
                _crack.SetActive(true);
                _crack.GetComponent<Crack>().m_LocalHealth += _damage;
                break;
            }
            else if (_crack.GetComponent<Crack>().m_LocalHealth + _damage < _crack.GetComponent<Crack>().m_MaxLocalHealth)
            {
                _crack.GetComponent<Crack>().m_LocalHealth += _damage;
                break;
            }
            else
            {
                if (_crack.GetComponent<Crack>().m_LocalHealth != _crack.GetComponent<Crack>().m_MaxLocalHealth)
                {
                    _damage = _crack.GetComponent<Crack>().m_LocalHealth - _crack.GetComponent<Crack>().m_MaxLocalHealth + _damage;
                    m_Health -= Mathf.Abs(_crack.GetComponent<Crack>().m_LocalHealth - _crack.GetComponent<Crack>().m_MaxLocalHealth);
                    _crack.GetComponent<Crack>().m_LocalHealth = _crack.GetComponent<Crack>().m_MaxLocalHealth;
                }              
            }
        }

        Shuffle(m_Cracks);

        m_Health -= _damage;
        if (m_Health <= 0)
        {
            SubmarineManager.GetInstance().m_GameOverScript.gameObject.SetActive(true);
            GameOverScript.m_GameOverFlag = true;
            SubmarineManager.GetInstance().m_Died = true;
            Debug.Log("DEAD");
        }
    }

    public void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public IEnumerator CollidedWithTerrainCooldown()
    {
        yield return new WaitForFixedUpdate();
        m_HitTerrainFlag = false;
    }

    public void CollidedWithTerrain(ContactPoint2D _impactPoint)
    {
        Debug.Log("Terrain");
        if (m_HitTerrainFlag) { return; } //Prevent multiple shield collisions
        m_HitTerrainFlag = true;

        //Start collision cooldown
        StartCoroutine(CollidedWithTerrainCooldown());

        //Zero out velocity before we push back - Do on shield due to weird behavior if not, but here seems ok not to do that
        //m_RigidBody.velocity = Vector2.zero;

        //Get direction we should push in (approximately away from wall)
        Vector2 direction = -(_impactPoint.point - new Vector2(m_RigidBody.position.x, m_RigidBody.position.y)).normalized;

        //Push back sub
        SubmarineManager.GetInstance().m_Submarine.m_RigidBody.AddForce(direction * SubmarineManager.GetInstance().m_SubmarineTerrianBounceBackForce);

        //Deal small damage to the Sub
        TakeDamage(1);
    }



}

