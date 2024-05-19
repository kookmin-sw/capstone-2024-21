using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviourWithTargets : SteeringBehaviour
{
    [Header("Targets to Steering")]
    public bool ShowTargetsGizmos;
    public bool multipleTargets=false;
    public Transform Target;
    public List<Transform> targets;
    GameObject[] playerObjects;
    protected float _targetDistance;
    protected int currrentTargetIndex = 0;

    protected virtual void Awake()
    {
        playerObjects = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < playerObjects.Length; i++)
        {
            targets.Add(playerObjects[i].transform);
        }
        multipleTargets = true;
        if (multipleTargets)
            Target = targets[0];
    }

    protected override void Update()
    {
        CalculateTargets();
        base.Update();
    }

    protected virtual void CalculateTargets()
    {
        if(Vector3.Distance(gameObject.transform.position, Target.position) < 0.1f)
        {
            targets.Clear();
            playerObjects = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < playerObjects.Length; i++)
            {
                targets.Add(playerObjects[i].transform);
            }
            multipleTargets = true;

            if (multipleTargets)
            {
                _targetDistance = 0;
                for (int i = 0; i < targets.Count; i++)
                {
                    var tempDistance = Vector3.Distance(transform.position, targets[i].position);
                    if (tempDistance > _targetDistance)
                    {
                        _targetDistance = tempDistance;
                        currrentTargetIndex = i;
                    }
                }
                Target = targets[currrentTargetIndex];
            }
        }

    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (ShowTargetsGizmos)
        {
            Gizmos.color = Color.red;
            if (!multipleTargets && Target!=null)
            {
                Gizmos.DrawRay(transform.position, (Target.position-transform.position).normalized * Vector3.Distance(transform.position, Target.position));
            }
            else if(multipleTargets && targets.Count > 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    Gizmos.DrawRay(transform.position, (targets[i].position-transform.position).normalized* Vector3.Distance(transform.position, targets[i].position));
                }
            }
        }
    }

    protected override Vector3 CalculateDirection()
    {
        throw new System.NotImplementedException();
    }

    protected override float CalculateSpeed()
    {
        throw new System.NotImplementedException();
    }
}
