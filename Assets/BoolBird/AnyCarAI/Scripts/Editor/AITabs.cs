using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnyCarAI))]
public class AITabs : Editor
{
    private AnyCarAI myTarget;
    private SerializedObject soTarget;

    private SerializedProperty acceleration;
    private SerializedProperty dumpingRate;
    private SerializedProperty suspensionDistance;
    private SerializedProperty wheelsPosition;
    private SerializedProperty wheelStiffness;
    private SerializedProperty suspensionSpring;
    private SerializedProperty suspensionDumper;
    private SerializedProperty maxSpeed;
    private SerializedProperty engineSound;
    private SerializedProperty extraWheels;
    private SerializedProperty bodyMesh;
    private SerializedProperty maxSteerAngle;
    private SerializedProperty CenterOfMass;
    private SerializedProperty wheelsRotation;
    private SerializedProperty gearRatios;
    private SerializedProperty AITarget;
    private SerializedProperty frontLeft;
    private SerializedProperty frontRight;
    private SerializedProperty backLeft;
    private SerializedProperty backRight;
    private SerializedProperty skidMarks;
    private SerializedProperty vehicleMass;
    private SerializedProperty SkidMaterial;
    private SerializedProperty sensorAngle;
    private SerializedProperty sensorLenght;
    private SerializedProperty turnSpeed;
    private SerializedProperty waypointDistance;
    private SerializedProperty brakeForce;
    private SerializedProperty ABS;
    private SerializedProperty brakeDistance;
    private SerializedProperty reverseDistance;
    private SerializedProperty frontLightsOn;
    private SerializedProperty persuitAI;
    private SerializedProperty persuitTarget;
    private SerializedProperty persuitDistance;
    private SerializedProperty collisionSystem;
    private SerializedProperty collisionSound;
    private SerializedProperty optionalMeshList;
    private SerializedProperty demolutionStrenght;

    private void OnEnable()
    {
        myTarget = (AnyCarAI)target;
        soTarget = new SerializedObject(target);

        demolutionStrenght = soTarget.FindProperty("demolutionStrenght");
        collisionSound = soTarget.FindProperty("collisionSound");
        optionalMeshList = soTarget.FindProperty("optionalMeshList");
        collisionSystem = soTarget.FindProperty("collisionSystem");
        persuitAI = soTarget.FindProperty("persuitAI");
        persuitDistance = soTarget.FindProperty("persuitDistance");
        persuitTarget = soTarget.FindProperty("persuitTarget");
        frontLightsOn = soTarget.FindProperty("frontLightsOn");
        reverseDistance = soTarget.FindProperty("reverseDistance");
        ABS = soTarget.FindProperty("ABS");
        brakeForce = soTarget.FindProperty("brakeForce");
        turnSpeed = soTarget.FindProperty("turnSpeed");
        waypointDistance = soTarget.FindProperty("waypointDistance");
        sensorAngle = soTarget.FindProperty("sensorAngle");
        sensorLenght = soTarget.FindProperty("sensorLenght");
        acceleration = soTarget.FindProperty("acceleration");
        suspensionDistance = soTarget.FindProperty("suspensionDistance");
        suspensionDumper = soTarget.FindProperty("suspensionDumper");
        suspensionSpring = soTarget.FindProperty("suspensionSpring");
        dumpingRate = soTarget.FindProperty("dumpingRate");
        wheelsPosition = soTarget.FindProperty("wheelsPosition");
        wheelStiffness = soTarget.FindProperty("wheelStiffness");
        vehicleMass = soTarget.FindProperty("vehicleMass");
        skidMarks = soTarget.FindProperty("skidMarks");
        SkidMaterial = soTarget.FindProperty("SkidMaterial");
        frontLeft = soTarget.FindProperty("frontLeft");
        frontRight = soTarget.FindProperty("frontRight");
        backLeft = soTarget.FindProperty("backLeft");
        backRight = soTarget.FindProperty("backRight");
        AITarget = soTarget.FindProperty("AITarget");
        maxSpeed = soTarget.FindProperty("maxSpeed");
        engineSound = soTarget.FindProperty("engineSound");
        extraWheels = soTarget.FindProperty("extraWheels");
        bodyMesh = soTarget.FindProperty("bodyMesh");
        maxSteerAngle = soTarget.FindProperty("maxSteerAngle");
        CenterOfMass = soTarget.FindProperty("CenterOfMass");
        wheelsRotation = soTarget.FindProperty("wheelsRotation");
        gearRatios = soTarget.FindProperty("gearRatios");
        brakeDistance = soTarget.FindProperty("brakeDistance");
    }


    public override void OnInspectorGUI()
    {
        soTarget.Update();

        EditorGUI.BeginChangeCheck();


        EditorGUILayout.Space();
        myTarget.toolbarTab = GUILayout.Toolbar(myTarget.toolbarTab, new string[] { "Set Up", "Wheels", "AI", "Features" });
        EditorGUILayout.Space();

        switch (myTarget.toolbarTab)
        {
            case 0:
                myTarget.currentTab = "Set Up";
                break;
            case 1:
                myTarget.currentTab = "Wheels";
                break;
            case 2:
                myTarget.currentTab = "AI";
                break;
            case 3:
                myTarget.currentTab = "Features";
                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        EditorGUI.BeginChangeCheck();

        switch (myTarget.currentTab)
        {
            case "Set Up":
                EditorGUILayout.LabelField("Model References", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Wheels", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(frontLeft);
                EditorGUILayout.PropertyField(frontRight);
                EditorGUILayout.PropertyField(backLeft);
                EditorGUILayout.PropertyField(backRight);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(extraWheels, true);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Body", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(bodyMesh);
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.Label("BodyMesh Debug");
                if (GUILayout.Button("Create BodyCollider")) { myTarget.CreateDebugBodyCol(); }
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                if (GUILayout.Button("Attach Script"))
                {
                    myTarget.CreateColliders();
                }
                EditorGUILayout.Space();
                break;
            case "Wheels":
                EditorGUILayout.LabelField("Attributes", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(dumpingRate);
                EditorGUILayout.PropertyField(wheelStiffness);
                EditorGUILayout.PropertyField(suspensionDistance);
                EditorGUILayout.PropertyField(suspensionDumper);
                EditorGUILayout.PropertyField(suspensionSpring);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(vehicleMass);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Steering", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(maxSteerAngle);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(wheelsRotation);
                EditorGUILayout.PropertyField(wheelsPosition);
                EditorGUILayout.Space();
                break;
            case "AI":
                EditorGUILayout.LabelField("Artificial Intelligence", EditorStyles.boldLabel);                
                EditorGUILayout.PropertyField(AITarget);
                EditorGUILayout.PropertyField(waypointDistance);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Characteristics", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(acceleration);
                EditorGUILayout.PropertyField(maxSpeed);
                EditorGUILayout.PropertyField(brakeForce);
                EditorGUILayout.PropertyField(brakeDistance);
                EditorGUILayout.PropertyField(reverseDistance);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(engineSound);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Sensors", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(sensorAngle);
                EditorGUILayout.PropertyField(sensorLenght);
                EditorGUILayout.PropertyField(turnSpeed);
                EditorGUILayout.Space();
                break;
            case "Features":
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Customize", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(frontLightsOn);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(ABS);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(collisionSystem);
                if (collisionSystem.boolValue == true)
                {
                    EditorGUILayout.IntSlider(demolutionStrenght, 0, 5, "Demolution Strenght");
                    EditorGUILayout.PropertyField(collisionSound);
                    EditorGUILayout.PropertyField(optionalMeshList, true);
                }
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(skidMarks);
                if (skidMarks.boolValue == true) { EditorGUILayout.PropertyField(SkidMaterial); }
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(persuitAI);
                if (persuitAI.boolValue == true) { EditorGUILayout.PropertyField(persuitTarget); EditorGUILayout.PropertyField(persuitDistance); }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(CenterOfMass);
                EditorGUILayout.PropertyField(gearRatios, true);
                EditorGUILayout.Space();
                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
        }
    }
}

