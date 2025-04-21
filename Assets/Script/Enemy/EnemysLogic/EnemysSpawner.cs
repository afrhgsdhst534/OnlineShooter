using UnityEngine;
using Mirror;
public class EnemysSpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public Transform[] spawnPoints;
    private float timer;
    public bool active;
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
    public void Active()
    {
        active = true;
    }
    [Server]
    void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0||!active)
            return;
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(enemy);
    }
}