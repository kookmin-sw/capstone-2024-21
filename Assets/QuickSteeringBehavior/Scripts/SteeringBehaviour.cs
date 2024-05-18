using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    [Header("Steering Behaviour Basic")]
    [SerializeField] private bool ShowSteeringGizmos;
    public float maxSpeed = 3f;
    public float angularSpeed = 180f;

    [HideInInspector] public Vector3 _desiredDir;
    [HideInInspector] public float _desiredSpeed;

    protected virtual void Update()
    {
        _desiredDir = CalculateDirection();
        _desiredSpeed = CalculateSpeed();

        Rotate();
        Move();
    }
    protected virtual void OnDrawGizmosSelected()
    {
        if (ShowSteeringGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + _desiredDir.normalized * 2);
        }
    }

    protected virtual void Move()
    {
        transform.position += GetCurrentSpeed() * transform.forward * Time.deltaTime;
    }

    protected virtual void Rotate()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_desiredDir), angularSpeed * Time.deltaTime);
    }

    public float GetCurrentSpeed()
    {
        return _desiredSpeed * maxSpeed;
    }
    protected abstract float CalculateSpeed();
    protected abstract Vector3 CalculateDirection();
}
