using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceDestroyer : MonoBehaviour
{
    public GameObject destroyParticle;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject particle = GameObject.Instantiate(destroyParticle, transform.position, Quaternion.identity);
            Destroy(particle, 1f);
            Destroy(gameObject);
        }
    }
}
