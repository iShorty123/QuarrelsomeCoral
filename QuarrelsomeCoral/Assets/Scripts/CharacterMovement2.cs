using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement2 : MonoBehaviour
{
    public float speed = 0;

    // Start is called before the first frame update
    void Start()
    {
        Animator charAnim = GetComponent<Animator>();
        charAnim.speed = speed/10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
