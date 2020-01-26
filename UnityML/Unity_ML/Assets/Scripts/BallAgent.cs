using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BallAgent : Agent
{
    private Rigidbody ballRigidody;
    public Transform pivotTransform;
    public Transform target;

    public float moveForce = 10f;

    private bool isTargetEaten = false;
    private bool isDead = false;

    private void Awake()
    {
        ballRigidody = GetComponent<Rigidbody>();
    }

    private void resetTarget()
    {
        isTargetEaten = false;
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        target.position = randomPos + pivotTransform.position;
    }

    public override void AgentReset()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        transform.position = randomPos + pivotTransform.position;

        isDead = false;
        ballRigidody.velocity = Vector3.zero;

        resetTarget();
    }

    public override void CollectObservations()
    {
        Vector3 distanceToTarget = target.position - transform.position;

        AddVectorObs(Mathf.Clamp(distanceToTarget.x / 5f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(distanceToTarget.z / 5f, -1f, 1f));

        Vector3 relativePos = transform.position - pivotTransform.position;

        AddVectorObs(Mathf.Clamp(relativePos.x / 5f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(relativePos.z / 5f, -1f, 1f));

        AddVectorObs(Mathf.Clamp(ballRigidody.velocity.x / 10f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(ballRigidody.velocity.z / 10f, -1f, 1f));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.001f);

        float horizontalInput = vectorAction[0];
        float verticalInput = vectorAction[1];

        ballRigidody.AddForce(horizontalInput * moveForce, 0f, verticalInput * moveForce);

        if (isTargetEaten)
        {
            AddReward(1.0f);
            resetTarget();
        }
        else if (isDead)
        {
            AddReward(-1.0f);
            Done();
        }

        Monitor.Log(name, GetCumulativeReward(), transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("dead"))
        {
            isDead = true;
        }else if(other.CompareTag("goal"))
        {
            isTargetEaten = true;
        }
        
    }
}
