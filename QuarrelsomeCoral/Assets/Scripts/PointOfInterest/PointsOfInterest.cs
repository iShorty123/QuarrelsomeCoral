using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsOfInterest : MonoBehaviour
{

    public GameObject[] PointsOfInterestTypes = new GameObject[1];

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject[] GetPointOfInterestTypes()
    {
        return PointsOfInterestTypes;
    }
}
