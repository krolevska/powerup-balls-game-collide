using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    // The target towards which the rocket will move
    private Transform target;

    // Speed at which the rocket moves
    public float speed = 15.0f;

    // Flag to determine if the rocket should home in on the target
    private bool homing;

    // Strength of the rocket's impact
    private float rocketStrength = 15.0f;

    // The time after which the rocket is automatically destroyed, even if it hasn't hit a target
    private float aliveTimer = 5.0f;

    // Update method called once per frame
    void Update()
    {
        // If the rocket is in homing mode and there's a target
        if (homing && target != null)
        {
            // Calculate the direction in which to move towards the target
            Vector3 moveDirection = (target.transform.position - transform.position).normalized;

            // Move the rocket towards the target
            transform.position += moveDirection * speed * Time.deltaTime;

            // Make the rocket look (face) towards the target
            transform.LookAt(target);
        }
    }

    // Method to fire the rocket towards a new target
    public void Fire(Transform newTarget)
    {
        // Set the target for the rocket
        target = newTarget;

        // Activate homing mode
        homing = true;

        // Destroy the rocket after a specific amount of time to avoid it lingering indefinitely
        Destroy(gameObject, aliveTimer);
    }

    // Method called when the rocket collides with another object
    private void OnCollisionEnter(Collision collision)
    {
        // If there's a target set for the rocket
        if (target != null)
        {
            // Check if the rocket hit the intended target
            if (collision.gameObject.CompareTag(target.tag))
            {
                // Retrieve the rigidbody of the target for applying physics-based effects
                Rigidbody targetRigidbody = collision.gameObject.GetComponent<Rigidbody>();

                // Calculate the direction away from the impact point
                Vector3 away = -collision.contacts[0].normal;

                // Apply a force to the target in the opposite direction of the collision
                targetRigidbody.AddForce(away * rocketStrength, ForceMode.Impulse);

                // Destroy the rocket now that it has hit its target
                Destroy(gameObject);
            }
        }
    }
}
