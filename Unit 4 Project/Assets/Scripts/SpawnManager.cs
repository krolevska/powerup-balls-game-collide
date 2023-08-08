using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefab;
    public GameObject[] powerupPrefab;
    int powerIndex, enemyIndex;
    private float spawnRange = 9.0f;
    public int waveNumber = 1;
    public int enemyCount;
    public int fireCount;

    public GameObject player;

    public GameObject bossPrefab;
    public GameObject[] miniEnemyPrefabs;
    public int bossRound;
    // Start is called before the first frame update
    void Start()
    {
        powerIndex = Random.Range(0, powerupPrefab.Length);
        player = GameObject.Find("Player");
        SpawnEnemyWave(waveNumber);
        Instantiate(powerupPrefab[powerIndex], GenerateRandomPos(), powerupPrefab[powerIndex].transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount == 0)
        {
            waveNumber++;

            if (waveNumber % bossRound == 0)
            {
                SpawnBossWave(waveNumber);
            }
            else
            {
                SpawnEnemyWave(waveNumber);
            }
            powerIndex = Random.Range(0, powerupPrefab.Length);
            Instantiate(powerupPrefab[powerIndex], GenerateRandomPos(), powerupPrefab[powerIndex].transform.rotation);
        }


    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            enemyIndex = Random.Range(0, enemyPrefab.Length);
            Instantiate(enemyPrefab[enemyIndex], GenerateRandomPos(), enemyPrefab[enemyIndex].transform.rotation);
        }
    }

 
    private Vector3 GenerateRandomPos()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }

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
        var boss = Instantiate(bossPrefab, GenerateRandomPos(), bossPrefab.transform.rotation);
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn;
    }

    public void SpawnMiniEnemy(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomMini = Random.Range(0, miniEnemyPrefabs.Length);
            Instantiate(miniEnemyPrefabs[randomMini], GenerateRandomPos(), miniEnemyPrefabs[randomMini].transform.rotation);
        }
    }
}
