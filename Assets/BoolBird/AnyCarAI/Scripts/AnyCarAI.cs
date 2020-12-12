using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEditor;

// extraWheels
public enum AxlAI
{
    Front,
    Rear
}

[Serializable]
public struct CarWheelsAI
{
    public GameObject model;
    public AxlAI axel;
}

[Serializable]
public struct CarWheelsAICols
{
    public GameObject collider;
    public AxlAI axel;
}

public class AnyCarAI : MonoBehaviour
{
    // Set Up

    // Wheels
    public List<CarWheelsAI> extraWheels;
    public List<CarWheelsAICols> extraWheelsColList = new List<CarWheelsAICols>();
    public CarWheelsAICols extraWheelCol;
    public GameObject frontLeft;
    public GameObject frontRight;
    public GameObject backLeft;
    public GameObject backRight;

    public GameObject frontLeftCol;
    public GameObject frontRightCol;
    public GameObject backLeftCol;
    public GameObject backRightCol;

    public bool skidMarks = true;
    public Material SkidMaterial;

    // Body
    public GameObject bodyMesh;
    public GameObject extraBodyCol;

    // Wheels Tab
    public float dumpingRate = 0.016f;
    public float suspensionDistance = 0;
    public Vector3 wheelsPosition;
    public float wheelStiffness = 1f;
    public float suspensionSpring = 100000f;
    public float suspensionDumper = 7000f;

    // Features
    public float vehicleMass = 500f;
    public float maxSteerAngle = 45.0f;
    public float maxSpeed = 500f;
    public float brakeForce = 1500f;
    public bool ABS = false;
    public bool ABSCor = false;

    // Advanced
    public Vector3 CenterOfMass;
    public Vector3 wheelsRotation;
    public float[] gearRatios;

    // Initialized on Start
    public GameObject FrontLights;
    public GameObject backLights;

    // Mechanic
    public float extraWheelRadius;
    private float FLRadius;
    private float FRRadius;
    private float BLRadius;
    private float BRRadius;
    private float curRPM;
    private int CurGear;
    private int maxGear;

    private Rigidbody rb
    {
        get;
        set;
    }

    // Refine
    private AudioSource Audio;
    public AudioClip engineSound;

    // Editor References
    public int toolbarTab;
    public string currentTab;

    public Transform AITarget;
    public Transform persuitTarget;
    private bool isBraking = false;
    public bool retroGear = false;
    private bool avoiding = false;
    public bool frontLightsOn = false;
    public bool persuitAI = false;
    public bool persuitON = false;

    private List<Transform> nodes;
    private int currentNode = 0;
    public float waypointDistance = 2f;

    public GameObject sensorObj;
    public GameObject rightSensorObj;
    public GameObject leftSensorObj;
    public float sensorLenght = 10f;
    public float brakeDistance = 15f;
    public float reverseDistance = 8f;
    public float persuitDistance = 20f;
    public float sensorAngle = 30f;
    private float targetSteerAngle = 0;
    public float turnSpeed = 7f;

    public float acceleration = 200f;
    private float speedMultiplier = 10f;
    private float currentSpeed;

    // Collision System
    public bool collisionSystem = false;
    public AudioClip collisionSound;
    public MeshFilter[] optionalMeshList;
    public int demolutionStrenght = 1;

  
    public void unpackPrefab()
    {
        Transform _parent = this.transform.parent;
        GameObject objToUnpack = _parent.gameObject;
        //PrefabUtility.UnpackPrefabInstance(objToUnpack, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
    }
    

    public void CreateColliders()
    {
        unpackPrefab();

        // Create and Set Wheel Colliders
        frontLeft.AddComponent(typeof(SphereCollider));
        frontRight.AddComponent(typeof(SphereCollider));
        backLeft.AddComponent(typeof(SphereCollider));
        backRight.AddComponent(typeof(SphereCollider));

        // Create Wheel Colliders Objects
        frontLeftCol = new GameObject("FLCOL");
        frontRightCol = new GameObject("FRCOL");
        backLeftCol = new GameObject("BLCOL");
        backRightCol = new GameObject("BRCOL");

        // Front Left Wheel
        frontLeftCol.transform.parent = this.transform;
        frontLeftCol.transform.position = frontLeft.transform.position;
        frontLeftCol.transform.rotation = frontLeft.transform.rotation;

        // Front Right Wheel
        frontRightCol.transform.parent = this.transform;
        frontRightCol.transform.position = frontRight.transform.position;
        frontRightCol.transform.rotation = frontRight.transform.rotation;

        // Back Left Wheel
        backLeftCol.transform.parent = this.transform;
        backLeftCol.transform.position = backLeft.transform.position;
        backLeftCol.transform.rotation = backLeft.transform.rotation;

        // Back Right Wheel
        backRightCol.transform.parent = this.transform;
        backRightCol.transform.position = backRight.transform.position;
        backRightCol.transform.rotation = backRight.transform.rotation;

        // Add Wheel Colliders
        frontLeftCol.AddComponent(typeof(WheelCollider));
        frontRightCol.AddComponent(typeof(WheelCollider));
        backLeftCol.AddComponent(typeof(WheelCollider));
        backRightCol.AddComponent(typeof(WheelCollider));

        // Add Skid Marks
        frontLeftCol.AddComponent(typeof(SkidAI));
        frontRightCol.AddComponent(typeof(SkidAI));
        backLeftCol.AddComponent(typeof(SkidAI));
        backRightCol.AddComponent(typeof(SkidAI));

        frontLeftCol.GetComponent<SkidAI>().SkidMaterial = SkidMaterial;
        frontRightCol.GetComponent<SkidAI>().SkidMaterial = SkidMaterial;
        backLeftCol.GetComponent<SkidAI>().SkidMaterial = SkidMaterial;
        backRightCol.GetComponent<SkidAI>().SkidMaterial = SkidMaterial;

        // Get Wheel Radius
        FLRadius = frontLeft.GetComponent<SphereCollider>().radius * frontLeft.transform.lossyScale.x;
        FRRadius = frontRight.GetComponent<SphereCollider>().radius * frontLeft.transform.lossyScale.x;
        BLRadius = backLeft.GetComponent<SphereCollider>().radius * frontLeft.transform.lossyScale.x;
        BRRadius = backRight.GetComponent<SphereCollider>().radius * frontLeft.transform.lossyScale.x;

        // Set Wheel Radius
        frontLeftCol.GetComponent<WheelCollider>().radius = FLRadius;
        frontRightCol.GetComponent<WheelCollider>().radius = FRRadius;
        backLeftCol.GetComponent<WheelCollider>().radius = BLRadius;
        backRightCol.GetComponent<WheelCollider>().radius = BRRadius;

        // Destroy Sphere Colliders
        DestroyImmediate(frontLeft.GetComponent<SphereCollider>());
        DestroyImmediate(frontRight.GetComponent<SphereCollider>());
        DestroyImmediate(backLeft.GetComponent<SphereCollider>());
        DestroyImmediate(backRight.GetComponent<SphereCollider>());

        // Create Body Convex Mesh Collider
        if (bodyMesh != null)
        {
            bodyMesh.AddComponent(typeof(MeshCollider));
            bodyMesh.GetComponent<MeshCollider>().convex = true;
        }


        // Extra Wheels
        extraWheelsColList.Clear();
        int c = 1;
        foreach (var wheel in extraWheels)
        {
            wheel.model.AddComponent(typeof(SphereCollider));

            extraWheelCol.collider = new GameObject("extraWheel " + c);
            extraWheelCol.axel = wheel.axel;

            extraWheelCol.collider.transform.parent = this.transform;
            extraWheelCol.collider.transform.position = wheel.model.transform.position;
            extraWheelCol.collider.transform.rotation = wheel.model.transform.rotation;

            extraWheelCol.collider.AddComponent(typeof(WheelCollider));
            extraWheelCol.collider.AddComponent(typeof(SkidAI));
            extraWheelCol.collider.GetComponent<SkidAI>().SkidMaterial = SkidMaterial;

            extraWheelRadius = wheel.model.GetComponent<SphereCollider>().radius * wheel.model.transform.lossyScale.x;
            extraWheelCol.collider.GetComponent<WheelCollider>().radius = extraWheelRadius;

            DestroyImmediate(wheel.model.GetComponent<SphereCollider>());

            extraWheelsColList.Add(extraWheelCol);

            c++;
        }
    }

    public void CreateDebugBodyCol()
    {
        extraBodyCol = Instantiate(Resources.Load("BodyCollider"), this.transform.position, Quaternion.identity) as GameObject;
        extraBodyCol.transform.parent = this.transform;
        extraBodyCol.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - .6f, this.transform.position.z);
        extraBodyCol.transform.rotation = this.transform.rotation;
        bodyMesh = extraBodyCol;
    }

    private void Start()
    {
        // Get References
        GetReferences();

        // Set Skid Marks
        SetSkidMarks();

        // Set Wheels Values
        SetWheelsValues();

        SetPath();
    }

    private void Update()
    {
        // RPM Creation
        curRPM = (this.rb.velocity.magnitude * 60 / (3f * 2 * Mathf.PI)) * 2 * gearRatios[CurGear];

        // Engine Audio
        EngineAudio();

        // Motor
        AnimateWheels();

        // Transmission
        TrasmissionMode();
    }

    private void FixedUpdate()
    {
        Sensors();
        BrakeSensors();
        RetroSensors();
        PersuitSensors();
        ApplySteer();
        Drive();
        CheckWaypointDistance();
        LerpToSteerAngle();
        MainBrake();
    }

    private void GetReferences()
    {
        rb = GetComponent<Rigidbody>();
        Audio = GetComponent<AudioSource>();
        Audio.clip = engineSound;
        Audio.Play();
        rb.centerOfMass = CenterOfMass;
        rb.mass = vehicleMass;

        FrontLights = this.gameObject.transform.GetChild(0).transform.GetChild(2).gameObject;
        backLights = this.gameObject.transform.GetChild(0).transform.GetChild(1).gameObject;

        if (frontLightsOn)
        {
            FrontLights.SetActive(true);
        }
        else
        {
            FrontLights.SetActive(false);
        }

        backLights.SetActive(false);

        sensorObj = this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
        leftSensorObj = this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).gameObject;
        rightSensorObj = this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject;
    }

    private void Sensors()
    {
        RaycastHit hit;
        float avoidMultiplier = 0;
        avoiding = false;

        // Right Sensor
        if (Physics.Raycast(rightSensorObj.transform.position, sensorObj.transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(rightSensorObj.transform.position, hit.point, Color.red);
                avoiding = true;
                avoidMultiplier -= 1f;
            }
        }

        // Right Angle Sensor
        else if (Physics.Raycast(rightSensorObj.transform.position, Quaternion.AngleAxis(sensorAngle, rightSensorObj.transform.up) * rightSensorObj.transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(rightSensorObj.transform.position, hit.point, Color.yellow);
                avoiding = true;
                avoidMultiplier -= 0.5f;
            }
        }

        // Left Sensor
        if (Physics.Raycast(leftSensorObj.transform.position, sensorObj.transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(leftSensorObj.transform.position, hit.point, Color.green);
                avoiding = true;
                avoidMultiplier += 1f;
            }
        }


        // Left Angle Sensor
        else if (Physics.Raycast(leftSensorObj.transform.position, Quaternion.AngleAxis(-sensorAngle, leftSensorObj.transform.up) * leftSensorObj.transform.forward, out hit, sensorLenght))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(leftSensorObj.transform.position, hit.point, Color.yellow);
                avoiding = true;
                avoidMultiplier += 0.5f;
            }
        }

        if (avoidMultiplier == 0)
        {
            //front center sensor
            if (Physics.Raycast(sensorObj.transform.position, sensorObj.transform.forward, out hit, sensorLenght))
            {
                if (!hit.collider.CompareTag("Terrain"))
                {
                    Debug.DrawLine(sensorObj.transform.position, hit.point, Color.blue);
                    avoiding = true;
                    if (hit.normal.x < 0)
                    {
                        avoidMultiplier = -1f;
                    }
                    else
                    {
                        avoidMultiplier = 1f;
                    }
                }
            }
        }

        if (avoiding)
        {
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }
    }

    private void BrakeSensors()
    {
        RaycastHit hit;
        isBraking = false;

        // Right Sensor
        if (Physics.Raycast(rightSensorObj.transform.position, sensorObj.transform.forward, out hit, brakeDistance))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(rightSensorObj.transform.position, hit.point, Color.magenta);
                isBraking = true;
            }
        }

        // Right Angle Sensor
        else if (Physics.Raycast(rightSensorObj.transform.position, Quaternion.AngleAxis(sensorAngle, rightSensorObj.transform.up) * rightSensorObj.transform.forward, out hit, brakeDistance))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(rightSensorObj.transform.position, hit.point, Color.magenta);
                isBraking = true;
            }
        }

        // Left Sensor
        if (Physics.Raycast(leftSensorObj.transform.position, sensorObj.transform.forward, out hit, brakeDistance))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(leftSensorObj.transform.position, hit.point, Color.magenta);
                isBraking = true;
            }
        }


        // Left Angle Sensor
        else if (Physics.Raycast(leftSensorObj.transform.position, Quaternion.AngleAxis(-sensorAngle, leftSensorObj.transform.up) * leftSensorObj.transform.forward, out hit, brakeDistance))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(leftSensorObj.transform.position, hit.point, Color.magenta);
                isBraking = true;
            }
        }
    }

    private void RetroSensors()
    {
        RaycastHit hit;
        retroGear = false;

        // Right Sensor
        if (Physics.Raycast(rightSensorObj.transform.position, sensorObj.transform.forward, out hit, reverseDistance))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                retroGear = true;
            }
        }

        // Right Angle Sensor
        else if (Physics.Raycast(rightSensorObj.transform.position, Quaternion.AngleAxis(sensorAngle, rightSensorObj.transform.up) * rightSensorObj.transform.forward, out hit, reverseDistance))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                retroGear = true;
            }
        }

        // Left Sensor
        if (Physics.Raycast(leftSensorObj.transform.position, sensorObj.transform.forward, out hit, reverseDistance))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                retroGear = true;
            }
        }


        // Left Angle Sensor
        else if (Physics.Raycast(leftSensorObj.transform.position, Quaternion.AngleAxis(-sensorAngle, leftSensorObj.transform.up) * leftSensorObj.transform.forward, out hit, reverseDistance))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                retroGear = true;
            }
        }
    }

    private void PersuitSensors()
    {
        RaycastHit hit;

        // Right Sensor
        if (Physics.Raycast(rightSensorObj.transform.position, sensorObj.transform.forward, out hit, persuitDistance))
        {
            if (!hit.collider.CompareTag("Terrain") && hit.collider.CompareTag("PersuitTarget"))
            {
                persuitON = true;
            }
        }

        // Right Angle Sensor
        if (Physics.Raycast(rightSensorObj.transform.position, Quaternion.AngleAxis(sensorAngle, rightSensorObj.transform.up) * rightSensorObj.transform.forward, out hit, persuitDistance))
        {
            if (!hit.collider.CompareTag("Terrain") && hit.collider.CompareTag("PersuitTarget"))
            {
                persuitON = true;
            }
        }

        // Left Sensor
        if (Physics.Raycast(leftSensorObj.transform.position, sensorObj.transform.forward, out hit, persuitDistance))
        {
            if (!hit.collider.CompareTag("Terrain") && hit.collider.CompareTag("PersuitTarget"))
            {
                persuitON = true;
            }
        }


        // Left Angle Sensor
        if (Physics.Raycast(leftSensorObj.transform.position, Quaternion.AngleAxis(-sensorAngle, leftSensorObj.transform.up) * leftSensorObj.transform.forward, out hit, persuitDistance))
        {
            if (!hit.collider.CompareTag("Terrain") && hit.collider.CompareTag("PersuitTarget"))
            {
                persuitON = true;
            }
        }

        if (Physics.Raycast(sensorObj.transform.position, sensorObj.transform.forward, out hit, persuitDistance))
        {
            if (!hit.collider.CompareTag("Terrain") && hit.collider.CompareTag("PersuitTarget"))
            {
                persuitON = true;
            }
        }
    }

    private void ApplySteer()
    {
        if (avoiding) return;

        if (persuitAI)
        {
            if (persuitON)
            {
                Vector3 relativeVector = transform.InverseTransformPoint(persuitTarget.position);
                float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
                targetSteerAngle = newSteer;
            }
            else
            {
                Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
                float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
                targetSteerAngle = newSteer;
            }
        }
        else
        {
            Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
            float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
            targetSteerAngle = newSteer;
        }
        
    }

    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * frontLeftCol.GetComponent<WheelCollider>().radius * frontLeftCol.GetComponent<WheelCollider>().rpm * 60 / 1000;

        if (!retroGear)
        {
            if (currentSpeed < maxSpeed && !isBraking)
            {
                frontLeftCol.GetComponent<WheelCollider>().motorTorque = acceleration * speedMultiplier / gearRatios[CurGear];
                frontRightCol.GetComponent<WheelCollider>().motorTorque = acceleration * speedMultiplier / gearRatios[CurGear];
                backLeftCol.GetComponent<WheelCollider>().motorTorque = acceleration * speedMultiplier / gearRatios[CurGear];
                backRightCol.GetComponent<WheelCollider>().motorTorque = acceleration * speedMultiplier / gearRatios[CurGear];
            }
            else
            {
                frontLeftCol.GetComponent<WheelCollider>().motorTorque = 0;
                frontRightCol.GetComponent<WheelCollider>().motorTorque = 0;
                backLeftCol.GetComponent<WheelCollider>().motorTorque = 0;
                backRightCol.GetComponent<WheelCollider>().motorTorque = 0;
            }
        }
        else
        {
            frontLeftCol.GetComponent<WheelCollider>().motorTorque = -acceleration * speedMultiplier / gearRatios[CurGear];
            frontRightCol.GetComponent<WheelCollider>().motorTorque = -acceleration * speedMultiplier / gearRatios[CurGear];
            backLeftCol.GetComponent<WheelCollider>().motorTorque = -acceleration * speedMultiplier / gearRatios[CurGear];
            backRightCol.GetComponent<WheelCollider>().motorTorque = -acceleration * speedMultiplier / gearRatios[CurGear];
        }
    }

    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < waypointDistance)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }

    private void LerpToSteerAngle()
    {
        frontLeftCol.GetComponent<WheelCollider>().steerAngle = Mathf.Lerp(frontLeftCol.GetComponent<WheelCollider>().steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        frontRightCol.GetComponent<WheelCollider>().steerAngle = Mathf.Lerp(frontRightCol.GetComponent<WheelCollider>().steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);

        foreach (var wheelCol in extraWheelsColList)
        {
            if (wheelCol.axel == AxlAI.Front)
            {
                wheelCol.collider.GetComponent<WheelCollider>().steerAngle = Mathf.Lerp(wheelCol.collider.GetComponent<WheelCollider>().steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
            }
        }
    }

    private void AnimateWheels()
    {
        Quaternion FLRot;
        Vector3 FLPos;
        Quaternion FRRot;
        Vector3 FRPos;
        Quaternion BLRot;
        Vector3 BLPos;
        Quaternion BRRot;
        Vector3 BRPos;

        // Front Left
        frontLeftCol.GetComponent<WheelCollider>().GetWorldPose(out FLPos, out FLRot);
        FLRot = FLRot * Quaternion.Euler(wheelsRotation);
        frontLeft.transform.position = FLPos;
        frontLeft.transform.rotation = FLRot;

        // Front Right
        frontRightCol.GetComponent<WheelCollider>().GetWorldPose(out FRPos, out FRRot);
        FRRot = FRRot * Quaternion.Euler(wheelsRotation);
        frontRight.transform.position = FRPos;
        frontRight.transform.rotation = FRRot;

        // Back Left
        backLeftCol.GetComponent<WheelCollider>().GetWorldPose(out BLPos, out BLRot);
        BLRot = BLRot * Quaternion.Euler(wheelsRotation);
        backLeft.transform.position = BLPos;
        backLeft.transform.rotation = BLRot;

        // Back Right
        backRightCol.GetComponent<WheelCollider>().GetWorldPose(out BRPos, out BRRot);
        BRRot = BRRot * Quaternion.Euler(wheelsRotation);
        backRight.transform.position = BRPos;
        backRight.transform.rotation = BRRot;

        // Extra Wheels
        int i = 0;
        foreach (var wheelCol in extraWheelsColList)
        {
            Quaternion _rot;
            Vector3 _pos;
            wheelCol.collider.GetComponent<WheelCollider>().GetWorldPose(out _pos, out _rot);
            _rot = _rot * Quaternion.Euler(wheelsRotation);
            extraWheels[i].model.transform.position = _pos;
            extraWheels[i].model.transform.rotation = _rot;

            i++;
        }

    }

    private void MainBrake()
    {
        if (!ABS)
        {
            if (isBraking == true && !retroGear)
            {
                frontLeftCol.GetComponent<WheelCollider>().brakeTorque = brakeForce;
                frontRightCol.GetComponent<WheelCollider>().brakeTorque = brakeForce;
                backLeftCol.GetComponent<WheelCollider>().brakeTorque = brakeForce;
                backRightCol.GetComponent<WheelCollider>().brakeTorque = brakeForce;
                backLights.SetActive(true);
            }
            else
            {
                frontLeftCol.GetComponent<WheelCollider>().brakeTorque = 0;
                frontRightCol.GetComponent<WheelCollider>().brakeTorque = 0;
                backLeftCol.GetComponent<WheelCollider>().brakeTorque = 0;
                backRightCol.GetComponent<WheelCollider>().brakeTorque = 0;
                backLights.SetActive(false);
            }

            foreach (var wheelCol in extraWheelsColList)
            {
                if (isBraking == true && !retroGear)
                {
                    wheelCol.collider.GetComponent<WheelCollider>().brakeTorque = brakeForce;
                    backLights.SetActive(true);
                }
                else
                {
                    wheelCol.collider.GetComponent<WheelCollider>().brakeTorque = 0;
                    backLights.SetActive(false);
                }
            }
        }
        else
        {
            if (isBraking == true && !retroGear)
            {
                StartCoroutine(ABSCoroutine());
                backLights.SetActive(true);
            }
            else
            {
                frontLeftCol.GetComponent<WheelCollider>().brakeTorque = 0;
                frontRightCol.GetComponent<WheelCollider>().brakeTorque = 0;
                backLeftCol.GetComponent<WheelCollider>().brakeTorque = 0;
                backRightCol.GetComponent<WheelCollider>().brakeTorque = 0;
                backLights.SetActive(false);
            }

            foreach (var wheelCol in extraWheelsColList)
            {
                if (isBraking == true && !retroGear)
                {
                    wheelCol.collider.GetComponent<WheelCollider>().brakeTorque = brakeForce;
                    backLights.SetActive(true);
                }
                else
                {
                    wheelCol.collider.GetComponent<WheelCollider>().brakeTorque = 0;
                    backLights.SetActive(false);
                }
            }
        }
    }

    private void SetWheelsValues()
    {
        frontLeftCol.GetComponent<WheelCollider>().wheelDampingRate = dumpingRate;
        frontRightCol.GetComponent<WheelCollider>().wheelDampingRate = dumpingRate;
        backLeftCol.GetComponent<WheelCollider>().wheelDampingRate = dumpingRate;
        backRightCol.GetComponent<WheelCollider>().wheelDampingRate = dumpingRate;

        frontLeftCol.GetComponent<WheelCollider>().suspensionDistance = suspensionDistance;
        frontRightCol.GetComponent<WheelCollider>().suspensionDistance = suspensionDistance;
        backLeftCol.GetComponent<WheelCollider>().suspensionDistance = suspensionDistance;
        backRightCol.GetComponent<WheelCollider>().suspensionDistance = suspensionDistance;

        frontLeftCol.GetComponent<WheelCollider>().center = wheelsPosition;
        frontRightCol.GetComponent<WheelCollider>().center = wheelsPosition;
        backLeftCol.GetComponent<WheelCollider>().center = wheelsPosition;
        backRightCol.GetComponent<WheelCollider>().center = wheelsPosition;

        var leftCol = frontLeftCol.GetComponent<WheelCollider>().suspensionSpring;
        var rightCol = frontRightCol.GetComponent<WheelCollider>().suspensionSpring;
        var bLeft = backLeftCol.GetComponent<WheelCollider>().suspensionSpring;
        var bRight = backRightCol.GetComponent<WheelCollider>().suspensionSpring;
        leftCol.spring = suspensionDumper;
        rightCol.spring = suspensionDumper;
        bLeft.spring = suspensionDumper;
        bRight.spring = suspensionDumper;
        leftCol.damper = suspensionDumper;
        rightCol.damper = suspensionDumper;
        bLeft.damper = suspensionDumper;
        bRight.damper = suspensionDumper;
        frontLeftCol.GetComponent<WheelCollider>().suspensionSpring = leftCol;
        frontRightCol.GetComponent<WheelCollider>().suspensionSpring = rightCol;
        backLeftCol.GetComponent<WheelCollider>().suspensionSpring = bLeft;
        backRightCol.GetComponent<WheelCollider>().suspensionSpring = bRight;

        var fl = frontLeftCol.GetComponent<WheelCollider>().sidewaysFriction;
        var fr = frontRightCol.GetComponent<WheelCollider>().sidewaysFriction;
        var bl = backLeftCol.GetComponent<WheelCollider>().sidewaysFriction;
        var br = backRightCol.GetComponent<WheelCollider>().sidewaysFriction;
        fl.stiffness = wheelStiffness;
        fr.stiffness = wheelStiffness;
        bl.stiffness = wheelStiffness;
        br.stiffness = wheelStiffness;
        frontLeftCol.GetComponent<WheelCollider>().sidewaysFriction = fl;
        frontRightCol.GetComponent<WheelCollider>().sidewaysFriction = fr;
        backLeftCol.GetComponent<WheelCollider>().sidewaysFriction = bl;
        backRightCol.GetComponent<WheelCollider>().sidewaysFriction = br;

        var flf = frontLeftCol.GetComponent<WheelCollider>().forwardFriction;
        var frf = frontRightCol.GetComponent<WheelCollider>().forwardFriction;
        var blf = backLeftCol.GetComponent<WheelCollider>().forwardFriction;
        var brf = backRightCol.GetComponent<WheelCollider>().forwardFriction;
        flf.stiffness = wheelStiffness;
        frf.stiffness = wheelStiffness;
        blf.stiffness = wheelStiffness;
        brf.stiffness = wheelStiffness;
        frontLeftCol.GetComponent<WheelCollider>().forwardFriction = flf;
        frontRightCol.GetComponent<WheelCollider>().forwardFriction = frf;
        backLeftCol.GetComponent<WheelCollider>().forwardFriction = blf;
        backRightCol.GetComponent<WheelCollider>().forwardFriction = brf;

        foreach (var wheelCol in extraWheelsColList)
        {
            wheelCol.collider.GetComponent<WheelCollider>().wheelDampingRate = dumpingRate;
            wheelCol.collider.GetComponent<WheelCollider>().suspensionDistance = suspensionDistance;
            wheelCol.collider.GetComponent<WheelCollider>().center = wheelsPosition;

            var susp = wheelCol.collider.GetComponent<WheelCollider>().suspensionSpring;
            susp.spring = suspensionSpring;
            wheelCol.collider.GetComponent<WheelCollider>().suspensionSpring = susp;

            var stifn = wheelCol.collider.GetComponent<WheelCollider>().sidewaysFriction;
            stifn.stiffness = wheelStiffness;
            wheelCol.collider.GetComponent<WheelCollider>().sidewaysFriction = stifn;
        }
    }

    private void EngineAudio()
    {
        Audio.pitch = Mathf.Abs(curRPM / 100) + 1;
        Audio.volume = Mathf.Abs(curRPM / 1000) + .2f;

        if (Audio.pitch > 120)
            Audio.pitch = 120;
    }

    private void TrasmissionMode()
    {
        if (CurGear == 0)
        {
            if (curRPM > 290)
            {
                CurGear++;
            }
        }
        else if (CurGear == (gearRatios.Length - 1))
        {
            if (curRPM < 150)
                CurGear--;
        }
        else if (CurGear < (gearRatios.Length - 1) && CurGear > 0)
        {
            if (curRPM < 150)
            {
                CurGear--;
            }
            else if (curRPM > 280)
            {
                CurGear++;
            }
        }
    }

    IEnumerator ABSCoroutine()
    {
        frontLeftCol.GetComponent<WheelCollider>().brakeTorque = brakeForce;
        frontRightCol.GetComponent<WheelCollider>().brakeTorque = brakeForce;
        backLeftCol.GetComponent<WheelCollider>().brakeTorque = brakeForce;
        backRightCol.GetComponent<WheelCollider>().brakeTorque = brakeForce;
        yield return new WaitForSeconds(0.1f);
        frontLeftCol.GetComponent<WheelCollider>().brakeTorque = 0;
        frontRightCol.GetComponent<WheelCollider>().brakeTorque = 0;
        backLeftCol.GetComponent<WheelCollider>().brakeTorque = 0;
        backRightCol.GetComponent<WheelCollider>().brakeTorque = 0;
        yield return new WaitForSeconds(0.1f);
    }

    private void SetPath()
    {
        Transform[] pathTransforms = AITarget.GetComponentsInChildren<Transform>();

        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != AITarget.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
    }

    private void SetSkidMarks()
    {
        if (skidMarks == true)
        {
            frontLeftCol.GetComponent<SkidAI>().enabled = true;
            frontRightCol.GetComponent<SkidAI>().enabled = true;
            backLeftCol.GetComponent<SkidAI>().enabled = true;
            backRightCol.GetComponent<SkidAI>().enabled = true;

            frontLeftCol.GetComponent<SkidAI>().SkidMaterial = SkidMaterial;
            frontRightCol.GetComponent<SkidAI>().SkidMaterial = SkidMaterial;
            backLeftCol.GetComponent<SkidAI>().SkidMaterial = SkidMaterial;
            backRightCol.GetComponent<SkidAI>().SkidMaterial = SkidMaterial;

            foreach (var wheelCol in extraWheelsColList)
            {
                wheelCol.collider.GetComponent<SkidAI>().enabled = true;
                wheelCol.collider.GetComponent<SkidAI>().SkidMaterial = SkidMaterial;
            }
        }
        else
        {
            frontLeftCol.GetComponent<SkidAI>().enabled = false;
            frontRightCol.GetComponent<SkidAI>().enabled = false;
            backLeftCol.GetComponent<SkidAI>().enabled = false;
            backRightCol.GetComponent<SkidAI>().enabled = false;

            foreach (var wheelCol in extraWheelsColList)
            {
                wheelCol.collider.GetComponent<SkidAI>().enabled = false;
            }
        }
    }
}