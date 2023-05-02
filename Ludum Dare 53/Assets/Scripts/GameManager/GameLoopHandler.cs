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

    GameObject currentArriveCircle;
    public LayerMask groundLayer;
    private int currentRound = 0;
    public float difficultAmount = 10;


    void Start()
    {
        if(!player)
        {
             player = GameObject.FindGameObjectWithTag("Player");
        }
        player.GetComponent<CarTimeHandler>().OnCountDownFinished.AddListener(SpawnNewCostumer);
        carPassengerHandler = player.GetComponent<CarPassengerPickUpHandler>();
        carPassengerHandler.OnPickUpCustomer.AddListener(ChangeCompassMarker);
        carPassengerHandler.OnDropCustomer.AddListener(SpawnNewCostumer);
        SpawnEnemysNew();
        currentRound = 0;
    }

    private void ChangeCompassMarker()
    {
        OnNewLocationMarker.Invoke(currentArriveCircle.GetComponent<TargetLocationMarker>());
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

        RaycastHit arriveSpawnHit;
        Vector3 ArriveSpawnPos = Vector3.zero;
        if (Physics.Raycast(arrivePos.transform.position, Vector3.down, out arriveSpawnHit, 10f, groundLayer))
        {
            ArriveSpawnPos = arriveSpawnHit.point;
        }
        else
        {
            ArriveSpawnPos = arrivePos.transform.position;
        }

        currentArriveCircle = GameObject.Instantiate(arrivedCirclePrefab, ArriveSpawnPos, Quaternion.identity);
        OnArriveCircleChange.Invoke(currentArriveCircle);

        RaycastHit customerSpawnHit;
        Vector3 CustomerSpawnPos = Vector3.zero;
        if (Physics.Raycast(customerSpawnPos.transform.position, Vector3.down, out customerSpawnHit,10f, groundLayer)){
            CustomerSpawnPos = customerSpawnHit.point;
        }
        else{
            CustomerSpawnPos = customerSpawnPos.transform.position;
        }
        GameObject customer = GameObject.Instantiate(customerPrefabs[randomeCustomer], CustomerSpawnPos, Quaternion.identity);
        customer.transform.forward = arrivePos.transform.forward;
        OnNewLocationMarker.Invoke(customer.GetComponent<TargetLocationMarker>());

        minDistanceFromCustomerToPlayer += currentRound * difficultAmount/2;
        maxDistanceFromCustomerToPlayer += currentRound * difficultAmount/2;
        minDistanceBetweenCustomerandArrive += currentRound * difficultAmount;
        maxDistanceBetweenCustomerandArrive += currentRound * difficultAmount;
        currentRound++;
    }




    Transform FindCustomerPlace()
    {
        for(int i = 0; i< spawnpoints.Count; i++)
        {
            int randomeIndex = Random.Range(0, spawnpoints.Count);
            float distanceToPlayer = Vector2.Distance(player.transform.position, spawnpoints[randomeIndex].position);
            if (distanceToPlayer > minDistanceFromCustomerToPlayer && distanceToPlayer < maxDistanceFromCustomerToPlayer)
            {
                return spawnpoints[randomeIndex];
            }
        }
        return spawnpoints[Random.Range(0, spawnpoints.Count)];
    }

    Transform FindArrivePlace(Transform customer)
    {
        for(int i = 0; i< spawnpoints.Count; i++)
        {
            int randomeIndex = Random.Range(0, spawnpoints.Count);
            if (customer == spawnpoints[randomeIndex])
            {
                continue;
            }
            float distanceToPlayer = Vector2.Distance(customer.position, spawnpoints[randomeIndex].position);
            if (distanceToPlayer > minDistanceBetweenCustomerandArrive && distanceToPlayer < maxDistanceBetweenCustomerandArrive)
            {
                return spawnpoints[randomeIndex];
            }
        }


        return spawnpoints[Random.Range(0, spawnpoints.Count)];
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
                spawnedEnemy.GetComponent<MutantHandler>().OnKilled.AddListener(SpawnEnemy);
                spawnedEnemy.transform.parent = transform;
            }
        }
    }

    public void SpawnEnemy()
    {
        for (int i = 0; i < spawnpoints.Count; i++)
        {
            int randomeIndex = Random.Range(0, spawnpoints.Count);
            float distanceToPlayer = Vector2.Distance(player.transform.position, spawnpoints[randomeIndex].position);
            if (distanceToPlayer > minDistanceFromCustomerToPlayer && distanceToPlayer < maxDistanceFromCustomerToPlayer)
            {
                int randomeEnemyIndex = Random.Range(0, enemyObstaclePrefabs.Count);
                GameObject spawnedEnemy = GameObject.Instantiate(enemyObstaclePrefabs[randomeEnemyIndex], spawnpoints[randomeIndex].position, Quaternion.identity);
                spawnedEnemy.GetComponent<MutantHandler>().OnKilled.AddListener(SpawnEnemy);
                spawnedEnemy.transform.parent = transform;
                return;
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
