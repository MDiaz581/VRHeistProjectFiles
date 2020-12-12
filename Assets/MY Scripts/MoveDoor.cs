using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDoor : MonoBehaviour
{
    public float upSpeed = 0.1f;
    public float downSpeed = -0.5f;
    public bool Opening = false;
    public float doorHeight = 3f;
    Rigidbody RB;
    Vector3 startingPosition;

    void Start()
    {
        RB = GetComponent<Rigidbody>();
        startingPosition = transform.position;
    }
    public void SetOpen(bool newVal)
    {
        Opening = newVal;
    }

    void FixedUpdate()
    {
        Vector3 moveVector = Vector3.zero;

        if (Opening)
        {
            if (transform.position.y < doorHeight)
            {
                moveVector = new Vector3(0, upSpeed, 0);
                RB.MovePosition(transform.position + moveVector);
            } else
            {
                RB.MovePosition(startingPosition + new Vector3(0, doorHeight, 0));
            }
        } else {
            if (transform.position.y > 1f)
            {
                moveVector = new Vector3(0, downSpeed, 0);
                RB.MovePosition(transform.position + moveVector);
            } else
            {
                RB.MovePosition(startingPosition);
            }
        }
    }
}
