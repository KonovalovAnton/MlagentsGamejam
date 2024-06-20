using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class JuggleAgent : Agent
{
    [SerializeField] private List<BallScript> balls;
    [SerializeField] private HandScript leftHand;
    [SerializeField] private HandScript rightHand;
    [SerializeField] private float jugglePower;

    private Queue<BallScript> toPush = new Queue<BallScript>();

    public override void OnEpisodeBegin()
    {
        foreach (var ball in balls)
        {
            ball.ResetPosition();
        }
    }

    private void FixedUpdate()
    {
        foreach (var ball in balls)
        {
            if (ball.transform.position.y < leftHand.transform.position.y - 5)
            {
                EndEpisode();
                return;
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (var ball in balls)
        {
            sensor.AddObservation((int)ball.LastCollidedHand);
            sensor.AddObservation(ball.transform.position);
            sensor.AddObservation(ball.Rigidbody.velocity);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heuristic");
        if (Input.GetAxis("Vertical") > 0)
        {
            Debug.Log("W");
            while (toPush.Count > 0)
            {
                Debug.Log("PUSH");
                var ball = toPush.Dequeue();
                int index = balls.IndexOf(ball);
                ball.Rigidbody.AddForce(new Vector3(0, 1000, 0));
            }
        }
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        while (toPush.Count > 0)
        {
            var ball = toPush.Dequeue();
            int index = balls.IndexOf(ball);
            var xForce = actions.ContinuousActions[0 + index];
            var yForce = actions.ContinuousActions[1 + index];
            var zForce = actions.ContinuousActions[2 + index];
            var force = new Vector3(xForce, yForce, zForce) * jugglePower;
            Debug.Log(force);
            ball.Rigidbody.AddForce(force);
        }
    }

    public void ReportCollisionWithHand(BallScript ballScript, HandScript.HandType componentHand)
    {
        if (ballScript.LastCollidedHand != componentHand)
        {
            AddReward(1f);
        }
        else
        {
            SetReward(-1f);
            EndEpisode();
        }
        toPush.Enqueue(ballScript);
        RequestDecision();
    }
}
