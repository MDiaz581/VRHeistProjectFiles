using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidAI : MonoBehaviour
{
    float currentFrictionValue = 0;
    float skidAt = 0.5f;
    float soundEmission = 5;
    float soundWait = 15;
    GameObject skidSound;
    float markWidth = 0.3f;
    int skidding = 0;

    Vector3[] lastPosition = new Vector3[2];
    public Material SkidMaterial;


    void Update()
    {
        PlayerSkidSound();
    }

    public void PlayerSkidSound()
    {
        WheelHit hit;
        WheelCollider collider;
        collider = GetComponent<WheelCollider>();

        collider.GetGroundHit(out hit);
        currentFrictionValue = Mathf.Abs(hit.sidewaysSlip);

        if(skidAt <= currentFrictionValue && soundWait <= 0)
        {
            skidSound = Instantiate(Resources.Load("SoundSlip"), hit.point, Quaternion.identity) as GameObject;
            soundWait = 1;
        }

        if(skidAt <= currentFrictionValue)
        {
            SkidMesh();
        }
        else
        {
            skidding = 0;
        }

        soundWait -= Time.deltaTime * soundEmission;
    }

    void SkidMesh()
    {
        WheelHit hit;
        GetComponent<WheelCollider>().GetGroundHit(out hit);

        GameObject marks = new GameObject("Marks");

        marks.AddComponent<MeshFilter>();
        marks.AddComponent<MeshRenderer>();

        // Marks Generate
        Mesh markMesh = new Mesh();

        // Vertices for Cube
        Vector3[] vertices = new Vector3[4];
        int[] triangle = new int[6] { 0, 1, 2, 2, 3, 0 };

        // These Condition Generate The Vertices
        if (skidding == 0)
        {
            vertices[0] = hit.point + Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z) * new Vector3(markWidth, 0.1f, 0f);
            vertices[1] = hit.point + Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z) * new Vector3(-markWidth, 0.1f, 0f);
            vertices[2] = hit.point + Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z) * new Vector3(-markWidth, 0.1f, 0f);
            vertices[3] = hit.point + Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z) * new Vector3(markWidth, 0.1f, 0f);

            lastPosition[0] = vertices[2];
            lastPosition[1] = vertices[3];
            skidding = 1;
        }
        else
        {
            vertices[0] = lastPosition[1];
            vertices[1] = lastPosition[0];

            vertices[2] = hit.point + Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z) * new Vector3(-markWidth, 0.1f, 0f);
            vertices[3] = hit.point + Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z) * new Vector3(markWidth, 0.1f, 0f);

            lastPosition[0] = vertices[2];
            lastPosition[1] = vertices[3];
        }

        // Store all vertices of cube
        markMesh.vertices = vertices;

        // Store verties of triangle
        markMesh.triangles = triangle;

        // Remove corners of triangle and create smoothness
        markMesh.RecalculateNormals();

        // Generate OutPut
        marks.GetComponent<MeshFilter>().mesh = markMesh;
        marks.GetComponent<MeshRenderer>().material = SkidMaterial;

        // Destroy Marks
        marks.AddComponent<destroySkid>();
    }

}
