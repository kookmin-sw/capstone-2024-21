using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitAndEvade : SteeringBehaviourWithTargets
{
    [Header("PursuitAndEvade")]
    [SerializeField] private bool ShowPursuitAndEvadeGizmos;
    public bool Evade = false;
    public float BrakingDistance = 3f;
    private Vector3 FutureTargetPosition;
    private Vector3 dir;
    protected override float CalculateSpeed()
    {
        if (Evade)
        {
            return 1;
        }
        else
        {
            if (Target == null)
                return 0;
            else
            {
                _targetDistance = Vector3.Distance(transform.position, Target.position);
                if (_targetDistance < BrakingDistance)
                    return _targetDistance / BrakingDistance;
                else
                    return 1;
            }
        }
    }

    protected override Vector3 CalculateDirection()
    {
        if(Target.TryGetComponent(out SteeringBehaviour steeringBehaviourTarget)){
            FutureTargetPosition = Target.position + steeringBehaviourTarget._desiredDir.normalized * steeringBehaviourTarget._desiredSpeed * steeringBehaviourTarget.maxSpeed;
            dir = Evade ? transform.position- FutureTargetPosition : FutureTargetPosition - transform.position;
        }
        return Target==null ? transform.forward : dir.normalized;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (ShowPursuitAndEvadeGizmos)
        {
            Gizmos.color = Color.yellow;

            if (!multipleTargets && Target != null)
            {
                Gizmos.DrawWireSphere(Target.position, BrakingDistance);
            }
            else if (multipleTargets && targets.Count > 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    Gizmos.DrawWireSphere(targets[i].position, BrakingDistance);
                }
            }

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(FutureTargetPosition, 0.2f);  
                
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, Vector3.Distance(transform.position, FutureTargetPosition)* (FutureTargetPosition-transform.position).normalized);           
        }
    }
}
