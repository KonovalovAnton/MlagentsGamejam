using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class WalkingAgent : Agent
{
    [SerializeField] private float speed;
    [SerializeField] private IncomeArea incomeZone;
    private Vector2 desiredVelocity;
    private Vector3 lastPosition;
    private void FixedUpdate()
    {
        transform.position = transform.position + new Vector3(
                                 desiredVelocity.x,
                                 0,
                                 desiredVelocity.y
                            ).normalized * Time.fixedDeltaTime * speed;

        if (StepCount > 5000)
        {
            EndEpisode();
        }
    }

    public void GetIncomeReward()
    {
        AddReward((incomeZone.transform.localPosition - lastPosition).magnitude * 100);
        lastPosition = incomeZone.transform.localPosition;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 1, 0);
        incomeZone.ResetArea();
        lastPosition = new Vector3();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionZ = 2f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var actionX = 2f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        desiredVelocity = new Vector2(actionZ, actionX);
        AddReward((
            (incomeZone.transform.localPosition - lastPosition).magnitude
            -
            (incomeZone.transform.position - transform.position).magnitude)
            / 100
        );
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation((incomeZone.transform.position - transform.position).magnitude);
        sensor.AddObservation(incomeZone.transform.localPosition);
    }
}
