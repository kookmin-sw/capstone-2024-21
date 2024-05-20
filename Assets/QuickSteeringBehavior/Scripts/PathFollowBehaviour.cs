using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowBehaviour : SeekAndFleeBehaviour
{
    [Header("PathFollow")]
    [SerializeField] private bool ShowPathFollowGizmos;
    public bool loop=false;
    public float distanceToChangeTarget=0.3f;
    private int currentTargetIndex=-1;

    protected override void CalculateTargets()
    {
        if (_targetDistance <= distanceToChangeTarget)
        {
            NextTarget();
            Target = targets[currentTargetIndex];
        }
    }

    public void NextTarget()
    {
        if (currentTargetIndex < targets.Count-1)
        {
            currentTargetIndex++;
        }
        else if(loop)
        {
            currentTargetIndex = 0;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (ShowPathFollowGizmos)
        {
            Gizmos.color = Color.magenta;
            if (targets.Count > 0)
            {
                for(int i = 0; i < targets.Count; i++)
                {
                    if (i < targets.Count - 1)
                    {
                        Gizmos.DrawRay(targets[i].position, (targets[i+1].position - targets[i].position).normalized * Vector3.Distance(targets[i+1].position, targets[i].position));
                    }
                    else
                    {
                        Gizmos.DrawRay(targets[i].position, (targets[0].position - targets[i].position).normalized * Vector3.Distance(targets[0].position, targets[i].position));
                    }
                }
            }
        }
    }
}
