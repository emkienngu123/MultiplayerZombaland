using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;

    [Space]

    public Transform[] spawnPoints;
    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedPoint = spawnPoints[randomIndex];

        Debug.Log("Spawning Player at Point index: " + randomIndex);
        Debug.Log("Spawn Player");
        PhotonNetwork.Instantiate(player.name, selectedPoint.position, Quaternion.identity);
    }
}
