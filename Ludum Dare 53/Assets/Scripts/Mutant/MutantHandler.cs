using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MutantHandler : MonoBehaviour
{

    public MutantType.Mutant currentMutantTyp;
    public NavMeshAgent navMeshAgent;
    public float movementRadius = 5f;
    public float timeUntilNewWaypoint = 5f;
    public List<Rigidbody> flyAwayPartsRigidbodys;
    public float flyAwayPartDestroyTimer = 3f;
    public float flyWayPushPower = 3f;

    public Animator anim;
    public UnityEvent OnKilled = new();


    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        GetNewWaypoint();
        anim ??= GetComponentInChildren<Animator>();

        foreach (Rigidbody rigidbody in flyAwayPartsRigidbodys)
        {
            rigidbody.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        anim.SetFloat("velocity", navMeshAgent.velocity.normalized.magnitude);
    }

    public void GetNewWaypoint()
    {
        Vector3 randomPoint;
        RandomPoint(transform.position, movementRadius, out randomPoint);
        navMeshAgent.SetDestination(randomPoint);
        Invoke(nameof(GetNewWaypoint), timeUntilNewWaypoint);
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }


    public void Kill(Transform Player)
    {

        foreach(Rigidbody rigidbody in flyAwayPartsRigidbodys)
        {
            rigidbody.transform.parent = null;
            Destroy(rigidbody.gameObject, flyAwayPartDestroyTimer);
            rigidbody.gameObject.SetActive(true);
            Vector3 flyAwayDirection = rigidbody.transform.position -Player.transform.position;
            flyAwayDirection = flyAwayDirection.normalized;
            rigidbody.AddForce(flyAwayDirection * flyWayPushPower + Vector3.up, ForceMode.Impulse);
        }
        OnKilled.Invoke();
        Destroy(gameObject);
    }
}
