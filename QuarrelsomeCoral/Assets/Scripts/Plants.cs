﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plants : MonoBehaviour
{

    public GameObject[] PlantTypes = new GameObject[8];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject[] GetPlantTypes() {
        return PlantTypes;
    }
}
