using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public static SpawnPoints Instance;
    [SerializeField] List<Transform> spawnPointList = new List<Transform>();

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetRandomSpawn()
    {
        int random = Random.Range(0, spawnPointList.Count);

        return spawnPointList[random];
    }
}
