using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
	public AutomaticEnemy AEnemy = null;
    public AutomaticCave ACave = null;
    public AutomaticPlant APlant = null;

    // Start is called the first frame update
    void Start()
    {
        ACave.ConstructCave();
        APlant.AddPlants();
        AEnemy.StartEnemySpawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
