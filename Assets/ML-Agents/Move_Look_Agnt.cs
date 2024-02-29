using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System;

public class Move_Look_Agnt : Agent
{
    public Transform target;  // The target object to look at.
    public float moveSpeed =5f;
    public float rotationSpeed = 100f;
    private bool touchedTarget;
    private float xRotation;
    private float yRotation;
    private Rigidbody rb;
    float timer = 0;
    float maxTimer = 1;


    [SerializeField] GameObject rayObject;
    [SerializeField] RayPerceptionSensorComponent3D rayPerception;
    [SerializeField] GameObject startPosition;
    [SerializeField] Camera mainCam;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material looseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    private bool touchingButton;
    private Quaternion startingRotation;
    private Quaternion startingCameraRotation;
    internal bool isLookingAtTarget = false;
    internal bool isLookingAtFloor = false;
    private void Start()
    {
        
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

        print(GetCumulativeReward());
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

                print(numberOfHitTages);

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
        //target.gameObject.SetActive(false);
        transform.position = startPosition.transform.position;
        mainCam.transform.rotation  = startingCameraRotation; 
        transform.rotation = startingRotation;

        //transform.localPosition = new Vector3(UnityEngine.Random.Range(-4f, +6f), 0, UnityEngine.Random.Range(-4f, +4f));
        //target.localPosition = new Vector3(UnityEngine.Random.Range(-5f, +3.5f), 0, UnityEngine.Random.Range(-7f, +7f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observations: Agent's position, target's position, and rotation.
        sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(target.localPosition);
        sensor.AddObservation(isLookingAtFloor);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(mainCam.transform.localRotation);
        sensor.AddObservation(rb.velocity.normalized);
        sensor.AddObservation(isLookingAtTarget);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {


        float moveDirection = actions.ContinuousActions[0];
        float moveRightDirection = actions.ContinuousActions[1];


        // Calculate movement based on input
        Vector3 move = transform.forward * moveDirection * moveSpeed * Time.deltaTime;
        move += transform.right * moveRightDirection * moveSpeed * Time.deltaTime;

        // Apply movement
        rb.MovePosition(transform.position + move);


    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousAction = actionsOut.ContinuousActions;

        // Implement manual control for testing.
        continousAction[0] = Input.GetAxis("Vertical");
        continousAction[1] = Input.GetAxis("Horizontal");


    }

    private void OnTriggerEnter(Collider collision)
    {
        print(collision.gameObject.tag);

        if (collision.gameObject.tag == "Enemy" && isLookingAtTarget == true)
        {
            AddReward(1f);
            floorMeshRenderer.material = winMaterial;
            touchedTarget = true;
            EndEpisode();
        }
        else if (collision.gameObject.tag == "Enemy" && isLookingAtTarget == false)
        {
            floorMeshRenderer.material = looseMaterial;
            touchedTarget = false;
            EndEpisode();
        } 

        if (collision.gameObject.tag == "Wall")
        {
            AddReward(-0.5f);
            floorMeshRenderer.material = looseMaterial;
            touchedTarget = false;
            EndEpisode();
        }

        
    }
}
