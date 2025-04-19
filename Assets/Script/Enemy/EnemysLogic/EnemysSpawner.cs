using UnityEngine;
using Mirror;
public class EnemysSpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public Transform[] spawnPoints;
    private float timer;
    void Update()
    {
        if (!isServer) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }
    [Server]
    void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0)
            return;
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(enemy);
    }
}