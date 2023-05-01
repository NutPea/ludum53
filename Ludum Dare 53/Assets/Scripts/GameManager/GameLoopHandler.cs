using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GameLoopHandler : MonoBehaviour
{
    public List<GameObject> customerPrefabs;
    public List<GameObject> enemyObstaclePrefabs;

    public GameObject arrivedCirclePrefab;

    public List<Transform> spawnpoints;

    public GameObject player;
    private CarPassengerPickUpHandler carPassengerHandler;
    public float minDistanceFromCustomerToPlayer = 10;
    public float maxDistanceFromCustomerToPlayer = 20;

    public float minDistanceBetweenCustomerandArrive = 50;
    public float maxDistanceBetweenCustomerandArrive = 100;

    public UnityEvent<GameObject> OnArriveCircleChange = new();
    public UnityEvent<TargetLocationMarker> OnNewLocationMarker = new();

    public float amountOfEnemys = 40;
    public float minSpawnDistanceOfEnemys = 20f;
    public float maxSpawnDIstanceEnemys = 40f;


    void Start()
    {
        if(!player)
        {
             player = GameObject.FindGameObjectWithTag("Player");
        }
        player.GetComponent<CarTimeHandler>().OnCountDownFinished.AddListener(SpawnNewCostumer);
        carPassengerHandler = player.GetComponent<CarPassengerPickUpHandler>();
        carPassengerHandler.OnDropCustomer.AddListener(SpawnNewCostumer);
        SpawnEnemysNew();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnNewCostumer()
    {
        Transform customerSpawnPos = FindCustomerPlace();
        Transform arrivePos = FindArrivePlace(customerSpawnPos);
        int randomeCustomer = Random.Range(0, customerPrefabs.Count);

        GameObject arriveCircle = GameObject.Instantiate(arrivedCirclePrefab, arrivePos.transform.position, Quaternion.identity);
        OnArriveCircleChange.Invoke(arriveCircle);

        GameObject customer = GameObject.Instantiate(customerPrefabs[randomeCustomer], customerSpawnPos.transform.position, Quaternion.identity);
        customer.transform.forward = arrivePos.transform.forward;
        OnNewLocationMarker.Invoke(customer.GetComponent<TargetLocationMarker>());

    }




    Transform FindCustomerPlace()
    {
        foreach(Transform customerPos in spawnpoints)
        {
            float distanceToPlayer = Vector2.Distance(player.transform.position, customerPos.position);
            if(distanceToPlayer > minDistanceFromCustomerToPlayer && distanceToPlayer < maxDistanceFromCustomerToPlayer)
            {
                return customerPos;
            }
        }
        return spawnpoints[0];
    }

    Transform FindArrivePlace(Transform customer)
    {
        foreach (Transform arrivePos in spawnpoints)
        {
            if(customer == arrivePos){
                continue;
            }
            float distanceToPlayer = Vector2.Distance(customer.position, arrivePos.position);
            if (distanceToPlayer > minDistanceBetweenCustomerandArrive && distanceToPlayer < maxDistanceBetweenCustomerandArrive)
            {
                return arrivePos;
            }
        }
        return spawnpoints[0];
    }

    public void SpawnEnemysNew()
    {
        for(int i = 0; i< amountOfEnemys; i++)
        {
            int randomeSPawnpointIndex = Random.Range(0, spawnpoints.Count);

            Vector3 spawnPosition = spawnpoints[randomeSPawnpointIndex].position;
            if (RandomPoint(spawnPosition, minSpawnDistanceOfEnemys, maxDistanceBetweenCustomerandArrive, out spawnPosition))
            {
                int randomeEnemyIndex = Random.Range(0, enemyObstaclePrefabs.Count);
                GameObject spawnedEnemy = GameObject.Instantiate(enemyObstaclePrefabs[randomeEnemyIndex], spawnPosition, Quaternion.identity);
            }
        }
    }

    bool RandomPoint(Vector3 center,float minRange ,float maxRange, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * Random.Range(minRange,maxRange);
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

    private void OnDrawGizmos() {
        if(spawnpoints.Count > 0) {
            Gizmos.color = Color.red;
            foreach(Transform spawnPoint in spawnpoints) {
                Gizmos.DrawSphere(spawnPoint.position, 0.25f);
            }
        }
        
    }
}
