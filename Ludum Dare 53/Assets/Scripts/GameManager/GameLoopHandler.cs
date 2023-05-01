using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    void Start()
    {
        if(!player)
        {
             player = GameObject.FindGameObjectWithTag("Player");
        }
        player.GetComponent<CarTimeHandler>().OnCountDownFinished.AddListener(SpawnNewCostumer);
        carPassengerHandler = player.GetComponent<CarPassengerPickUpHandler>();
        carPassengerHandler.OnDropCustomer.AddListener(SpawnNewCostumer);
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


    private void OnDrawGizmos() {
        if(spawnpoints.Count > 0) {
            Gizmos.color = Color.red;
            foreach(Transform spawnPoint in spawnpoints) {
                Gizmos.DrawSphere(spawnPoint.position, 0.25f);
            }
        }
    }
}
