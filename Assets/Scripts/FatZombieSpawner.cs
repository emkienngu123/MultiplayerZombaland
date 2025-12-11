using UnityEngine;
using Photon.Pun;

public class FatZombieSpawner : MonoBehaviourPun
{
    [Header("Spawner Settings")]
    [Tooltip("The name of the prefab in the Resources folder.")]
    public string zombiePrefabName = "FatZombie";
    public float spawnInterval = 10f;
    public Transform[] spawnPoints;

    private float nextSpawnTime = 0f;

    void Start()
    {
        Debug.Log("FatZombieSpawner: Script Started! I am in the scene.");
        if (PhotonNetwork.IsMasterClient)
        {
             Debug.Log("FatZombieSpawner: I am the Master Client. I will spawn zombies.");
        }
        else
        {
             Debug.Log("FatZombieSpawner: I am NOT Master Client. I will NOT spawn zombies.");
        }
    }

    void Update()
    {
        // Only Master Client spawns enemies to avoid duplicates
        if (PhotonNetwork.IsMasterClient == false) return;

        if (Time.time >= nextSpawnTime)
        {
            Debug.Log("FatZombieSpawner: Attempting to spawn...");
            SpawnZombie();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnZombie()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("FatZombieSpawner: No Spawn Points assigned! Please verify the inspector.");
            return;
        }

        // Choose random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Debug.Log($"FatZombieSpawner: Spawning at {spawnPoint.name} position {spawnPoint.position}");

        // Randomize rotation
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // Instantiate over network
        GameObject zombie = PhotonNetwork.Instantiate(zombiePrefabName, spawnPoint.position, randomRotation);
        if (zombie != null)
        {
             Debug.Log("FatZombieSpawner: Successfully instantiated FatZombie.");
        }
        else
        {
             Debug.LogError($"FatZombieSpawner: Failed to Instantiate. Check if '{zombiePrefabName}' is in Assets/Resources folder.");
        }
    }
}
