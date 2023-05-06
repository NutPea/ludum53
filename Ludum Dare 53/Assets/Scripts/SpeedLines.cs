using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLines : MonoBehaviour
{
    public float activationVelocity5 = 26f; // The velocity threshold to activate the particle system
    public ParticleSystem particleSystem1; // Reference to the particle system

    private Rigidbody Car_rigidbody;

    private void Start()
    {
        Car_rigidbody = GetComponent<Rigidbody>();


        particleSystem1 = GetComponentInChildren<ParticleSystem>();
        // Disable the particle system at the start
        particleSystem1.Stop();
    }

    private void Update()
    {

        //Debug.Log(Car_rigidbody.velocity.magnitude);
        // Check if the mesh's velocity exceeds the activation velocity
        if (Car_rigidbody.velocity.magnitude >= activationVelocity5)
        {
            // If the velocity is above the threshold, and the particle system is not already playing, start it
            if (!particleSystem1.isPlaying)
            {
                particleSystem1.Play();
            }
        }
        else
        {
            // If the velocity is below the threshold, and the particle system is playing, stop it
            if (particleSystem1.isPlaying)
            {
                particleSystem1.Stop();
            }
        }
    }
}
