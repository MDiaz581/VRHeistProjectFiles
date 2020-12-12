using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    
    public GameObject Door;
    public GameObject HitSound;

    private void OnCollisionStay(Collision collision)
    {
        Door.GetComponent<MoveDoor>().SetOpen(true);
        
    }

    private void OnCollisionExit(Collision collision)
    {
        Door.GetComponent<MoveDoor>().SetOpen(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(HitSound, collision.gameObject.transform.position, transform.rotation);
    }
}
