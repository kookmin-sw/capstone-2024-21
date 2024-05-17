using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacleBehaviour : SeekAndFleeBehaviour
{
    [Header("Avoid Obstacle")]
    [SerializeField] private bool ShowAvoidObstacleGizmos;
    [Range(0, 1)] public float verticalAngle =1;
    public int horizontalAccuracy = 0;
    public LayerMask mask;
    public float RaycastDistance;
    [Range(0f, 1f)] public float Separation = 1f;
    [Range(1f, 100f)] public float CorrectionFactor = 50f;

    private float _front, _left, _right, _up, _down;
    private Vector3 frontRaycast;
    private RaycastHit raycastHitFront, raycastHitLeft, raycastHitRight, raycastHitUp, raycastHitDown;
    [HideInInspector] public Vector3 directionCalculated;
    [HideInInspector] public bool obstacles;

    protected override Vector3 CalculateDirection()
    {
        obstacles = true;
        frontRaycast = transform.forward.normalized * RaycastDistance;

        _front = _left = _right = _up = _down = 0;


        Physics.Raycast(transform.position, frontRaycast.normalized, out raycastHitFront, RaycastDistance*2, mask);
        _front = raycastHitFront.distance;
        Physics.Raycast(transform.position, (transform.forward * verticalAngle + transform.up * (1 - verticalAngle)).normalized, out raycastHitUp, RaycastDistance, mask);
        _up = raycastHitUp.distance;
        Physics.Raycast(transform.position, (transform.forward * verticalAngle - transform.up * (1 - verticalAngle)).normalized, out raycastHitDown, RaycastDistance * 2, mask);
        _down = raycastHitDown.distance;

        float subdivision = (90f / (float)horizontalAccuracy) / 100f;
        for (int i = 0; i < horizontalAccuracy; i++)
        {
            Physics.Raycast(transform.position, ((transform.forward * (subdivision * i)) + transform.right * (horizontalAccuracy - i) * subdivision).normalized, out raycastHitLeft, RaycastDistance, mask);
            float rightTemp = raycastHitLeft.distance;
            Physics.Raycast(transform.position, ((transform.forward * (subdivision * i)) - transform.right * (horizontalAccuracy - i) * subdivision).normalized, out raycastHitRight, RaycastDistance, mask);
            float leftTemp = raycastHitRight.distance;

            if (rightTemp > _right)
                _right = rightTemp;

            if (leftTemp > _left)
                _left = leftTemp;
        }

        if (_front != 0 && _right <= _left)
        {
            directionCalculated = transform.right;
        }
        else if (_front != 0 && _right > _left)
        {
            directionCalculated = -transform.right;
        }
        else if (_front == 0 && _right < _left)
        {
            if (transform.forward.z < Separation)
                directionCalculated = transform.forward + transform.right / CorrectionFactor;
            else
                directionCalculated = transform.forward;
        }
        else if (_front == 0 && _right > _left)
        {
            if (transform.forward.z > -Separation)
                directionCalculated = transform.forward - transform.right / CorrectionFactor;
            else
                directionCalculated = transform.forward;
        }
        else
        {
            directionCalculated = Target.position - transform.position;
            obstacles = false;
        }

        if (verticalAngle != 1)
        {
            if (_up < _down)
            {
                directionCalculated += transform.up;
            }
            else if (_up > _down)
            {
                directionCalculated += -transform.up;
            }
        }

        return directionCalculated;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (ShowAvoidObstacleGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward.normalized * RaycastDistance);

            if (verticalAngle != 1)
            {
                Gizmos.DrawRay(transform.position, (transform.forward * verticalAngle + transform.up * (1 - verticalAngle)).normalized * RaycastDistance * 2);
                Gizmos.DrawRay(transform.position, (transform.forward * verticalAngle - transform.up * (1 - verticalAngle)).normalized * RaycastDistance * 2);
            }

            float subdivision = (90f / (float)horizontalAccuracy) / 100f;
            for (int i = 0; i < horizontalAccuracy; i++)
            {
                Gizmos.DrawRay(transform.position, ((transform.forward * (subdivision * i)) + transform.right * (horizontalAccuracy - i) * subdivision).normalized * RaycastDistance);
                Gizmos.DrawRay(transform.position, ((transform.forward * (subdivision * i)) - transform.right * (horizontalAccuracy - i) * subdivision).normalized * RaycastDistance);
            }
        }
    }
}
