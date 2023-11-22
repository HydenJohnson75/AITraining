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
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    private ConeFieldOfView fieldOfView;
    private bool foundVisibleTargets;
    private bool touchedTarget;

    [SerializeField] private Material winMaterial;
    [SerializeField] private Material looseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    private float timer;
    private bool touchingButton;
    private Quaternion startingRotation;

    private void Start()
    {
        fieldOfView = GetComponent<ConeFieldOfView>();
        startingRotation = transform.rotation;
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

    }

    public override void OnEpisodeBegin()
    {
        //target.gameObject.SetActive(false);
        if(touchedTarget == false) 
        {
            SetReward(0.6f);
        }
        touchingButton = false;
        transform.localPosition = Vector3.zero;
        transform.rotation = startingRotation;
        //new Vector3(UnityEngine.Random.Range(-4f, +6f), 0, UnityEngine.Random.Range(-4f, +4f));
        //target.localPosition = new Vector3(UnityEngine.Random.Range(-5f, +3.5f), 0, UnityEngine.Random.Range(-7f, +7f));
        foundVisibleTargets = false;
        touchedTarget = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observations: Agent's position, target's position, and rotation.
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localRotation);
        //sensor.AddObservation(touchingButton ? 1 : 0);
        sensor.AddObservation(foundVisibleTargets ? 1 : 0);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Actions:
        // - vectorAction[0]: Move forward (1) or backward (-1)
        // - vectorAction[1]: Rotate left (-1) or right (1)

        float moveDirection = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float rotateDirection = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        // Move the agent forward or backward.
        transform.Translate(Vector3.forward * moveDirection * moveSpeed * Time.fixedDeltaTime);

        // Rotate the agent left or right.
        transform.Rotate(Vector3.up * rotateDirection * rotationSpeed * Time.fixedDeltaTime);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousAction = actionsOut.ContinuousActions;

        // Implement manual control for testing.
        continousAction[0] = Input.GetAxis("Vertical"); // Forward/Backward
        continousAction[1] = Input.GetAxis("Mouse X"); // Left/Right
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (foundVisibleTargets && collision.TryGetComponent<AI_Box_Target>(out AI_Box_Target target))
        {
            SetReward(1.0f);
            floorMeshRenderer.material = winMaterial;
            touchedTarget = true;
            EndEpisode();
        }
        else
        {
            SetReward(-0.8f);
            EndEpisode();
        }
        if (collision.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            floorMeshRenderer.material = looseMaterial;
            touchedTarget = false;
            EndEpisode();
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Button_Script>(out Button_Script button))
        {
            touchingButton = true;
            Debug.Log("colliding");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Button_Script>(out Button_Script button))
        {
            touchingButton = false;
        }
    }
}
