using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb; // Rigidbody component of the player
    public float speed = 10.0f; // Movement speed of the player
    public bool hasPowerup = false; // Flag to check if player has a powerup
    private float powerupStrength = 15.0f; // Strength of the pushback powerup

    private GameObject focalPoint; // Point towards which the player moves
    public GameObject powerIndicator; // UI/Visual indicator for the powerup

    public PowerUpType currentPowerUp = PowerUpType.None; // Enum to track the current powerup

    public GameObject rocketPrefab; // Prefab for rockets to be launched
    private GameObject tmpRocket; // Temporary reference for instantiated rockets
    private Coroutine powerupCountdown; // Reference for the powerup duration countdown

    // Smash powerup variables
    public float hangTime = 0.5f;  
    public float smashSpeed = 5.0f;  
    public float explosionForce = 10.0f;  
    public float explosionRadius = 10.0f; 
    bool smashing = false; // Flag to check if the smash action is ongoing
    float floorY; // Y-position of the floor (or starting y-position)

    void Start()
    {
        playerRb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        focalPoint = GameObject.Find("FocalPoint"); // Find the FocalPoint game object
    }

    void Update()
    {
        // Handle player movement
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);

        // Position the power indicator below the player
        powerIndicator.transform.position = playerRb.transform.position + new Vector3(0f, -0.5f, 0f);

        // Launch rockets if player has the Rockets powerup and presses 'F'
        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets();
        }

        // Start the smash action if player has the Smash powerup and presses 'Space'
        if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }
    }

    // Coroutine to disable powerup after a duration
    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        powerIndicator.SetActive(false);
        currentPowerUp = PowerUpType.None;
    }

    // Trigger logic for powerups
    private void OnTriggerEnter(Collider other)
    {
        // Check if collided with a powerup
        if (other.CompareTag("PowerUp"))
        {
            Destroy(other.gameObject); // Destroy the powerup
            hasPowerup = true; // Set the flag to true
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType; // Get the type of powerup
            powerIndicator.SetActive(true); // Activate the power indicator
        }

        // Stop any ongoing powerup countdown and start a new one
        if (powerupCountdown != null)
        {
            StopCoroutine(powerupCountdown);
        }
        powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
    }

    // Collision logic with the enemy
    private void OnCollisionEnter(Collision collision)
    {
        // Check if collided with an enemy and has the Pushback powerup
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.Pushback)
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>(); // Get the enemy's Rigidbody
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position; // Direction away from player
            enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse); // Push the enemy
            Debug.Log("Player collided with: " + collision.gameObject.name + " with powerup set to " + currentPowerUp.ToString());
        }
    }

    // Function to launch rockets towards all enemies
    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity); // Instantiate a rocket
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform); // Launch it towards the enemy
        }
    }

    // Coroutine for the Smash action
    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>(); // Get all enemies

        floorY = transform.position.y; // Set the starting y-position
        float jumpTime = Time.time + hangTime; // Calculate time to jump

        // Jump logic
        while (Time.time < jumpTime)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        // Fall back down logic
        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }

        // Apply explosion force to enemies in radius
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }
            smashing = false; // End the smash action
        }
    }
}
