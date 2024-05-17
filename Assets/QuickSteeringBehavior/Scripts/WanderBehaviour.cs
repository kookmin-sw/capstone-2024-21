using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderBehaviour : SeekAndFleeBehaviour
{
    [Header("Wander")]
    [SerializeField] private bool ShowWanderGizmos;
    public float distanceSpawnTarget = 6f;
    public float distanceToCalculateNewTarget = 3f;
    public float areaToSpawnRadius =3f;
    private float randomX, randomY, randomZ;

    protected override void Awake()
    {
        base.Awake();
        if (Target != null)
            Target.parent = null;
    }

    protected override void Update()
    {
        if(_targetDistance< distanceToCalculateNewTarget)
        {
            randomX = Random.Range(-areaToSpawnRadius, areaToSpawnRadius);
            randomY = Random.Range(-areaToSpawnRadius, areaToSpawnRadius);
            randomZ = Random.Range(-areaToSpawnRadius, areaToSpawnRadius);
            Target.position = transform.position + (transform.forward * distanceSpawnTarget) + new Vector3(randomX, randomY, randomZ);
        }
        base.Update();
    }

    protected override void CalculateTargets()
    {
        
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (ShowWanderGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + (transform.forward* distanceSpawnTarget), areaToSpawnRadius);
        }
    }
}
