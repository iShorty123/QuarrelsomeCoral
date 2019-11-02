using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCharacterMovementScene()
    {
        SceneManager.LoadScene("CharacterMovement");
    }

    public void StartSubmarineMovementScene()
    {
        SceneManager.LoadScene("SubmarineMovement");
    }

    public void StartMultipleInputScene()
    {
        SceneManager.LoadScene("MultipleInput");
    }

    public void StartEnterExitStationScene()
    {
        SceneManager.LoadScene("EnterExitStation");
    }

    public void StartShieldStationScene()
    {
        SceneManager.LoadScene("ShieldStation");
    }

    public void StartWeaponStationScene()
    {
        SceneManager.LoadScene("WeaponStation");
    }

    public void StartRandomWorldGenerationScene()
    {
        SceneManager.LoadScene("RandomWorldGeneration");
    }
    
    public void BackToSceneSelector()
    {
        SceneManager.LoadScene("PrototypeManager");
    }
}
