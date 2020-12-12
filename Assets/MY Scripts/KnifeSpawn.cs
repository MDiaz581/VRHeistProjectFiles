using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeSpawn : MonoBehaviour
{
    public GameObject Knife;
    public Transform SpawnPosition;



    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(Knife, SpawnPosition.position, SpawnPosition.rotation);

    }

}
