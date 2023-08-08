using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Arrays of enemy and powerup prefabs
    public GameObject[] enemyPrefab;
    public GameObject[] powerupPrefab;

    // Indices for randomly selecting an enemy and a powerup
    int powerIndex, enemyIndex;

    // Range within which entities will be spawned
    private float spawnRange = 9.0f;

    // Wave counter and the count of enemies and fires
    public int waveNumber = 1;
    public int enemyCount;
    public int fireCount;

    // Reference to the player
    public GameObject player;

    // Prefab for boss and mini-enemies, and counter to determine boss wave 
    public GameObject bossPrefab;
    public GameObject[] miniEnemyPrefabs;
    public int bossRound;

    // Start is called before the first frame update
    void Start()
    {
        // Randomly pick a powerup type to spawn
        powerIndex = Random.Range(0, powerupPrefab.Length);

        // Find the player in the scene
        player = GameObject.Find("Player");

        // Spawn the initial wave of enemies
        SpawnEnemyWave(waveNumber);

        // Spawn a powerup
        Instantiate(powerupPrefab[powerIndex], GenerateRandomPos(), powerupPrefab[powerIndex].transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        // Count the number of enemies currently in the scene
        enemyCount = FindObjectsOfType<Enemy>().Length;

        // If all enemies have been defeated
        if (enemyCount == 0 && player.GetComponent<PlayerController>().lives > 1)
        {
            waveNumber++;

            // If it's a boss round
            if (waveNumber % bossRound == 0)
            {
                SpawnBossWave(waveNumber);
            }
            else
            {
                SpawnEnemyWave(waveNumber);
            }

            // Spawn a new powerup after completing the wave
            powerIndex = Random.Range(0, powerupPrefab.Length);
            Instantiate(powerupPrefab[powerIndex], GenerateRandomPos(), powerupPrefab[powerIndex].transform.rotation);
        }
    }

    // Function to spawn a wave of random enemies
    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            enemyIndex = Random.Range(0, enemyPrefab.Length);
            Instantiate(enemyPrefab[enemyIndex], GenerateRandomPos(), enemyPrefab[enemyIndex].transform.rotation);
        }
    }

    // Function to generate a random position within the defined spawn range
    private Vector3 GenerateRandomPos()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }

    // Function to spawn a boss and its associated mini-enemies
    void SpawnBossWave(int currentRound)
    {
        int miniEnemysToSpawn;

        if (bossRound != 0)
        {
            miniEnemysToSpawn = currentRound / bossRound;
        }
        else
        {
            miniEnemysToSpawn = 1;
        }

        // Spawn the boss
        var boss = Instantiate(bossPrefab, GenerateRandomPos(), bossPrefab.transform.rotation);

        // Assign the number of mini-enemies this boss should spawn
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn;
    }

    // Function to spawn a specified amount of mini-enemies
    public void SpawnMiniEnemy(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomMini = Random.Range(0, miniEnemyPrefabs.Length);
            Instantiate(miniEnemyPrefabs[randomMini], GenerateRandomPos(), miniEnemyPrefabs[randomMini].transform.rotation);
        }
    }
}
