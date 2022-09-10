using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineConstraintUsingUnitySplinesPackage : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _forwardForceCoefficient;
    ConfigurableJoint joint;

    private void Start()
    {
        joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.anchor = new Vector3(0, 0, 0);
        joint.autoConfigureConnectedAnchor = false;
        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
    }

    private void FixedUpdate()
    {
        UpdateConstraint();
        AddAirFriction();
        AddForwardForce();
    }

    private void UpdateConstraint()
    {
        SplineUtility.GetNearestPoint(_splineContainer.Spline, new float3(transform.position), out var nearest, out var nearestTime);
        joint.connectedAnchor = nearest;
        joint.axis = _splineContainer.EvaluateTangent(nearestTime);
    }

    private void AddAirFriction()
    {
        var p = 1.225f;
        var cd = .47f;
        var a = Mathf.PI * 0.5f * 0.5f;
        var v = _rigidBody.velocity.magnitude;

        var direction = _rigidBody.velocity.normalized;
        var forceAmount = (p * v * v * cd * a) / 2;
        _rigidBody.AddForce(-direction * forceAmount);
    }

    private void AddForwardForce()
    {
        if (Input.GetKey(KeyCode.U))
        {
            _rigidBody.AddForce(joint.axis * _forwardForceCoefficient);
        }
    }
}
