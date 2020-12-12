using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroySkid : MonoBehaviour
{
    float destroyAfter = 2f;
    float Timer = 0;

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (destroyAfter < Timer)
        {
            Destroy(gameObject);
        }
    }
}
