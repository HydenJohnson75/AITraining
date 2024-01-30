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
    public float moveSpeed;
    public float rotationSpeed = 100f;
    private ConeFieldOfView fieldOfView;
    private bool foundVisibleTargets;
    private bool touchedTarget;
    private float xRotation;
    private float yRotation;
    private Rigidbody rb;

    [SerializeField] Camera mainCam;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material looseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    private float timer;
    private bool touchingButton;
    private Quaternion startingRotation;
    private Quaternion startingCameraRotation;

    private void Start()
    {
        
        startingRotation = transform.rotation;
        startingCameraRotation = mainCam.transform.rotation;
    }

    public override void Initialize()
    {
        fieldOfView = GetComponent<ConeFieldOfView>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (fieldOfView.FindVisableTargets())
        {
            foundVisibleTargets = true;
        }

        //if(touchingButton)
        //{
        //    if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        target.gameObject.SetActive(true);
        //    }
        //}

        //print(foundVisibleTargets);

    }

    public override void OnEpisodeBegin()
    {
        //target.gameObject.SetActive(false);
        touchingButton = false;
        //transform.localPosition = Vector3.zero;
        startingCameraRotation = mainCam.transform.rotation;
        transform.rotation = startingRotation;

        transform.localPosition = new Vector3(UnityEngine.Random.Range(-4f, +6f), 0, UnityEngine.Random.Range(-4f, +4f));
        target.localPosition = new Vector3(UnityEngine.Random.Range(-5f, +3.5f), 0, UnityEngine.Random.Range(-7f, +7f));
        foundVisibleTargets = false;
        touchedTarget = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observations: Agent's position, target's position, and rotation.
        sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localRotation);

        //sensor.AddObservation(touchingButton ? 1 : 0);
        sensor.AddObservation(foundVisibleTargets ? 1 : 0);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {


        float moveDirection = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveRightDirection = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        float rotateDirection = Mathf.Clamp(actions.ContinuousActions[2], -1f, 1f);
        float rotateUpDirection = Mathf.Clamp(actions.ContinuousActions[3], -1f, 1f);

        Vector3 camForward = mainCam.transform.forward;
        Vector3 camRight = mainCam.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        Vector3 cameraRelative = moveDirection * camForward * moveSpeed;
        Vector3 cameraRightRelative = moveRightDirection * camRight * moveSpeed;

        Vector3 velocity = cameraRelative + cameraRightRelative;

        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);


        //transform.Rotate(Vector3.up,rotateDirection * rotationSpeed * Time.deltaTime);

        xRotation -= rotateUpDirection * rotationSpeed * Time.deltaTime; 

        xRotation = Mathf.Clamp(xRotation, -80, 80);

        yRotation += rotateDirection * rotationSpeed * Time.deltaTime;


        mainCam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);


        transform.forward = mainCam.transform.forward;

        if(foundVisibleTargets == false)
        {
            AddReward(-0.3f/ MaxStep);
        }
        if(foundVisibleTargets == true)
        {
            AddReward(1f / MaxStep);
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousAction = actionsOut.ContinuousActions;

        // Implement manual control for testing.
        continousAction[0] = Input.GetAxis("Vertical");
        continousAction[1] = Input.GetAxis("Horizontal");
        continousAction[2] = Input.GetAxis("Mouse X"); // Left/Right
        continousAction[3] = Input.GetAxis("Mouse Y");
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        print(collision.gameObject.tag);
        print(foundVisibleTargets);

        if (collision.gameObject.tag == "Enemy" && foundVisibleTargets == true)
        {
            SetReward(2f);
            floorMeshRenderer.material = winMaterial;
            touchedTarget = true;
            EndEpisode();
        }
        else if (collision.gameObject.tag == "Enemy" && foundVisibleTargets == false)
        {
            SetReward(-0.1f);
            floorMeshRenderer.material = looseMaterial;
            touchedTarget = false;
            EndEpisode();
        } 

        if (collision.gameObject.tag == "Wall")
        {
            SetReward(-0.7f);
            floorMeshRenderer.material = looseMaterial;
            touchedTarget = false;
            EndEpisode();
        }
    }
}
