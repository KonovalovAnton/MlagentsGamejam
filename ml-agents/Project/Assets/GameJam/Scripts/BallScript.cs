using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BallScript : MonoBehaviour
{
    [SerializeField] private JuggleAgent _juggler;
    private Vector3 _initialPosition;
    public Rigidbody Rigidbody;
    [FormerlySerializedAs("HandType")] public HandScript.HandType LastCollidedHand;

    private void Awake()
    {
        _initialPosition = transform.position;
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void ResetPosition()
    {
        transform.position = _initialPosition;
        Rigidbody.velocity = Vector3.zero;
        LastCollidedHand = HandScript.HandType.None;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<HandScript>(out var hand))
        {
            _juggler.ReportCollisionWithHand(this, hand.Hand);
            LastCollidedHand = hand.Hand;
        }
    }
}
