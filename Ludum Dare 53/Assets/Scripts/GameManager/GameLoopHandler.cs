using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoopHandler : MonoBehaviour
{
    public List<GameObject> customerPrefabs;
    public List<GameObject> enemyObstaclePrefabs;

    public GameObject arrivedCirclePrefab;

    public List<Transform> spawnpoints;

    public GameObject player;
    public float minDistanceFromCustomerToPlayer = 10;
    public float maxDistanceFromCustomerToPlayer = 20;

    public float minDistanceBetweenCustomerandArrive = 50;
    public float maxDistanceBetweenCustomerandArrive = 100;

    void Start()
    {
        if(!player)
        {
             player = GameObject.FindGameObjectWithTag("Player");
        }
        player.GetComponent<CarTimeHandler>().OnCountDownFinished.AddListener(SpawnNewCostumer);

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
        GameObject customer = GameObject.Instantiate(customerPrefabs[randomeCustomer], customerSpawnPos.transform.position, Quaternion.identity);
        customer.transform.forward = arrivePos.transform.forward;
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

}
