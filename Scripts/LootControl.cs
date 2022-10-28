using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootControl : MonoBehaviour
{
    public List<GameObject> loot;
    
   //dicidir el loot cuando se rompe la caja

    public void OpenLoot()
    {
        //random loot
        int randomLoot = Random.Range(0, loot.Count);
        GameObject newLoot = Instantiate(loot[randomLoot], transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
