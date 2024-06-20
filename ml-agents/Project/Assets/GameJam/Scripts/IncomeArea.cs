using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Random = UnityEngine.Random;

public class IncomeArea : MonoBehaviour
{
    [SerializeField] private WalkingAgent m_Agent;

    public void ResetArea()
    {
        transform.localPosition = new Vector3(Random.value * 30 - 15, 0, Random.value * 30 - 15);
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_Agent.gameObject == other.gameObject)
        {
            m_Agent.GetIncomeReward();
            ResetArea();
        }
    }
}
