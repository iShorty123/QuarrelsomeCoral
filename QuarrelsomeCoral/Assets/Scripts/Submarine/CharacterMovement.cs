using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public int m_Player;
    public float m_Speed;
    public float m_SubmarineSpeed;

    private bool m_OnLaddder;
    private Rigidbody2D m_RigidBody;
    private SpriteRenderer m_SpriteRenderer;
    private GameObject m_ClosestStation;
    private GameObject m_CurrentStation;
    private bool m_CanMove;
    private bool m_InStation;
    private bool m_AtTopOfLadder = true;
    private bool m_AtBottomOfLadder;
    private const float DISTANCE_ALLOWED_BETWEEN_WALL_AND_PLAYER = 0.1f;
    private Animator m_Animator;
    private bool m_CanMoveRight, m_CanMoveLeft;
    public bool m_HasAmmo;
    public GameObject m_AmmoCrate;
    public bool m_HasRepairPanel;
    public GameObject m_RepairPanel;


    private string m_Horizontal = "Horizontal_P";
    private string m_Vertical = "Vertical_P";
    private string m_Action1 = "Action1_P";
    private string m_Action2 = "Action2_P";

    // Start is called before the first frame update
    void Start()
    {
        MapPlayerControls();


        m_CanMove = true;
        m_Speed = 5;
        m_OnLaddder = false;
        m_InStation = false;
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_ClosestStation = null;
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("Move", false);
        m_Animator.SetBool("Ladder", false);
        m_CanMoveRight = true;
        m_CanMoveLeft = true;
        m_HasAmmo = false;
        m_AmmoCrate.SetActive(false);
        m_HasRepairPanel = false;
        m_RepairPanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        EnterOrExitStation();    
    }

    private void FixedUpdate()
    {
        if (m_CanMove)
        {
            MoveCharacter();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Submarine") // Pushing again sub walls
        {
        }
        else if (collision.name == "FirstFloor")
        {
            m_AtBottomOfLadder = true;
            m_AtTopOfLadder = false;
        }
        else if (collision.name == "SecondFloor")
        {
            m_AtTopOfLadder = true;
            m_AtBottomOfLadder = false;
        }
        else if (collision.tag == "Ladder")
        {
            m_OnLaddder = true;
        }
             
        else if (collision.tag == "Station")
        {
            m_ClosestStation = collision.gameObject;
            CheckIfAmmoStation();
        }
        else if (collision.tag == "Crack")
        {
            CheckIfCanRepair(collision.gameObject);
        }
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "FirstFloor")
        {
            m_AtBottomOfLadder = true;
            if (!m_OnLaddder || m_RigidBody.velocity.y < 0)
            {
                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, 0);//Remove y velocity
            }
        }
        else if (collision.name == "SecondFloor")
        {
            m_AtTopOfLadder = true;
            if (!m_OnLaddder || m_RigidBody.velocity.y > 0)
            {
                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, 0); //Remove y velocity
            }
        }
        else if (collision.tag == "Station")
        {
            CheckIfAmmoStation();
        }
        else if (collision.tag == "Crack")
        {
            CheckIfCanRepair(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "FirstFloor")
        {
            m_AtBottomOfLadder = false;
        }
        else if (collision.name == "SecondFloor")
        {
            m_AtTopOfLadder = false;
        }
        else if (collision.tag == "Ladder")
        {
            m_OnLaddder = false;
        }
        if (collision.tag == "Station")
        {
            m_ClosestStation = null;
            CheckIfAmmoStation();
        }
        else if (collision.tag == "Crack")
        {
            CheckIfCanRepair(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "LeftWall")
        {
            m_CanMoveLeft = false;
        }
        if (collision.collider.tag == "RightWall")
        {
            m_CanMoveRight = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "LeftWall")
        {
            m_CanMoveLeft = true;
        }
        if (collision.collider.tag == "RightWall")
        {
            m_CanMoveRight = true;
        }
    }

    private void FlipSprite(SpriteRenderer _mySprite, float _direction)
    {
        if (_direction > 0)
        {           
            _mySprite.flipX = false;
        }
        else if (_direction < 0)
        {            
           _mySprite.flipX = true;
        }
    }

    private void MoveCharacter()
    {
        //Horizontally
        float moveHorizontally = Input.GetAxis(m_Horizontal);
        float moveVertically = Input.GetAxis(m_Vertical);
        FlipSprite(m_SpriteRenderer, moveHorizontally);

        //Prevent moving through a wall
        if ((moveHorizontally > 0 && !m_CanMoveRight) || (moveHorizontally < 0 && !m_CanMoveLeft)) { moveHorizontally = 0;}

        //Prevent moving too high or too low
        if (transform.localPosition.y >= 0.02f && moveVertically > 0) { moveVertically = 0; }
        if (transform.localPosition.y <= -6.07f && moveVertically < 0) { moveVertically = 0; }
        if (transform.localPosition.y >= 0.02f) { transform.localPosition = new Vector3(transform.localPosition.x, 0.02f, 0); }
        if (transform.localPosition.y <= -6.07f) { transform.localPosition = new Vector3(transform.localPosition.x, -6.07f, 0); }
        

        if (m_AtTopOfLadder || m_AtBottomOfLadder)
        {
            m_RigidBody.velocity = new Vector2(moveHorizontally * m_Speed, 0);
        }           
        
        if (m_OnLaddder) //Vertically
        {
            if (!m_AtTopOfLadder && !m_AtBottomOfLadder) //If we're only on the ladder, prevent sideways movement
            {
                moveHorizontally = 0;
            }
            if (moveVertically > 0 && !m_AtTopOfLadder) //Want to go up and can go up
            {
                m_RigidBody.velocity = new Vector2(moveHorizontally * m_Speed, (moveVertically *  m_Speed));            
            }
            else if (moveVertically < 0 && !m_AtBottomOfLadder) //Want to go down and can go down
            {
                m_RigidBody.velocity = new Vector2(moveHorizontally * m_Speed, (moveVertically * m_Speed));
            }
            else if (moveVertically == 0) //Prevent player from floating up or down the ladder slowly
            {
                m_RigidBody.velocity = new Vector2(moveHorizontally * m_Speed, (moveVertically * m_Speed));
            }
        }
        else if (!m_AtTopOfLadder && !m_AtBottomOfLadder)
        {
            m_RigidBody.velocity = new Vector2(moveHorizontally * m_Speed, -m_Speed); //Fall down until on the first or second floor
        }

        if (moveHorizontally > 0 && m_AmmoCrate.activeSelf)
        {
            m_AmmoCrate.transform.localPosition = new Vector3(-4, 14, 0);
        }
        else if (moveHorizontally < 0 && m_AmmoCrate.activeSelf)
        {
            m_AmmoCrate.transform.localPosition = new Vector3(4, 14, 0);
        }
        else if (moveHorizontally == 0 && m_AmmoCrate.activeSelf && m_OnLaddder)
        {
            m_AmmoCrate.transform.localPosition = new Vector3(0, 14, 0);
        }

        if (moveHorizontally > 0 && m_RepairPanel.activeSelf)
        {
            m_RepairPanel.transform.localPosition = new Vector3(-4, 14, 0);
        }
        else if (moveHorizontally < 0 && m_RepairPanel.activeSelf)
        {
            m_RepairPanel.transform.localPosition = new Vector3(4, 14, 0);
        }
        else if (moveHorizontally == 0 && m_RepairPanel.activeSelf && m_OnLaddder)
        {
            m_RepairPanel.transform.localPosition = new Vector3(0, 14, 0);
        }

        SetWalkingAnimation();
    }

    private void SetWalkingAnimation()
    {
        m_Animator.enabled = true;
        if (m_RigidBody.velocity.x != 0)
        {
            m_Animator.SetBool("Move", true);
        }
        else
        {
            m_Animator.SetBool("Move", false);
        }
        if (m_RigidBody.velocity.y != 0 && m_OnLaddder)//if moving on ladder
        {
            m_Animator.SetBool("Ladder", true);
        }
        else if(!m_AtTopOfLadder && !m_AtBottomOfLadder) //if stationary on ladder - pause
        {
            m_Animator.enabled = false;
        }
        else //if not moving but not on a ladder
        {
            m_Animator.SetBool("Ladder", false);
        }
    }

    public void ExitStation()
    {
        m_InStation = false;
        DetermineStationExit(m_CurrentStation.name);
        m_ClosestStation.GetComponent<StationController>().m_PlayerControlled = false;
        m_ClosestStation.GetComponent<StationController>().AddNoPlayerLight(m_Player);
        m_CurrentStation = null;
    }

    private void EnterOrExitStation()
    {
        if (m_ClosestStation != null && Input.GetButtonDown(m_Action1))
        {
            if (m_InStation) // In -> Out
            {
                m_InStation = false;
                DetermineStationExit(m_CurrentStation.name);
                m_ClosestStation.GetComponent<StationController>().m_PlayerControlled = false;
                m_ClosestStation.GetComponent<StationController>().AddNoPlayerLight(m_Player);
                m_CurrentStation = null;                            
            }
            else // Out -> In
            {
                if (!m_ClosestStation.GetComponent<StationController>().m_PlayerControlled) //Can only enter if no one else is already there
                {
                    m_ClosestStation.GetComponent<StationController>().m_PlayerControlled = true;
                    m_ClosestStation.GetComponent<StationController>().RemoveAllPlayerLightsExceptControllers(m_Player);
                    m_RigidBody.velocity = Vector2.zero;
                    m_InStation = true;
                    m_CurrentStation = m_ClosestStation;
                    DetermineStationEnter(m_CurrentStation.name);
                }
            }
        }
    }

    private void MapPlayerControls()
    {
        m_Horizontal = m_Horizontal + m_Player;
        m_Vertical = m_Vertical + m_Player;
        m_Action1 = m_Action1 + m_Player;
        m_Action2 = m_Action2 + m_Player;
    }

    private void DetermineStationEnter(string _stationName)
    {
        m_CanMove = false;
        m_Animator.SetBool("Move", false);

        switch (_stationName)
        {           
            case SubmarineManager.PILOT_STATION:
                SubmarineManager.GetInstance().m_Submarine.SetControls(true, m_Horizontal, m_Vertical, m_Action2);
                break;
            case SubmarineManager.TOP_WEAPON_STATION:
                SubmarineManager.GetInstance().m_TopWeaponStation.SetControls(true, m_Horizontal, m_Action2, 
                    -90 / -SubmarineManager.GetInstance().m_TopWeaponStation.m_Speed, 90 / -SubmarineManager.GetInstance().m_TopWeaponStation.m_Speed);
                break;
            case SubmarineManager.BOTTOM_WEAPON_STATION:
                SubmarineManager.GetInstance().m_BottomWeaponStation.SetControls(true, m_Horizontal, m_Action2, 
                    90 / SubmarineManager.GetInstance().m_BottomWeaponStation.m_Speed, 270 / SubmarineManager.GetInstance().m_BottomWeaponStation.m_Speed);
                break;
            case SubmarineManager.SHIELD_STATION:
                SubmarineManager.GetInstance().m_Shield.SetControls(true, m_Horizontal);
                break;
            case SubmarineManager.MAP_STATION:
                SubmarineManager.GetInstance().m_MiniMap.SetControls(true, m_Horizontal, m_Vertical, m_Action2);
                break;
            case SubmarineManager.REPAIR_STATION:
                SubmarineManager.GetInstance().m_RepairStation.SetControls(true, m_Action2, gameObject);
                break;
            case SubmarineManager.ARMORY_STATION:
                SubmarineManager.GetInstance().m_ArmoryStation.SetControls(true, m_Action2, gameObject);
                break;
            default: 
                break;
        }
    }

    public void DetermineStationExit(string _stationName)
    {
        m_CanMove = true;
        switch (_stationName)
        {
            case SubmarineManager.PILOT_STATION:
                SubmarineManager.GetInstance().m_Submarine.SetControls(false, string.Empty, string.Empty, string.Empty);
                break;
            case SubmarineManager.TOP_WEAPON_STATION:
                SubmarineManager.GetInstance().m_TopWeaponStation.SetControls(false, string.Empty, string.Empty, 0, 0);
                break;
            case SubmarineManager.BOTTOM_WEAPON_STATION:
                SubmarineManager.GetInstance().m_BottomWeaponStation.SetControls(false, string.Empty, string.Empty, 0, 0);
                break;
            case SubmarineManager.SHIELD_STATION:
                SubmarineManager.GetInstance().m_Shield.SetControls(false, string.Empty);
                break;
            case SubmarineManager.MAP_STATION:
                SubmarineManager.GetInstance().m_MiniMap.SetControls(false, string.Empty, string.Empty, string.Empty);
                break;
            case SubmarineManager.REPAIR_STATION:
                SubmarineManager.GetInstance().m_RepairStation.SetControls(false, string.Empty, null);
                break;
            case SubmarineManager.ARMORY_STATION:
                SubmarineManager.GetInstance().m_ArmoryStation.SetControls(false, string.Empty, null);
                break;
            default:
                break;
        }
    }
   
    private void Reload()
    {
        m_HasAmmo = false;
        m_AmmoCrate.SetActive(false);
    }

    private void CheckIfAmmoStation()
    {
        //If an ammo station, and hit reload button, and have ammo
        if (Input.GetButtonUp(m_Action2) && m_HasAmmo)
        {
            if (m_ClosestStation.name == SubmarineManager.TOP_WEAPON_STATION)
            {
                if (SubmarineManager.GetInstance().m_TopWeaponStation.GetComponent<GunController>().m_AmmoCount + 10 > 100) 
                { SubmarineManager.GetInstance().m_TopWeaponStation.GetComponent<GunController>().m_AmmoCount = 100; }
                else { SubmarineManager.GetInstance().m_TopWeaponStation.GetComponent<GunController>().m_AmmoCount += 10; }
                Reload();
            }
            else if( m_ClosestStation.name == SubmarineManager.BOTTOM_WEAPON_STATION)
            {
                if (SubmarineManager.GetInstance().m_BottomWeaponStation.GetComponent<GunController>().m_AmmoCount + 10 > 100)
                { SubmarineManager.GetInstance().m_BottomWeaponStation.GetComponent<GunController>().m_AmmoCount = 100; }
                else { SubmarineManager.GetInstance().m_BottomWeaponStation.GetComponent<GunController>().m_AmmoCount += 10; }
                Reload();
            }
            else if (m_ClosestStation.name == SubmarineManager.PILOT_STATION)
            {
                if (SubmarineManager.GetInstance().m_Submarine.GetComponent<SubmarineController>().m_AmmoCount + 5 > 50)
                { SubmarineManager.GetInstance().m_Submarine.GetComponent<SubmarineController>().m_AmmoCount = 50; }
                else { SubmarineManager.GetInstance().m_Submarine.GetComponent<SubmarineController>().m_AmmoCount += 5; }
                Reload();
            }
        }
    }

    private void CheckIfCanRepair(GameObject _crack)
    {
        //If at a crack, hit action2, and has repair panel
        if (Input.GetButtonUp(m_Action2) && m_HasRepairPanel)
        {
            _crack.GetComponent<FollowCrack>().m_MyCrack.GetComponent<Crack>().Repair();
            m_HasRepairPanel = false;
            m_RepairPanel.SetActive(false);
        }
    }
}
