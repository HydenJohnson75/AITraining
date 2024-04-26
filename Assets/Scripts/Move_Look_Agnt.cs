using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;

public class Move_Look_Agnt : Agent
{ 
    float moveSpeed =8f;
    float rotationSpeed = 30f;
    private bool touchedTarget;
    private float xRotation;
    private float yRotation;
    private Rigidbody rb;
    float timer = 0;
    float shootTimer;
    float maxTimer = 1;
    float timeBetweenBullets = 0.15f;
    List<Transform> targetLocations = new List<Transform>();
    [SerializeField] GameObject spawnLocations;
    [SerializeField] GameObject singleSpawnLoc;
    [SerializeField] GameObject enemyObject;
    [SerializeField] GameObject shootPos;
    [SerializeField] GameObject rayObject;
    [SerializeField] RayPerceptionSensorComponent3D rayPerception;
    [SerializeField] GameObject startPosition;
    [SerializeField] Camera mainCam;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material looseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private AgentShoot _aiShoot;
    private bool touchingButton;
    private Quaternion startingRotation;
    private Quaternion startingCameraRotation;
    internal bool isLookingAtTarget = false;
    internal bool isLookingAtFloor = false;
    private bool hasShot = false;
    private int numOfHits = 0;
    private GameObject spawnedEnemy;
    private void Start()
    {
        if(spawnLocations != null)
        {
            targetLocations = spawnLocations.GetComponentsInChildren<Transform>().ToList<Transform>();

        }
        startingRotation = transform.rotation;
        startingCameraRotation = mainCam.transform.rotation;
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(timer <=0)
        {
            UpdateLookingAtTarget();
            timer = maxTimer;
        }
        timer -= Time.deltaTime;


    }

    private void UpdateLookingAtTarget()
    {
        if (rayPerception != null)
        {
            var rayOutputs = RayPerceptionSensor.Perceive(rayPerception.GetRayPerceptionInput()).RayOutputs;
            int lengthOfRayOutputs = rayOutputs.Length;

            if (rayOutputs != null)
            {
                int numberOfHitTages = 0;
                int numberOfFloorTags = 0;
                for (int i = 0; i < lengthOfRayOutputs; i++)
                {
                    if (rayOutputs[i].HitGameObject != null && rayOutputs[i].HitGameObject.tag == "Enemy")
                    {
                        numberOfHitTages++;
                    }
                    if (rayOutputs[i].HitGameObject != null && rayOutputs[i].HitGameObject.tag == "Floor")
                    {
                        numberOfFloorTags++;
                    }

                }

                if (numberOfHitTages > 0)
                {
                    isLookingAtTarget = true;
                }
                else
                {
                    isLookingAtTarget = false;
                }
                if(numberOfFloorTags > 0)
                {
                    isLookingAtFloor = true;
                }
                else
                {
                    isLookingAtFloor = false;
                }
            }
        }

    }

    public override void OnEpisodeBegin()
    {



        numOfHits = 0;
        //target.gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
        //transform.localPosition = new Vector3(UnityEngine.Random.Range(-4f, +6f), 0, UnityEngine.Random.Range(-4f, +4f));
       // target.localPosition = new Vector3(UnityEngine.Random.Range(-5f, +3.5f), 0, UnityEngine.Random.Range(-7f, +7f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observations: Agent's position, target's position, and rotation.
        sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(mainCam.transform.rotation.x);
        sensor.AddObservation(rb.velocity.normalized);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {


        float moveDirection = actions.ContinuousActions[0];
        float moveRightDirection = actions.ContinuousActions[1];
        float rotateDirection = actions.ContinuousActions[2];
        float rotateUpDirection = actions.ContinuousActions[3];
        //ActionSegment<int> discreteActions = actions.DiscreteActions;

        rb.velocity = ((transform.forward * moveDirection + transform.right * moveRightDirection) * moveSpeed);
        // Calculate rotation based on input
        float rotation = rotateDirection * rotationSpeed * Time.deltaTime;
        float rotationUp = rotateUpDirection * rotationSpeed * Time.deltaTime;

        // Apply rotation to the agent's Y-axis (horizontal rotation)
        transform.Rotate(0, rotation, 0);

        // Rotate the camera based on vertical input
        xRotation -= rotationUp;

        xRotation = Mathf.Clamp(xRotation, -10f, 10f);

        mainCam.transform.localEulerAngles = new Vector3(xRotation, 0, 0);

        rayObject.transform.localRotation = mainCam.transform.localRotation;


    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousAction = actionsOut.ContinuousActions;

        // Implement manual control for testing.
        continousAction[0] = Input.GetAxis("Vertical");
        continousAction[1] = Input.GetAxis("Horizontal");
        continousAction[2] = Input.GetAxis("Horizontal2");
        continousAction[3] = Input.GetAxis("Vertical2");
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "Enemy" && isLookingAtTarget == true)
        {
            AddReward(1f);
            if(floorMeshRenderer != null)
            {
                floorMeshRenderer.material = winMaterial;
            }
            touchedTarget = true;
            EndEpisode();
        }
        else if (collision.gameObject.tag == "Enemy" && isLookingAtTarget == false)
        {
            if(floorMeshRenderer != null)
            {
                floorMeshRenderer.material = looseMaterial;
            }
            touchedTarget = false;
            AddReward(0.5f);
            EndEpisode();
        }

        if (collision.gameObject.tag == "Wall")
        {
            AddReward(-1.5f);
            if(floorMeshRenderer != null)
            {
                floorMeshRenderer.material = looseMaterial;
            }
            Destroy(spawnedEnemy);
            EndEpisode();
        }

    }

    public void Kill()
    {
        Destroy(this.gameObject);
    }
}
