using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreIncrease : MonoBehaviour
{
    public Stopwatch stopwatchscript;
    public Rigidbody rb;
    private bool hitSomething = false;
    // Start is called before the first frame update
    void Start()
    {
        stopwatchscript = GameObject.FindGameObjectWithTag("Stopwatch").GetComponent<Stopwatch>();
    }

    // Update is called once per frame
    void Update()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "3pt")
        {
            hitSomething = true;
            scoreText.scoreAmount += 3;
            //Destroy(collision.gameObject);
            Stick();
            print("3");
           
        }
        if (collision.gameObject.tag == "2pt")
        {

            hitSomething = true;
            scoreText.scoreAmount += 2;
            //Destroy(collision.gameObject);
            Stick();
            print("2");

        }
        if (collision.gameObject.tag == "1pt")
        {
            stopwatchscript.timerStop = true;
            hitSomething = true;
            scoreText.scoreAmount += 1;
            //Destroy(collision.gameObject);
            Stick();
            print("1");
            

        }
        if (collision.gameObject.tag == "door")
        {

            hitSomething = true;
            scoreText.scoreAmount += 1;
            //Destroy(collision.gameObject);
            Stick();
            print("door");

        }
    }
    private void Stick()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
