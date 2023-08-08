using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Variables for the enemy's physics and movement
    private Rigidbody enemyRb;
    private GameObject player;

    // Movement speed of the enemy
    public float speed = 1.0f;

    // Flag to check if the enemy is a boss
    public bool isBoss = false;

    // Variables to manage when the boss spawns mini enemies
    public float spawnInterval;          // Time interval between spawning mini enemies
    private float nextSpawn;             // Time at which the next mini enemy will spawn
    public int miniEnemySpawnCount;      // Number of mini enemies to spawn

    // Reference to the SpawnManager to allow spawning of mini enemies
    private SpawnManager spawnManager;

    // This method is called when the script instance is being loaded
    void Start()
    {
        // Getting the Rigidbody component for physics calculations
        enemyRb = GetComponent<Rigidbody>();

        // Finding and assigning the player object for target tracking
        player = GameObject.Find("Player");

        // If this enemy is a boss, find and assign the SpawnManager for spawning mini enemies
        if (isBoss)
        {
            spawnManager = FindObjectOfType<SpawnManager>();
        }
    }

    // Update method called once per frame
    void Update()
    {
        // Calculate direction towards the player
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;

        // Move the enemy towards the player
        enemyRb.AddForce(lookDirection * speed);

        // If the enemy is a boss, handle the spawning of mini enemies
        if (isBoss)
        {
            // Check if it's time to spawn the next wave of mini enemies
            if (Time.time > nextSpawn)
            {
                // Update the next spawn time
                nextSpawn = Time.time + spawnInterval;

                // Command the spawn manager to spawn the mini enemies
                spawnManager.SpawnMiniEnemy(miniEnemySpawnCount);
            }
        }

        // If the enemy falls below y = -10, destroy it
        if (enemyRb.transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }
}
