using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockBehaviour : SeekAndFleeBehaviour
{
    [Header("Flocking")]
    [SerializeField] private bool ShowFlockGizmos;
    public float _neighborhood_distance = 5f;
    public float _separation_radius = 2f;

    [Range(0.0f, 1.0f)]
    public float _aligment_factor = 0.25f;

    [Range(0.0f, 1.0f)]
    public float _separation_factor = 0.5f;

    [Range(0.0f, 1.0f)]
    public float _cohesion_factor = 0.25f;

    protected GameObject _currentTarget;
    protected GameObject initialTarget;

    private static List<FlockBehaviour> flocks = new List<FlockBehaviour>();

    private List<FlockBehaviour> flocks_neighborhood = new List<FlockBehaviour>();
    private FlockBehaviour local_leader = null;
    private float distanceToTarget, speed;

    protected override void Awake()
    {
        base.Awake();
        initialTarget = Target.gameObject;
    }

    private void Start()
    {
        _currentTarget = initialTarget;
    }

    void OnEnable()
    {
        flocks.Add(this);
    }

    void OnDisable()
    {
        flocks.Remove(this);
    }

    protected override void CalculateTargets()
    {
        base.CalculateTargets();
        initialTarget = Target.gameObject;
    }

    protected override void Update()
    {
            distanceToTarget = Vector3.Distance(this.transform.position, initialTarget.transform.position);
            base.Update();
    }

    protected override float CalculateSpeed()
    {
        if (_currentTarget == initialTarget)
        {
            speed = base.CalculateSpeed();
            return speed;
        }
        else
        {
            speed = local_leader.speed;
            return local_leader.speed;
        }
    }

    protected override Vector3 CalculateDirection()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alineation = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 center = Vector3.zero; 

        int neighbourCount = 0;
        flocks_neighborhood.Clear();
        flocks_neighborhood.Add(this);

        foreach (var flock in flocks)
        {
            if (flock == this) continue;

            float dist = Vector3.Distance(flock.transform.position, transform.position);
            if (dist > _neighborhood_distance) continue;

            flocks_neighborhood.Add(flock);
            neighbourCount++;

            if (dist < _separation_radius) 
            {
                float n = 1 - dist / _separation_radius;
                separation += (transform.position - flock.transform.position).normalized * n;
            }

            alineation += _currentTarget.transform.position - flock.transform.position;

            center += flock.transform.position + (_currentTarget.transform.position - flock.transform.position); 
        }

        flocks_neighborhood.Sort((p1, p2) => p1.distanceToTarget.CompareTo(p2.distanceToTarget));

        if (flocks_neighborhood[0] != this)
        {
            int index = flocks_neighborhood.FindIndex((f1)=>f1==this);
            _currentTarget = flocks_neighborhood[0].gameObject;
            local_leader = flocks_neighborhood[0];
        }
        else
            _currentTarget = initialTarget;

        if (neighbourCount == 0)
            return initialTarget.transform.position - transform.position;

        center /= neighbourCount; 
        cohesion = center - transform.position;

        return (separation.normalized * _separation_factor + cohesion.normalized * _cohesion_factor + alineation.normalized * _aligment_factor);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (ShowFlockGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, _separation_radius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, _neighborhood_distance);
        }
    }
}
