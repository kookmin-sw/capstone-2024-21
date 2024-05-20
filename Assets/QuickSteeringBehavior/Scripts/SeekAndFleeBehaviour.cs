using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAndFleeBehaviour : SteeringBehaviourWithTargets
{
    [Header("SeekAndFlee")]
    [SerializeField] private bool ShowSeekAndFleeGizmos;
    public bool Flee = false;
    public float BrakingDistance=3f;
    private Vector3 dir;

    protected override float CalculateSpeed()
    {
        if (Flee)
        {
            return 1;
        }
        else
        {
            if(Target==null)
                return 0;
            else
            {
                _targetDistance = Vector3.Distance(transform.position, Target.position); 
                if(_targetDistance < BrakingDistance)
                    return _targetDistance / BrakingDistance; 
                else
                    return 1;
            }
        }
    }

    protected override Vector3 CalculateDirection()
    {       
        dir = Flee ? transform.position- Target.position: Target.position - transform.position; 
        return Target==null ? transform.forward : dir.normalized;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (ShowSeekAndFleeGizmos)
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
        }
    }
}
